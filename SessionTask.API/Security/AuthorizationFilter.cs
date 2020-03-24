using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SessionTask.Models;

namespace SessionTask.API.Security
{
    public class AuthorizationFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _feature;
        private readonly string _permissions;

        public AuthorizationFilter(string feature, string permissions)
        {
            _feature = feature;
            _permissions = permissions;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (string.IsNullOrEmpty(_feature))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var requiredPermissions = _permissions.Split(",");
            var permissions = context.HttpContext.User.Claims.Where(x => x.Type == "Features").Select(x => x.Value).ToList();
            if (permissions.Count == 1)
            {
                var featurePermissions = JsonConvert.DeserializeObject<List<FeaturePermissionDto>>(permissions[0]);
                if (featurePermissions.Any(x => x.FeatureName == _feature && requiredPermissions.Contains(x.Permission)))
                    return;
                context.Result = new UnauthorizedResult();
                return;
            }
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}
