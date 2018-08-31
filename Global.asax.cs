using Microsoft.PS.FXP.Service.Api.App_Start;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.PS.FXP.KeyVaultHelper;
using System.Web.Routing;
using System;
using Microsoft.PS.FXP.Core.Logging;

namespace Microsoft.PS.FXP.Service.Api
{
	[ExcludeFromCodeCoverage]
	public class WebApiApplication : System.Web.HttpApplication
	{

		private IKeyVaultConfigurationManager _keyVaultConfigurationManager;
		protected void Application_Start()
		{
			AppBootstrapConfig.InitializeLoggingEnv();
			InitializeAzureKeyVaults();
			AppBootstrapConfig.Configure();
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
		}
		private void InitializeAzureKeyVaults()
		{
			AppInsightsLoggingProvider appInsightsLoggingProvider = new AppInsightsLoggingProvider();
			DefaultLogger logger = new DefaultLogger(appInsightsLoggingProvider, AppBootstrapConfig.LogginConfig);
			try
			{
				_keyVaultConfigurationManager = new KeyVaultConfigurationManager(logger, ConfigurationManager.AppSettings["KeyVaultUrl"]);
				_keyVaultConfigurationManager.InitializeConfiguration();
			}
			catch (Exception azureKeyVaultException)
			{
				logger.LogError(new Exception("Failed to fetch keys from vaults for Fxp Services ", azureKeyVaultException), true);
			}
		}
	}
}
