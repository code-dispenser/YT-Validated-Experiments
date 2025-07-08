using Validation.Core.Types;

namespace Validation.Core.Factories;

public interface IValidatorFactory
{
    Func<T, Validated<T>> CreatePartial<T>(ValidationRuleConfig ruleConfig) where T : notnull;
}
