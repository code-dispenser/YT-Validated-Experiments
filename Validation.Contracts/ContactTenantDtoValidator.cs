using System.Collections.Immutable;
using Validation.Contracts.DTOs;
using Validation.Core.Common.Utils;
using Validation.Core.Factories;
using Validation.Core.Types;
using Validation.Core.Validator;

namespace Validation.Contracts
{
    public class ContactTenantDtoValidator : ITenantObjectValidator<ContactDto>
    {
        public Validated<ContactDto> Validate(string tenantID, ContactDto dataObject, List<ValidationRuleConfig> configurations, ValidationFactoryProvider validationFactoryProvider)
        {

            var ruleConfigs         = TenantValidationHelper.GetTenantConfigs(typeof(ContactDto).FullName!, tenantID, configurations.ToImmutableList());
     
            var validatedTitle      = ValidatorFactories.CreatePropertyValidator<string>(tenantID,  typeof(ContactDto).FullName!, nameof(ContactDto.Title), ruleConfigs, validationFactoryProvider)(dataObject.Title);
            var validatedGivenName  = ValidatorFactories.CreatePropertyValidator<string>(tenantID,  typeof(ContactDto).FullName!, nameof(ContactDto.GivenName), ruleConfigs, validationFactoryProvider)(dataObject.GivenName);
            var validatedFamily     = ValidatorFactories.CreatePropertyValidator<string>(tenantID,  typeof(ContactDto).FullName!, nameof(ContactDto.FamilyName), ruleConfigs, validationFactoryProvider)(dataObject.FamilyName);
            var validatedDOB        = ValidatorFactories.CreatePropertyValidator<DateOnly>(tenantID,typeof(ContactDto).FullName!, nameof(ContactDto.DOB), ruleConfigs, validationFactoryProvider)(dataObject.DOB);
            var validatedEmail      = ValidatorFactories.CreatePropertyValidator<string>(tenantID,  typeof(ContactDto).FullName!, nameof(ContactDto.Email), ruleConfigs, validationFactoryProvider)(dataObject.Email);

            List<IValidated> validations = [validatedTitle, validatedGivenName, validatedFamily, validatedDOB, validatedEmail];

            if(false == String.IsNullOrWhiteSpace(dataObject.Mobile))
            {
                var validatedMobile = ValidatorFactories.CreatePropertyValidator<string>(tenantID, typeof(ContactDto).FullName!, nameof(ContactDto.Mobile), ruleConfigs, validationFactoryProvider)(dataObject.Mobile);
                validations.Add(validatedMobile);
            }
            /*
                * This time just doing the address validation here 
            */
            if(dataObject.Address is not null)
            {
                ruleConfigs = TenantValidationHelper.GetTenantConfigs(typeof(AddressDto).FullName!, tenantID, configurations.ToImmutableList());
                var validatedAddressLine = ValidatorFactories.CreatePropertyValidator<string>(tenantID, typeof(AddressDto).FullName!, nameof(AddressDto.AddressLine), ruleConfigs, validationFactoryProvider)(dataObject.Address.AddressLine);
                var validatedTownCity    = ValidatorFactories.CreatePropertyValidator<string>(tenantID, typeof(AddressDto).FullName!, nameof(AddressDto.TownCity), ruleConfigs, validationFactoryProvider)(dataObject.Address.TownCity);
                var validatedCounty      = ValidatorFactories.CreatePropertyValidator<string>(tenantID, typeof(AddressDto).FullName!, nameof(AddressDto.County), ruleConfigs, validationFactoryProvider)(dataObject.Address.County);

                validations.AddRange(validatedAddressLine, validatedTownCity, validatedCounty);

                if(dataObject.Address.Postcode is not null)
                {
                    var validatedPostcode = ValidatorFactories.CreatePropertyValidator<string>(tenantID, typeof(AddressDto).FullName!, nameof(AddressDto.Postcode), ruleConfigs, validationFactoryProvider)(dataObject.Address.Postcode);
                    validations.Add(validatedPostcode);
                }

            }

            var invalidEntries = validations.Where(f => f.IsInvalid).SelectMany(f => f.Failures).ToList();

            return invalidEntries.Count > 0 ? Validated<ContactDto>.Invalid(invalidEntries) : Validated<ContactDto>.Valid(dataObject);

        }
    }
}
