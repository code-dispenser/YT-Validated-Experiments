using System.Text.RegularExpressions;
using Validation.Core.Types;

namespace Validation.Core.Factories;

public class RegexValidatorFactory : IValidatorFactory
{
    public Func<T, Validated<T>> CreatePartial<T>(ValidationRuleConfig ruleConfig) where T : notnull
    
        =>  valueToValidate =>  Regex.IsMatch(valueToValidate.ToString() ?? "", ruleConfig.Pattern)
                                    ? Validated<T>.Valid(valueToValidate!)
                                        : Validated<T>.Invalid(new ValidationEntry(ruleConfig.PropertyName, ruleConfig.DisplayName, ruleConfig!.FailureMessage));
}
