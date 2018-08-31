using Microsoft.IT.Ps.Authorization.Common;
using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Microsoft.PS.FXP.Service.Api.Constants;
using Owin;
using System;
using System.Configuration;


[assembly: OwinStartup(typeof(Microsoft.PS.FXP.Service.Api.Startup))]

namespace Microsoft.PS.FXP.Service.Api
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            bool useAADAuth = Convert.ToBoolean(ConfigurationManager.AppSettings["UseAADAuthorizationService"]);

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidAudience = ConfigurationManager.AppSettings["ida:Audience"]
                    },
                    Tenant = ConfigurationManager.AppSettings["ida:Tenant"]
                });

            app.UseItAuthClaimsAugmentation(new ItAuthClaimsAugmentationOptions()
            {
                ItAuthresourceUri = useAADAuth ? ConfigurationManager.AppSettings["ItAuthResourceUriAADService"] : ConfigurationManager.AppSettings["ItAuthResourceUri"],
                ItAuthClientId = ConfigurationManager.AppSettings["ItAuthClientId"],
                ItAuthClientSecret = ConfigurationManager.AppSettings["ItAuthClientSecret"],
                ItAUthBusinessDomainName = ConfigurationManager.AppSettings["ItAUthBusinessDomainName"],
                AuthBusinessDomainClientId = Guid.Parse(ConfigurationManager.AppSettings["AuthBusinessDomainClientId"]),
                AdminRoleName = ConfigurationManager.AppSettings["AdminRoleName"],
                CacheEnabled = bool.Parse(ConfigurationManager.AppSettings["EnableClaimsCache"]),
                ExpirationMinutes = long.Parse(ConfigurationManager.AppSettings["CacheExpiryMinutes"]),
                ItaClaimServiceUrl = ConfigurationManager.AppSettings["ItaClaimServiceUrl"],
                EnableServiceToServiceAuthorization = true,
                UseAADAuthorizationService = useAADAuth
            });

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableHttpTransactionTimeLogging"]))
            {
                //Configuring CustomHeaders value
                app.Use(async (context, next) =>
                {
                    var origins = ConfigurationManager.AppSettings["CorsOrigins"];
                    var originsList = origins.Split(',');
                    context.Response.Headers.Add(CustomHeaders.TimingAllowOrigin, originsList);
                    await next();
                });
            }
        }
    }
}