using Validation.Core.Extensions;
using Validation.Core.Factories;
using Validation.Core.Types;

namespace Validation.Domain.ValueObjects;

public record FullName
{
    public string Title { get; }
    public string GivenName { get; }
    public string FamilyName { get; }

    private FullName(string title, string givenName, string familyName)

        => (Title, GivenName, FamilyName) = (title, givenName, familyName);

    public static Validated<FullName> Create(string title, string givenName, string familyName)
    {
        var validatedTitle      = ValidatorFactories.CreateTitleValidator<string>()(title);         // create and then execute the validator
        var validatedGivenName  = ValidatorFactories.CreateGivenNameValidator<string>()(givenName);
        var validatedFamilyName = ValidatorFactories.CreateFamilyNameValidator<string>()(familyName);

        Func<string, Func<string, Func<string, FullName>>> curriedFunc = title => given => family => new FullName(title, given, family);

        return Validated<Func<string, Func<string, Func<string, FullName>>>>.Valid(curriedFunc)
                .Apply(validatedTitle)
                    .Apply(validatedGivenName)
                        .Apply(validatedFamilyName);
    }

    internal static Validated<FullName> InternalCreate(Validated<string> validatedTitle, Validated<string> validatedGivenName, Validated<string> validatedFamilyName)
    {
        Func<string, Func<string, Func<string, FullName>>> curriedFunc = title => given => family => new FullName(title, given, family);

        return Validated<Func<string, Func<string, Func<string, FullName>>>>.Valid(curriedFunc)
                .Apply(validatedTitle)
                    .Apply(validatedGivenName)
                        .Apply(validatedFamilyName);

    }

}