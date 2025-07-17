using Validation.Contracts.DTOs;
using Validation.Core.Factories;
using Validation.Core.Types;
using Validation.Core.Validator;

namespace Validation.Contracts;

public class AddressDTOValidator : IObjectValidator<AddressDto>
{
    public Validated<AddressDto> Validate(AddressDto dataObject)
    {
        List<IValidated> validations =
        [
            ValidatorFactories.CreateRegexValidator<string>(@"^(?=.{5,250}$)(?!.* {2})(?!.*[,\-']{2})[A-Za-z0-9][A-Za-z0-9 ,\-\n']+[A-Za-z0-9]$", nameof(dataObject.AddressLine),
                                                            "Address Line","Must start with a letter or number and be 5 to 250 characters in length.")(dataObject.AddressLine),

            ValidatorFactories.CreateRegexValidator<string>(@"^(?=.{3,100}$)[A-Z](?!.* {2})(?!.*'{2})(?!.*-{2})[\-A-Za-z ']+[a-z]+$", nameof(dataObject.TownCity),
                                                "Town / City","Must start with a capital letter and be between 3 to 100 characters in length.")(dataObject.TownCity),

            ValidatorFactories.CreateRegexValidator<string>(@"^(?=.{3,100}$)[A-Z](?!.* {2})(?!.*'{2})(?!.*-{2})[\-A-Za-z ']+[a-z]+$", nameof(dataObject.TownCity),
                                                "County","Must start with a capital letter and be between 3 to 100 characters in length.")(dataObject.County)

        ];

        if (dataObject.Postcode is not null)
        {
            var validatedPostcode = ValidatorFactories.CreateRegexValidator<string>(@"^(GIR 0AA)|((([ABCDEFGHIJKLMNOPRSTUWYZ][0-9][0-9]?)|(([ABCDEFGHIJKLMNOPRSTUWYZ][ABCDEFGHKLMNOPQRSTUVWXY][0-9][0-9]?)|(([ABCDEFGHIJKLMNOPRSTUWYZ][0-9][ABCDEFGHJKSTUW])|([ABCDEFGHIJKLMNOPRSTUWYZ][ABCDEFGHKLMNOPQRSTUVWXY][0-9][ABEHMNPRVWXY])))) [0-9][ABDEFGHJLNPQRSTUWXYZ]{2})$",
                                                                                    nameof(dataObject.Postcode), "Postcode", "Must be a valid UK formatted postcode.")(dataObject.Postcode);
            validations.Add(validatedPostcode);
        }

        var invalidEntries = validations.Where(f => f.IsInvalid).SelectMany(f => f.Failures).ToList();

        return invalidEntries.Count > 0 ? Validated<AddressDto>.Invalid(invalidEntries) : Validated<AddressDto>.Valid(dataObject);

    }
}
