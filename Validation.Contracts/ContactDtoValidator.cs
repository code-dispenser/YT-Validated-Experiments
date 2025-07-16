using Validation.Contracts.DTOs;
using Validation.Core.Factories;
using Validation.Core.Types;
using Validation.Core.Validator;

namespace Validation.Contracts;

public class ContactDtoValidator : IObjectValidator<ContactDto>
{
    public Validated<ContactDto> Validate(ContactDto dataObject)
    {
        List<IValidated> validations = //Use prebuilt validators
        [
            ValidatorFactories.CreateFamilyNameValidator()(dataObject.FamilyName),// create validator and then execute with value
            ValidatorFactories.CreateGivenNameValidator()(dataObject.GivenName),
            ValidatorFactories.CreateTitleValidator()(dataObject.Title),
            ValidatorFactories.CreateDateOfBirthValidator(DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-122), DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18))(dataObject.DOB)
        ];
        /*
            * Or create a different validator. 
        */ 
        var emailRule = ValidatorFactories.CreatePredicateValidator<ContactDto>(c => c.Email.Contains("@gmail.com", StringComparison.InvariantCultureIgnoreCase), nameof(ContactDto.Email),"Email", "Must be a gmail address");

        validations.Add(emailRule(dataObject));

        /* 
            * Optional property values if not using the Option type otherwise if Option.IsSome etc.
         */
        if (dataObject.Mobile != null && dataObject.Mobile.Trim() != String.Empty) validations.Add(ValidatorFactories.CreateMobileValidator()(dataObject.Mobile));

        /*
            * Address property validations could be done here or in a separate validator class as below 
         */
        if (dataObject.Address is not null) validations.Add(new AddressDTOValidator().Validate(dataObject.Address));


        var invalidEntries = validations.Where(f => f.IsInvalid).SelectMany(f => f.Failures).ToList();

        return invalidEntries.Count > 0 ? Validated<ContactDto>.Invalid(invalidEntries) : Validated<ContactDto>.Valid(dataObject);
    }
}
