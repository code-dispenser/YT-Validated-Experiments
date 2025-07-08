using Microsoft.AspNetCore.Mvc;
using Validation.Application;
using Validation.Core.Types;
using Validation.Domain.ValueObjects;


namespace Validation.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValidationController : ControllerBase
{
    public readonly ApplicationFacade _application;
    public ValidationController(ApplicationFacade application) 
        
        => _application = application;

    /*
        * IMPORTANT:
        * 
        * You would never expose the TenantID and/or accept it. 
        * You get the TenantID / userID from a database and store it in cache and then in each users authenticated request you
        * can use the claims transformation process to get and store tenant information in the httpcontext etc

    */


    [HttpGet("TestCreateFullNameFromConfig")]
    public async Task<string> TestCreateFullNameFromConfig(string title, string givenName, string familyName, string tenantID = "")
     
        => await _application.CreateFullNameUsingConfig(title, givenName, familyName, tenantID);

    [HttpGet("TestCreateFullName")]
    public string TestCreateFullName(string title, string givenName, string familyName)

        =>  _application.CreateFullName(title, givenName, familyName);
}
