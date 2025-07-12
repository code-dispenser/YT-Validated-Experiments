using System.Text.RegularExpressions;
using Validation.Core.Extensions;
using Validation.Core.Types;

namespace Validation.Core.Factories;

public static class ValidatorFactories
{
    public static Func<T, Validated<T>> CreateRegexValidator<T>(string pattern, string propertyName, string displayName, string failureMessage) where T : notnull

        => valueToValidate => Regex.IsMatch(valueToValidate?.ToString() ?? "", pattern)
                              ? Validated<T>.Valid(valueToValidate!)
                                 : Validated<T>.Invalid(new ValidationEntry(propertyName, displayName, failureMessage));

    public static Func<T, Validated<T>> CreatePredicateValidator<T>(Func<T, bool> predicate, string propertyName, string displayName, string failureMessage) where T : notnull

        => valueToValidate => predicate(valueToValidate) ? Validated<T>.Valid(valueToValidate) : Validated<T>.Invalid(new ValidationEntry(propertyName, displayName, failureMessage));


    private static Func<string, Validated<string>> CreateStringLengthValidator(int minLength, int maxLength, string propertyName, string displayName, string failureMessage)

        => valueToValidate =>
        {
            int value = valueToValidate?.Length ?? 0;

            return (value >= minLength && value <= maxLength)
                ? Validated<string>.Valid(valueToValidate!)
                    : Validated<string>.Invalid(new ValidationEntry(propertyName, displayName, failureMessage));
        };

    public static Func<string, Validated<string>> CreateTitleValidator() 

        => CreateRegexValidator<string>(@"^(Mr|Mrs|Ms|Dr|Prof)$", "Title", "Title", "Must be one of Mr, Mrs, Ms, Dr, Prof");

    public static Func<string, Validated<string>> CreateGivenNameValidator() 

        => CreateRegexValidator<string>(@"^(?=.{2,50}$)[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]+$", "GivenName", "First name", "Must start with a capital letter and be between 2 and 50 characters in length");

    public static Func<string, Validated<string>> CreateFamilyNameValidator()
        
        => CreateRegexValidator<string>(@"^(?=.{2,50}$)[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]+$", "FamilyName", "Surname", "Must start with a capital letter")
            .AndThen(CreateStringLengthValidator(2, 50, "FamilyName", "Surname", "Must be between 2 and 50 characters long"));

    
}

