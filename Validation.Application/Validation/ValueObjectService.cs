﻿using System.Collections.Immutable;
using Validation.Application.Common.Seeds;
using Validation.Core.Factories;
using Validation.Core.Types;
using Validation.Domain.Seeds;
using Validation.Domain.ValueObjects;

namespace Validation.Application.Validation;

public class ValueObjectService (ICacheRepository cacheRepository, ValidationFactoryProvider validationFactoryProvider) : ValueObjectServiceBase
{
    private readonly ValidationFactoryProvider _validationFactoryProvider = validationFactoryProvider;
    private readonly ICacheRepository          _cacheRepository           = cacheRepository;

    public async Task<Validated<FullName>> CreateFullName(string title, string givenName, string familyName, string tenantID = "ALL")
    {
        var configurations = await _cacheRepository.GetAllTenantConfigurations("ValidationConfigurations");
        
        Func<string, string, List<ValidationRuleConfig>> getConfigs = (typeName, propertyName) => GetTenantConfigs(typeName, propertyName, tenantID, configurations);

        var familyNameValidator = ValueObjectValidationBuilder.BuildFamilyNameValidator(getConfigs, _validationFactoryProvider);
        var givenNameValidator  = ValueObjectValidationBuilder.BuildGivenNameValidator(getConfigs,  _validationFactoryProvider);
        var titleValidator      = ValueObjectValidationBuilder.BuildTitleValidator(getConfigs,      _validationFactoryProvider);

        return  base.CreateFullName(titleValidator(title), givenNameValidator(givenName), familyNameValidator(familyName)); //<< this can access the InternalCreate method
    }

    private static List<ValidationRuleConfig> GetTenantConfigs(string typeName, string propertyName, string tenantID, ImmutableList<ValidationRuleConfig> configurations)

        => configurations.Where(c => c.TypeFullName == typeName && c.PropertyName == propertyName)
                    .GroupBy(c => new { c.RuleType, c.Pattern, c.MinLength, c.MaxLength, c.MinValue, c.MaxValue }) // group by uniquely identifying rule content
                    .Select(group =>
                    {
                        var tenantSpecific = group.FirstOrDefault(config => config.TenantID == tenantID);
                        /*
                            * try to find a tenant-specific configuration or fall back to a default one - All
                        */ 
                        return tenantSpecific ?? group.FirstOrDefault(config => string.IsNullOrWhiteSpace(config.TenantID) || config.TenantID == "ALL");
                    })
                    .Where(config => config is not null).ToList()!;


}
