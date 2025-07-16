using System.Collections.Immutable;
using Validation.Core.Common.Constants;
using Validation.Core.Extensions;
using Validation.Core.Factories;
using Validation.Core.Types;

namespace Validation.Core.Common.Utils;

public static class TenantValidationHelper
{
    public static List<ValidationRuleConfig> GetTenantConfigs(string typeFullName, string propertyName, string tenantID, ImmutableList<ValidationRuleConfig> configurations)

        => configurations.Where(c => c.TypeFullName == typeFullName && c.PropertyName == propertyName)
                    .GroupBy(c => new { c.RuleType, c.Pattern, c.MinLength, c.MaxLength, c.MinValue, c.MaxValue }) // group by uniquely identifying rule content
                    .Select(group =>
                    {
                        var tenantSpecific = group.FirstOrDefault(config => config.TenantID == tenantID);
                        /*
                            * try to find a tenant-specific configuration or fall back to a default one - All
                        */
                        return tenantSpecific ?? group.FirstOrDefault(config => string.IsNullOrWhiteSpace(config.TenantID) || config.TenantID == GlobalValues.Default_TenantID);
                    })
                    .Where(config => config is not null).ToList()!;


    public static List<ValidationRuleConfig> GetTenantConfigs(string tenantID, ImmutableList<ValidationRuleConfig> configurations)

        => configurations.Where(c => c.TenantID == tenantID || c.TenantID == GlobalValues.Default_TenantID).ToList();

    public static ImmutableList<ValidationRuleConfig> GetTenantConfigs(string typeFullName, string tenantID, ImmutableList<ValidationRuleConfig> configurations)

        => configurations.Where(c => c.TypeFullName == typeFullName && (c.TenantID == tenantID || c.TenantID == GlobalValues.Default_TenantID)).ToImmutableList();


    public static Func<T, Validated<T>> BuildValidator<T>(List<ValidationRuleConfig> buildFor, ValidationFactoryProvider validationFactories) where T : notnull

        => buildFor.Select(ruleConfig => validationFactories.GetValidatorFactory(ruleConfig.RuleType).CreatePartial<T>(ruleConfig))
                        .ToList()
                            .Aggregate((current, next) => current.AndThen(next));
}
