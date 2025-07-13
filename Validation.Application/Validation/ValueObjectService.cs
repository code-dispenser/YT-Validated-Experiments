using Validation.Application.Common.Seeds;
using Validation.Core.Common.Constants;
using Validation.Core.Factories;
using Validation.Core.Types;
using Validation.Domain.Seeds;
using Validation.Domain.ValueObjects;

namespace Validation.Application.Validation;

public class ValueObjectService (ICacheRepository cacheRepository, ValidationFactoryProvider validationFactoryProvider) : ValueObjectServiceBase
{
    private readonly ValidationFactoryProvider _validationFactoryProvider = validationFactoryProvider;
    private readonly ICacheRepository          _cacheRepository           = cacheRepository;

    public async Task<Validated<FullName>> CreateFullName(string title, string givenName, string familyName, string tenantID = GlobalValues.DefaultTenantID)
    {
        var configurations = await _cacheRepository.GetAllTenantConfigurations("ValidationConfigurations");
        
        Func<string, string, List<ValidationRuleConfig>> getConfigs = (typeName, propertyName) => TenantConfigHelper.GetTenantConfigs(typeName, propertyName, tenantID, configurations);

        var familyNameValidator = ValueObjectValidationBuilder.BuildFamilyNameValidator(getConfigs, _validationFactoryProvider);
        var givenNameValidator  = ValueObjectValidationBuilder.BuildGivenNameValidator(getConfigs,  _validationFactoryProvider);
        var titleValidator      = ValueObjectValidationBuilder.BuildTitleValidator(getConfigs,      _validationFactoryProvider);

        return  base.CreateFullName(titleValidator(title), givenNameValidator(givenName), familyNameValidator(familyName)); //<< this can access the InternalCreate method
    }
    /*
        * Moved the GetTenantConfigs method to the TenantConfigHelper class as the method could be used by any project
    */
}
