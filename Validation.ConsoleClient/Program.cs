using Microsoft.Extensions.DependencyInjection;

namespace Validation.ConsoleClient;
internal class Program
{
    private static readonly Uri _hostingServer = new Uri("https://localhost:44395"); // <<<< Choose your poison, this is for IIS Express see launch settings on web server .
    
    //private static readonly Uri _hostingServer = new Uri("https://localhost:7259"); //  <<<< kestrel
    
    private static readonly string _validationRoute = "api/validation";

    static async Task Main()
    {
        var httpClientFactory = ConfigureServiceProvider(_hostingServer);

        var httpClient = httpClientFactory.CreateClient();

        string title = "Mr", givenName = "John", familyName = "Doe", tenantID = "";

        Console.WriteLine("Creating a FullName value object using cached config entries to build validators semi-dynamically (tenant specific with fallback) - no reflection\r\n");
        Console.WriteLine("Try creating a FullName with good data, uses the default fallback tenantID: All");
        Console.WriteLine(await httpClient.GetStringAsync($"{_validationRoute}/TestCreateFullNameFromConfig?title={title}&givenName={givenName}&familyName={familyName}&tenantID={tenantID}") + "\r\n");

        Console.WriteLine("Try creating a FullName with bad data, uses the default fallback tenantID: All");
        Console.WriteLine(await httpClient.GetStringAsync($"{_validationRoute}/TestCreateFullNameFromConfig?title={title}&givenName={"j"}&familyName={familyName}&tenantID={tenantID}") + "\r\n");

        Console.WriteLine("Try creating a FullName with bad data, uses the tenantID TenantOne for half of the family name composed rule, the other half uses the fallback:");
        Console.WriteLine(await httpClient.GetStringAsync($"{_validationRoute}/TestCreateFullNameFromConfig?title={title}&givenName={givenName}&familyName={"d"}&tenantID={"TenantOne"}") + "\r\n");

        Console.WriteLine("Creating a FullName value object with good data using regular methods to create the validators - no tenant or dynamic creation");
        Console.WriteLine(await httpClient.GetStringAsync($"{_validationRoute}/TestCreateFullName?title={title}&givenName={givenName}&familyName={familyName}"));

    }

    private static IHttpClientFactory ConfigureServiceProvider(Uri hostUri)
    {
        var services = new ServiceCollection();
        services.AddHttpClient("", c => c.BaseAddress = hostUri);
        var provider = services.BuildServiceProvider();

       return provider.GetRequiredService<IHttpClientFactory>();
    }
       //=> Host.CreateApplicationBuilder()
       //         .Services
       //         .AddHttpClient("", client => client.BaseAddress = hostUri)
       //         .Services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();

                      
                         
}
