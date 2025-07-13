using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace DAYA.Cloud.Framework.V2.Authentication.Contracts
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class DayaAuthorizeAttribute : AuthorizeAttribute
    {
        public DayaAuthorizeAttribute([Required] string policy, string? permission = null) : base(policy)
        {
            Permission = permission;
        }

        public string? Permission { get; }
    }
}