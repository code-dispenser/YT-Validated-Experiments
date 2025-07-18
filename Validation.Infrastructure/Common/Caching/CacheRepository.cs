using System.Collections.Immutable;
using Validation.Application.Common.Seeds;
using Validation.Core.Common.Constants;
using Validation.Core.Types;

namespace Validation.Infrastructure.Common.Caching;

public class CacheRepository(CacheProvider cacheProvider) : ICacheRepository
{
    private readonly CacheProvider _cacheProvider = cacheProvider;

    /*
        * You would wrap all this stuff in a Result type. 
    */
    public async Task<ImmutableList<ValidationRuleConfig>> GetAllTenantConfigurations(string itemKey)

        => await _cacheProvider.GetOrCreate(BuildConfigurations, itemKey, 5);// << you would probably use some global constants for keys and cache times, etc.

    /*
        * The method below in reality would use something like an injected dbcontext / data access code to go get the configurations periodically
        * Method gets called when the cache is empty or expired.  
    */
    internal Task<ImmutableList<ValidationRuleConfig>> BuildConfigurations()

        => Task.FromResult(ImmutableList.Create<ValidationRuleConfig>
            (
                new("Validation.Domain.ValueObjects.FullName", "Title", "Title", RuleType.Regex, MinMaxValueType.String, @"^(Mr|Mrs|Ms|Dr|Prof)$", "Must be one of Mr, Mrs, Ms, Dr, Prof", 2, 4),
                new("Validation.Domain.ValueObjects.FullName", "GivenName", "First name", RuleType.Regex, MinMaxValueType.String, @"^(?=.{2,50}$)[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]+$", "Must start with a capital letter and be between 2 and 50 characters in length", 2, 50),
                new("Validation.Domain.ValueObjects.FullName", "FamilyName", "Surname", RuleType.Regex, MinMaxValueType.String, @"^[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]*$", "Must start with a capital letter (TenantOne)", 2, 50, "", "", "TenantOne"),
                new("Validation.Domain.ValueObjects.FullName", "FamilyName", "Surname", RuleType.Regex, MinMaxValueType.String, @"^[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]*$", "Must start with a capital letter", 2, 50),
                new("Validation.Domain.ValueObjects.FullName", "FamilyName", "Surname", RuleType.Length, MinMaxValueType.String, "", "Must be between 2 and 50 characters long", 2, 50),

                new("Validation.Contracts.DTOs.ContactDto", "Title", "Title", RuleType.Regex, MinMaxValueType.String, @"^(Mr|Mrs|Ms|Dr|Prof)$", "Must be one of Mr, Mrs, Ms, Dr, Prof", 2, 4),
                new("Validation.Contracts.DTOs.ContactDto", "GivenName", "First name", RuleType.Regex, MinMaxValueType.String, @"^(?=.{2,50}$)[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]+$", "Must start with a capital letter and be between 2 and 50 characters in length", 2, 50),
                new("Validation.Contracts.DTOs.ContactDto", "FamilyName", "Surname", RuleType.Regex, MinMaxValueType.String, @"^[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]*$", "Must start with a capital letter (TenantOne)", 2, 50, "", "", "TenantOne"),
                new("Validation.Contracts.DTOs.ContactDto", "FamilyName", "Surname", RuleType.Regex, MinMaxValueType.String, @"^[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]*$", "Must start with a capital letter", 2, 50),
                new("Validation.Contracts.DTOs.ContactDto", "FamilyName", "Surname", RuleType.Length, MinMaxValueType.String, "", "Must be between 2 and 50 characters long", 2, 50),
                new("Validation.Contracts.DTOs.ContactDto", "Email", "Email", RuleType.Regex, MinMaxValueType.String, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", "Must be a valid email format", 4, 75),
                new("Validation.Contracts.DTOs.ContactDto", "DOB", "Date of birth", RuleType.RollingDate, MinMaxValueType.Int32, "", "Date of birth must be between @MINDATE and @MAXDATE", 10, 10, "-122", "-18"),
                new("Validation.Contracts.DTOs.ContactDto", "Mobile", "Mobile Number", RuleType.Regex, MinMaxValueType.String, @"^(?:\+[1-9]\d{1,3}[ -]?7\d{9}|07\d{9})$", "Must be a valid UK mobile number format", 11, 16, "", ""),

                new("Validation.Contracts.DTOs.AddressDto", "AddressLine", "Address Line", RuleType.Regex, MinMaxValueType.String, @"^(?=.{5,250}$)(?!.* {2})(?!.*[,\-']{2})[A-Za-z0-9][A-Za-z0-9 ,\-\n']+[A-Za-z0-9]$", "Must start with a letter or number and be 5 to 250 characters in length.", 5, 250),
                new("Validation.Contracts.DTOs.AddressDto", "TownCity", "Town / City", RuleType.Regex, MinMaxValueType.String, @"^(?=.{3,100}$)[A-Z](?!.* {2})(?!.*'{2})(?!.*-{2})[\-A-Za-z ']+[a-z]+$", "Must start with a capital letter and be between 3 to 100 characters in length.", 3, 100),
                new("Validation.Contracts.DTOs.AddressDto", "County", "County", RuleType.Regex, MinMaxValueType.String, @"^(?=.{3,100}$)[A-Z](?!.* {2})(?!.*'{2})(?!.*-{2})[\-A-Za-z ']+[a-z]+$", "Must start with a capital letter and be between 3 to 100 characters in length.", 3, 100),
                new("Validation.Contracts.DTOs.AddressDto", "Postcode", "Postcode", RuleType.Regex, MinMaxValueType.String, @"^(GIR 0AA)|((([ABCDEFGHIJKLMNOPRSTUWYZ][0-9][0-9]?)|(([ABCDEFGHIJKLMNOPRSTUWYZ][ABCDEFGHKLMNOPQRSTUVWXY][0-9][0-9]?)|(([ABCDEFGHIJKLMNOPRSTUWYZ][0-9][ABCDEFGHJKSTUW])|([ABCDEFGHIJKLMNOPRSTUWYZ][ABCDEFGHKLMNOPQRSTUVWXY][0-9][ABEHMNPRVWXY])))) [0-9][ABDEFGHJLNPQRSTUWXYZ]{2})$",
                                                            "Must be a valid UK formatted postcode.", 5, 15)
            ));




}
