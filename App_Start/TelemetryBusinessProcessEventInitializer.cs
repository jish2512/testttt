using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Configuration;
using System.Linq;

namespace Microsoft.PS.FXP.Service.Api.App_Start
{
	[ExcludeFromCodeCoverage]
	public class TelemetryBusinessProcessEventInitializer : ITelemetryInitializer
	{
		public void Initialize(ITelemetry telemetry)
		{
			if (telemetry != null && telemetry.Context != null)
			{
				var senderId = ClaimsPrincipal.Current.Claims.Where(c => c.Type.Equals("appid")).Select(c => c.Value).SingleOrDefault();
				if (senderId != null)
					telemetry.Context.Properties["SenderID"] = senderId;
				telemetry.Context.Properties["BusinessProcessName"] = ConfigurationManager.AppSettings["TelemetryBusinessProcessEvent:BusinessProcessName"];
			}
		}
	}
}