using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using Validation.Application;
using Validation.Application.Common.Seeds;
using Validation.Core.Common.Constants;
using Validation.Core.Types;


namespace Validation.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValidationController : ControllerBase
{
    public readonly ApplicationFacade _application;
    public readonly ICacheRepository _cacheRepository;
    public ValidationController(ApplicationFacade application, ICacheRepository cacheRepository)

        => (_application, _cacheRepository) = (application, cacheRepository);

    /*
        * IMPORTANT:
        * 
        * You would never expose the TenantID and/or accept it if possible. 
        * You get the TenantID / userID from a database and store it in cache and then in each users authenticated request you
        * can use the claims transformation process to get and store tenant information in the httpcontext etc

    */


    [HttpGet("TestCreateFullNameFromConfig")]
    public async Task<string> TestCreateFullNameFromConfig(string title, string givenName, string familyName, string tenantID = GlobalValues.Default_TenantID)

        => await _application.CreateFullNameUsingConfig(title, givenName, familyName, tenantID);

    [HttpGet("TestCreateFullName")]
    public string TestCreateFullName(string title, string givenName, string familyName)

        => _application.CreateFullName(title, givenName, familyName);


    [HttpGet("GetTenantConfigurations")]
    public async Task<ImmutableList<ValidationRuleConfig>> GetTenantConfigurations(string tenantID = GlobalValues.Default_TenantID)

        => await _cacheRepository.GetAllTenantConfigurations(GlobalValues.All_Tenant_Validations_Configs_Cache_Key);
}