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

    public async Task<Validated<FullName>> CreateFullName(string title, string givenName, string familyName, string tenantID = GlobalValues.Default_TenantID)
    {
        var configurations = await _cacheRepository.GetAllTenantConfigurations("ValidationConfigurations");
        
        //Func<string, string, List<ValidationRuleConfig>> getConfigs = (typeName, propertyName) => TenantValidationHelper.GetTenantConfigs(typeName, propertyName, tenantID, configurations);

        //var familyNameValidator = ValueObjectValidationBuilder.BuildFamilyNameValidator(getConfigs, _validationFactoryProvider); //this was in the video now removed
        //var givenNameValidator  = ValueObjectValidationBuilder.BuildGivenNameValidator(getConfigs,  _validationFactoryProvider);
        //var titleValidator      = ValueObjectValidationBuilder.BuildTitleValidator(getConfigs,      _validationFactoryProvider);

        
        var familyNameValidator = ValidatorFactories.CreatePropertyValidator<string>(tenantID, typeof(FullName).FullName!, nameof(FullName.FamilyName), configurations, _validationFactoryProvider);
        var givenNameValidator  = ValidatorFactories.CreatePropertyValidator<string>(tenantID, typeof(FullName).FullName!, nameof(FullName.GivenName), configurations, _validationFactoryProvider);
        var titleValidator      = ValidatorFactories.CreatePropertyValidator<string>(tenantID, typeof(FullName).FullName!, nameof(FullName.Title), configurations, _validationFactoryProvider);

        return  base.CreateFullName(titleValidator(title), givenNameValidator(givenName), familyNameValidator(familyName)); //<< this can access the InternalCreate method
    }
    /*
        * Moved the GetTenantConfigs method to the TenantValidationHelper class as the method could be used by any project
    */
}
