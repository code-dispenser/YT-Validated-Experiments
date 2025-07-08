using System.Numerics;
using System.Security;
using Validation.Core.Types;

namespace Validation.Core.Factories;

public class BetweenValidatorFactory : IValidatorFactory
{
    public Func<T, Validated<T>> CreatePartial<T>(ValidationRuleConfig ruleConfig) where T : notnull
    
        => valueToValidate =>
        {
            var systemType = Type.GetType(String.Concat("System.", ruleConfig.MinMaxValueType.ToString()))!;

            bool valid   = false;
            var minValue = (IComparable)default!;
            var maxValue = (IComparable)default!;

            if (systemType == typeof(DateOnly))
            {
                minValue = (IComparable)DateOnly.Parse(ruleConfig.MinValue);
                maxValue = (IComparable)DateOnly.Parse(ruleConfig.MaxValue);
            }
            else
            {
                minValue = (IComparable)Convert.ChangeType(ruleConfig.MinValue, systemType);
                maxValue = (IComparable)Convert.ChangeType(ruleConfig.MaxValue, systemType);
            }
            
            valid = minValue.CompareTo(valueToValidate) <= 0 && maxValue.CompareTo(valueToValidate) >= 0;

            return valid
                ? Validated<T>.Valid(valueToValidate!)
                    : Validated<T>.Invalid(new ValidationEntry(ruleConfig.PropertyName, ruleConfig.DisplayName, ruleConfig.FailureMessage));
        };

}
