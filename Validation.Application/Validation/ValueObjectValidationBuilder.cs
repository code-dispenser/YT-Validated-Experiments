using Validation.Core.Extensions;
using Validation.Core.Factories;
using Validation.Core.Types;
using Validation.Domain.ValueObjects;

namespace Validation.Application.Validation;

public static class ValueObjectValidationBuilder
{
    public static Func<string, Validated<string>> BuildFamilyNameValidator(Func<string, string, List<ValidationRuleConfig>> getConfigs, ValidationFactoryProvider validationFactories)
    {
        var configs = getConfigs(typeof(FullName).FullName!, nameof(FullName.FamilyName));

        return TenantValidationHelper.BuildValidator<string>(configs, validationFactories);
    }

    public static Func<string, Validated<string>> BuildGivenNameValidator(Func<string, string, List<ValidationRuleConfig>> getConfigs, ValidationFactoryProvider validationFactories)
    {
        var configs = getConfigs(typeof(FullName).FullName!, nameof(FullName.GivenName));

        return TenantValidationHelper.BuildValidator<string>(configs, validationFactories);
    }

    public static Func<string, Validated<string>> BuildTitleValidator(Func<string, string, List<ValidationRuleConfig>> getConfigs, ValidationFactoryProvider validationFactories)
    {
        var configs = getConfigs(typeof(FullName).FullName!, nameof(FullName.Title));

        return TenantValidationHelper.BuildValidator<string>(configs, validationFactories);
    }

    /*
         * Moved the BuildValidator methods to this static class TenantValidationHelper as it was not tied to any layer. 
    */
}
