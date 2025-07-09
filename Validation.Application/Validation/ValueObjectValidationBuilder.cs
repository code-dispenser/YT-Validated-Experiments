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

        return BuildValidator<string>(configs, validationFactories);
    }

    public static Func<string, Validated<string>> BuildGivenNameValidator(Func<string, string, List<ValidationRuleConfig>> getConfigs, ValidationFactoryProvider validationFactories)
    {
        var configs = getConfigs(typeof(FullName).FullName!, nameof(FullName.GivenName));

        return BuildValidator<string>(configs, validationFactories);
    }

    public static Func<string, Validated<string>> BuildTitleValidator(Func<string, string, List<ValidationRuleConfig>> getConfigs, ValidationFactoryProvider validationFactories)
    {
        var configs = getConfigs(typeof(FullName).FullName!, nameof(FullName.Title));

        return BuildValidator<string>(configs, validationFactories);
    }

    public static Func<T, Validated<T>> BuildValidator<T>(List<ValidationRuleConfig> buildFor, ValidationFactoryProvider validationFactories) where T : notnull

        => buildFor.Select(ruleConfig => validationFactories.GetValidatorFactory(ruleConfig.RuleType).CreatePartial<T>(ruleConfig))
                        .ToList()
                            .Aggregate((current, next) => current.AndThen(next));
}
