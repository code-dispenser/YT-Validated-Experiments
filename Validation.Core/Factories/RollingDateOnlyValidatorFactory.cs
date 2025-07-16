using Validation.Core.Types;

namespace Validation.Core.Factories;

internal class RollingDateOnlyValidatorFactory(Func<DateOnly> getToday) : IValidatorFactory
{
    public Func<T, Validated<T>> CreatePartial<T>(ValidationRuleConfig ruleConfig) where T : notnull
    
        => valueToValidate =>
           {
               var systemType = Type.GetType(String.Concat("System.", ruleConfig.MinMaxValueType.ToString()))!;

               bool valid = false;
              
               if (systemType != typeof(Int32) || valueToValidate is not DateOnly) return Validated<T>.Invalid(new ValidationEntry(ruleConfig.PropertyName, ruleConfig.DisplayName, ruleConfig.FailureMessage));

               var earliestDate = getToday().AddYears(int.Parse(ruleConfig.MinValue));
               var latestDate   = getToday().AddYears(int.Parse(ruleConfig.MaxValue));

               valid = ((IComparable)valueToValidate).CompareTo(earliestDate) >= 0 && ((IComparable)valueToValidate).CompareTo(latestDate) <= 0;

               var failureMessage = valid == true ? ruleConfig.FailureMessage : ruleConfig.FailureMessage.Replace("@MINDATE", earliestDate.ToString()).Replace("@MAXDATE", latestDate.ToString());
           
               return valid
                   ? Validated<T>.Valid(valueToValidate!)
                       : Validated<T>.Invalid(new ValidationEntry(ruleConfig.PropertyName, ruleConfig.DisplayName, failureMessage));
           };
    
}
