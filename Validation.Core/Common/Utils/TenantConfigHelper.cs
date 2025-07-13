using System.Collections.Immutable;
using Validation.Core.Types;

public static class TenantConfigHelper
{
    public static List<ValidationRuleConfig> GetTenantConfigs(string typeName, string propertyName, string tenantID, ImmutableList<ValidationRuleConfig> configurations)

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