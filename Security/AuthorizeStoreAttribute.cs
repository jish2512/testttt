using Microsoft.Practices.Unity;
using Microsoft.PS.FXP.Core.Security;
using Microsoft.PS.FXP.Service.Api.Constants;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.PS.FXP.Service.Api.Security
{
    /// <summary>
    /// AuthorizeStoreAttribute
    /// </summary>
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeStoreAttribute : AuthorizeAttribute
    {
        private string _resourceName;
        private string _operation;
        private bool _checkResourcePermission;
        private bool _canAccessOwnResource;
        private IUserClaimsService _userClaimsService;
        private IAuthorizationService _authService;

        public AuthorizeStoreAttribute(bool canAccessOwnResource = false)
        {
            var contianer = UnityConfig.GetConfiguredContainer();
            _userClaimsService = contianer.Resolve<IUserClaimsService>();
            _authService = contianer.Resolve<IAuthorizationService>();
            _canAccessOwnResource = canAccessOwnResource;
        }

        public AuthorizeStoreAttribute(string resourceName, string operation, bool canAccessOwnResource = false) : this(canAccessOwnResource)
        {
            if (string.IsNullOrWhiteSpace(resourceName) || string.IsNullOrWhiteSpace(operation))
                throw new ArgumentNullException("Resource name or operation cannot be empty");

            _resourceName = resourceName;
            _operation = operation;
            _checkResourcePermission = true;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (!_userClaimsService.IsAuthenticated)
                return false;

            if (!_checkResourcePermission)
                return true;

            if (_canAccessOwnResource)
            {
                var routeData = actionContext.Request.GetRouteData();

                if (routeData.Values.ContainsKey(ApiConstants.UserAlias))
                {
                    var calledForUser = Convert.ToString(routeData.Values[ApiConstants.UserAlias]);
                    if (calledForUser.Equals(_userClaimsService.CurrentUserAlias, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return _authService.HasPersmission(_resourceName, _operation);
        }
    }
}