using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DAYA.Cloud.Framework.V2.Infrastructure.ContextAccessors
{
    /// <summary>
    /// Implementation of IUserContextAccessor that uses the HttpContext
    /// </summary>
    internal class HttpContextUserAccessor : IUserContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name).Value;
        }
    }
}