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
                new("Validation.Domain.ValueObjects.FullName", "Title",      "Title",      RuleType.Regex,  MinMaxValueType.String, @"^(Mr|Mrs|Ms|Dr|Prof)$", "Must be one of Mr, Mrs, Ms, Dr, Prof", 2, 4),
                new("Validation.Domain.ValueObjects.FullName", "GivenName",  "First name", RuleType.Regex,  MinMaxValueType.String, @"^(?=.{2,50}$)[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]+$", "Must start with a capital letter and be between 2 and 50 characters in length", 2, 50),
                new("Validation.Domain.ValueObjects.FullName", "FamilyName", "Surname",    RuleType.Regex,  MinMaxValueType.String, @"^[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]*$", "Must start with a capital letter (TenantOne)", 2, 50, false, "", "", "TenantOne"),
                new("Validation.Domain.ValueObjects.FullName", "FamilyName", "Surname",    RuleType.Regex,  MinMaxValueType.String, @"^[A-Z]+['\- ]?[A-Za-z]*['\- ]?[A-Za-z]*$", "Must start with a capital letter", 2, 50),
                new("Validation.Domain.ValueObjects.FullName", "FamilyName", "Surname",    RuleType.Length, MinMaxValueType.String, "", "Must be between 2 and 50 characters long", 2, 50)));

}
