using System.Collections.Immutable;
using Validation.Core.Types;

namespace Validation.Application.Common.Seeds;

public interface ICacheRepository
{
    Task<ImmutableList<ValidationRuleConfig>> GetTenantConfigurations(string itemKey, string tenantID);
}
