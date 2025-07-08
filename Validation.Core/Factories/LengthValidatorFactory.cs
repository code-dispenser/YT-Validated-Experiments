using Validation.Core.Types;

namespace Validation.Core.Factories;

public class LengthValidatorFactory : IValidatorFactory
{
    public Func<T, Validated<T>> CreatePartial<T>(ValidationRuleConfig ruleConfig) where T : notnull

        => valueToValidate =>
        {
            var value = valueToValidate is null ? 0 : valueToValidate.ToString()!.Length;

            var valid = value >= ruleConfig.MinLength && value <= ruleConfig.MaxLength;

            return valid
                ? Validated<T>.Valid(valueToValidate!)
                    : Validated<T>.Invalid(new ValidationEntry(ruleConfig.PropertyName, ruleConfig.DisplayName, ruleConfig.FailureMessage));
        };
}
