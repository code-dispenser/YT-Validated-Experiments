using Validation.Core.Types;

namespace Validation.Core.Extensions;

public static class ValidatedExtensions
{

    // The TOut could be the finished value or the nested Func   
    public static Validated<TOut> Apply<TIn, TOut>(this Validated<Func<TIn, TOut>> validatedFunc, Validated<TIn> validatedItem) where TIn : notnull where TOut : notnull
    {
        if (validatedFunc.IsValid && validatedItem.IsValid)
        {
            var func   = validatedFunc.GetValueOr(default!);
            var value  = validatedItem.GetValueOr(default!);
            var result = func(value);

            return Validated<TOut>.Valid(result);
        }

        var failures = new List<ValidationEntry>();

        if (validatedFunc.IsInvalid) failures.AddRange(validatedFunc.Failures);
        if (validatedItem.IsInvalid) failures.AddRange(validatedItem.Failures);

        return Validated<TOut>.Invalid(failures);
    }

    public static Func<T, Validated<T>> AndThen<T>(this Func<T, Validated<T>> thisFunc, Func<T, Validated<T>> nextFunc) where T : notnull

         => input =>
         {
             var firstResult  = thisFunc(input);
             var secondResult = nextFunc(input);

             return (firstResult.IsValid && secondResult.IsValid)
                         ? Validated<T>.Valid(input)
                             : Validated<T>.Invalid([.. firstResult.Failures, .. secondResult.Failures]);
         };

}
