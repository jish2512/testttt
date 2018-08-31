using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Practices.Unity;
using Microsoft.PS.FXP.Core.Security;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.PS.FXP.Service.Api.App_Start
{
    [ExcludeFromCodeCoverage]
    public class TelemetryUserDetailsInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry != null && telemetry.Context != null)
            {
                var container = UnityConfig.GetConfiguredContainer();
                var userClaimsService = container.Resolve<IUserClaimsService>();
                telemetry.Context.User.AuthenticatedUserId = userClaimsService.CurrentUserUpn;
            }
        }
    }
}