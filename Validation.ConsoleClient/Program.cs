using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text;
using Validation.Contracts;
using Validation.Contracts.DTOs;
using Validation.Core.Factories;
using Validation.Core.Types;
using Validation.Core.Validator;

namespace Validation.ConsoleClient;
internal class Program
{
    private static readonly Uri _hostingServer = new("https://localhost:44395"); // <<<< Choose your poison, this is for IIS Express see launch settings on web server .
    
    //private static readonly Uri _hostingServer = new ("https://localhost:7259"); //  <<<< kestrel
    
    private static readonly string _validationRoute = "api/validation";

    static async Task Main()
    {
        var httpClientFactory = ConfigureServiceProvider(_hostingServer);
        var httpClient        = httpClientFactory.CreateClient();

        Console.OutputEncoding = Encoding.UTF8;

        await PartOneClientCode(httpClient, _validationRoute);
        await PartTwoClientCode(httpClient, _validationRoute);

        Console.ReadLine();

    }

    private static async Task PartOneClientCode(HttpClient httpClient, string validationRoute)
    {
        string title = "Mr", givenName = "John", familyName = "Doe", tenantID = "";

        Console.WriteLine("Creating a FullName value object using cached config entries to build validators semi-dynamically (tenant specific with fallback) - no reflection\r\n");
        Console.WriteLine("Try creating a FullName with good data, uses the default fallback tenantID: All");
        Console.WriteLine(await httpClient.GetStringAsync($"{validationRoute}/TestCreateFullNameFromConfig?title={title}&givenName={givenName}&familyName={familyName}&tenantID={tenantID}") + "\r\n");

        Console.WriteLine("Try creating a FullName with bad data, uses the default fallback tenantID: All");
        Console.WriteLine(await httpClient.GetStringAsync($"{validationRoute}/TestCreateFullNameFromConfig?title={title}&givenName={"j"}&familyName={familyName}&tenantID={tenantID}") + "\r\n");

        Console.WriteLine("Try creating a FullName with bad data, uses the tenantID TenantOne for half of the family name composed rule, the other half uses the fallback:");
        Console.WriteLine(await httpClient.GetStringAsync($"{validationRoute}/TestCreateFullNameFromConfig?title={title}&givenName={givenName}&familyName={"d"}&tenantID={"TenantOne"}") + "\r\n");

        Console.WriteLine("Creating a FullName value object with good data using regular methods to create the validators - no tenant or dynamic creation");
        Console.WriteLine(await httpClient.GetStringAsync($"{validationRoute}/TestCreateFullName?title={title}&givenName={givenName}&familyName={familyName}\r\n"));
    }
    private static async Task PartTwoClientCode(HttpClient httpClient, string validationRoute)
    {
        

        Console.WriteLine("Validating a good ContactDto with a populated optional (Address?) address, client side using object/methods contained in the shared projects Validation.Contracts and Validation.Core");
        Console.WriteLine("This can also be done on the server side as all the code is in the shared projects\r\n"); 

        ContactDto goodContact = new(){ Title = "Mr", GivenName  = "John", FamilyName = "Doe", DOB= new DateOnly(1990, 1, 1), Email = "john.doe@gmail.com", Mobile = "07123456789",                                        
                                    Address = new AddressDto() { AddressLine = "House on the street", TownCity = "London", County="Greater London", Postcode = "SW1A 0AA" }};

        Console.WriteLine(new ContactDtoValidator().Validate(goodContact).Match(failure => String.Join(Environment.NewLine, failure), success => success.ToString()) + "\r\n");

        Console.WriteLine("Please note the above email address. This check was not done using a validator created in the ContactDtoValidator instead of using the shared ones.\r\n");

        Console.WriteLine("Validating a bad ContactDto with a populated optional (Address?) address client side");

        ContactDto badContact = new(){ Title = "M", GivenName  = "John", FamilyName = "Doe", DOB= new DateOnly(1901, 1, 1), Email = "john.doe@hotmail.com", Mobile = "07123456789",                                        
                        Address = new AddressDto() { AddressLine = "House on the street", TownCity = "", County="Greater London", Postcode = "SW1A 0AA" }};

        Console.WriteLine(new ContactDtoValidator().Validate(badContact).Match(failure => String.Join(Environment.NewLine, failure), success => success.ToString()) + "\r\n");

        Console.WriteLine("Getting configuration data from the server cache for client side validations . . .\r\n");

        var allConfigurations = await httpClient.GetFromJsonAsync<ImmutableList<ValidationRuleConfig>>($"{_validationRoute}/GetTenantConfigurations?tenantID={"TenantOne"}");

        Console.WriteLine("Validators using the normal email regex not the gmail check\r\n");

        Console.WriteLine("Validating in a semi-dynamic way (no reflection) using the multi-tenant configs and a bad ContactDto WITHOUT a populated optional (Address?) address client side\r\n");

        badContact = new(){ Title = "M", GivenName  = "John", FamilyName = "Doe", DOB= new DateOnly(1901, 1, 1), Email = "john.doe@hotmail.com", Mobile = "07123456789"};

        var validatedBadContact = new ContactTenantDtoValidator().Validate("TenantOne", badContact, [.. allConfigurations!], new ValidationFactoryProvider());
        
        Console.WriteLine(validatedBadContact.Match(failure => String.Join(Environment.NewLine, failure), success => success.ToString()) + "\r\n");

        Console.WriteLine("Validating using a reflection based approach with a basic generic validator (good for any dto/command/query/message etc) " +
                          "using the multi-tenant configs and a bad ContactDto with a populated optional (Address?) address client side\r\n");

        badContact = new(){ Title = "M", GivenName  = "John", FamilyName = "Doe", DOB= new DateOnly(1901, 1, 1), Email = "john.doe@hotmail.com", Mobile = "07123456789",                                        
                        Address = new AddressDto() { AddressLine = "House on the street", TownCity = "", County="Greater London", Postcode = "SW1A 0AA" }};

        var genericValidatedBadContact  = GenericValidator.Validate(badContact, allConfigurations!, new(),"TenantOne");

        Console.WriteLine(genericValidatedBadContact.Match(failure => String.Join(Environment.NewLine, failure), success => success.ToString()) + "\r\n");
    }



    private static IHttpClientFactory ConfigureServiceProvider(Uri hostUri)
    {
        var services = new ServiceCollection();
        services.AddHttpClient("", c => c.BaseAddress = hostUri);
        var provider = services.BuildServiceProvider();

       return provider.GetRequiredService<IHttpClientFactory>();
    }
                    
                         
}
