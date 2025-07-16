using System.Collections.Immutable;
using Validation.Core.Factories;
using Validation.Core.Types;

namespace Validation.Core.Validator;

public interface ITenantObjectValidator<T> where T : notnull, new()
{
    Validated<T> Validate(string tenantID, T dataObject, List<ValidationRuleConfig> configurations, ValidationFactoryProvider validationFactoryProvider);
}