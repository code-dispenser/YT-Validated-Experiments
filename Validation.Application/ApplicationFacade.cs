using Validation.Application.Validation;
using Validation.Domain.ValueObjects;

namespace Validation.Application;

public class ApplicationFacade(ValueObjectService valueObjectService)
{
    private readonly ValueObjectService _valueObjectService = valueObjectService;
    public async Task<string> CreateFullNameUsingConfig(string title, string givenName, string familyName, string tenantID = "All")
    
        => (await _valueObjectService.CreateFullName(title, givenName, familyName, tenantID))
            .Match(failure => String.Join(Environment.NewLine, failure), success => success.ToString());

    public string CreateFullName(string title, string givenName, string familyName)
    
        => FullName.Create(title, givenName, familyName).Match(failure => String.Join(Environment.NewLine, failure), success => success.ToString());

}
