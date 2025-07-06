using System.Reflection;
using System.Text;
using DAYA.Cloud.Framework.V2.Authentication.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAYA.Cloud.Framework.V2.Authentication.Tools
{
    public class AuthorizationCheck
    {
        public static void EnsureAllEndpointsAreSecured(Assembly assembly)
        {
            var invalidEndpoints = FindUnsecuredEndpoints(assembly);

            EnsureUnsecuredEndpointsAreEmpty(invalidEndpoints);
        }

        private static List<string> FindUnsecuredEndpoints(Assembly assembly)
        {
	        var controllers = assembly
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(ControllerBase)));
            var invalidEndpoints = new List<string>();
            foreach (var controller in controllers)
            {
                var endpoints = controller.GetMethods()
                    .Where(x => x.IsPublic && x.DeclaringType == controller)
                    .ToList();

                foreach (var endpoint in endpoints)
                {
                    var attributesCount = endpoint.GetCustomAttributes<DayaAuthorizeAttribute>().Count();
                    attributesCount += endpoint.GetCustomAttributes<AllowAnonymousAttribute>().Count();
                    if (attributesCount != 1)
                    {
                        invalidEndpoints.Add($"{controller.Name}.{endpoint.Name}");
                    }
                }
            }

            return invalidEndpoints;
        }

        private static void EnsureUnsecuredEndpointsAreEmpty(List<string> invalidEndpoints)
        {
            if (invalidEndpoints.Any())
            {
                var errorBuilder = new StringBuilder(Environment.NewLine + "Invalid permission configuration:" + Environment.NewLine);

                errorBuilder.AppendLine(string.Join(Environment.NewLine, invalidEndpoints
                    .Select(x => $"{x}")));

                throw new ApplicationException(errorBuilder.ToString());
            }
        }
    }
}