using Daya.Sample.Application.UserAccounts.Commands.Create;
using Daya.Sample.Domain.UserAccounts;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Daya.Sample.API.Controllers.UserAccounts
{
    [ApiController]
    [Route("api/platform/[controller]")]
    [ApiExplorerSettings(GroupName = SwaggerUiExtension.PublicApiGroup)]
    public class UserAccountsController : ControllerBase
    {
        private readonly IServiceModule _serviceModule;

        public UserAccountsController(IServiceModule serviceModule)
        {
            _serviceModule = serviceModule;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), statusCode: 200)]
        public async Task<IActionResult> CreateUserAccount([FromBody] CreateUserAccountRequest request)
        {
            var userAccountId = await _serviceModule.ExecuteCommandAsync(
                new CreateUserAccountCommand(
                    new UserAccountId(Guid.NewGuid()),
                    new TenantId(Guid.Parse(DefaultValues.PlatformPartitionKey)),
                    request.FirstName,
                    request.LastName,
                    request.EmailAddress));

            return Ok(userAccountId);
        }
    }
}