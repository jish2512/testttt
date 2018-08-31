using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.PS.FXP.Core.Caching;
using Microsoft.PS.FXP.Core.Enums;
using Microsoft.PS.FXP.Core.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

namespace Microsoft.PS.FXP.Service.Api.App_Start
{
	/// <summary>
	/// AppBootstrapConfig initializes all the application configuration required to start with.
	/// </summary>
	public class AppBootstrapConfig
	{
		public static LoggingConfig LogginConfig { get; private set; }
		public static CacheConfig CacheConfig { get; private set; }
		public static RedisCacheConfig RedisCacheConfig { get; private set; }
		public static MultiRegionRedisConfig MultiRegionRedisConfig { get; private set; }
		public static string CustomSettingsInfoFile { get { return HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["SettingsStore:CustomSettingsInfoFile"]); } }
		public static string MaxSettingsPerRequest { get { return ConfigurationManager.AppSettings["Validation:MaxSettingsPerRequest"]; } }
		public static string MaxPersonalizationIdsPerRequest { get { return ConfigurationManager.AppSettings["Validation:MaxPersonalizationIdsPerRequest"]; } }

		private static string _configString;
		private static string _secondaryRedisEndpoints;
				
		public static void Configure()
		{
			InitializeTelemetryEnv();
			InitializeCachingEnv();
		}

		private static void InitializeCachingEnv()
		{
			_configString = ConfigurationManager.AppSettings["RedisCache-Endpoints"];
			_secondaryRedisEndpoints = ConfigurationManager.AppSettings["RedisCache-SecondaryEndpoints"];
			var cacheEnabled = bool.Parse(ConfigurationManager.AppSettings["EnableCache"]);
			CacheConfig = new CacheConfig(GetCacheDuration(), cacheEnabled);

			ConfigurationOptions configurationOptions = ConfigurationOptions.Parse(_configString);
			configurationOptions.ConnectTimeout =
				int.Parse(ConfigurationManager.AppSettings["RedisCache:ConnectTimeoutInMilliSeconds"]);
			configurationOptions.SyncTimeout = int.Parse(ConfigurationManager.AppSettings["RedisCache:SyncTimeoutInMilliSeconds"]);
			RedisCacheConfig = new RedisCacheConfig { ConfigurationOptions = configurationOptions };

			MultiRegionRedisConfig = new MultiRegionRedisConfig
			{
				PrimaryCacheConnection = _configString,
				SecondaryCacheConnections = _secondaryRedisEndpoints.Split(';'),
				SyncTimeout = configurationOptions.SyncTimeout,
				ConnectTimeout = configurationOptions.ConnectTimeout
			};
		}

		private static Dictionary<CacheDuration, long> GetCacheDuration()
		{
			var shortCacheDuration = long.Parse(ConfigurationManager.AppSettings["CacheExpiryDurationShortInMinutes"]);
			var mediumCacheDuration = long.Parse(ConfigurationManager.AppSettings["CacheExpiryDurationMediumInMinutes"]);
			var longCacheDuration = long.Parse(ConfigurationManager.AppSettings["CacheExpiryDurationLongInMinutes"]);
			var cacheDuration = new Dictionary<CacheDuration, long>
			{
				{CacheDuration.Off, 0},
				{CacheDuration.Short, shortCacheDuration},
				{CacheDuration.Medium, mediumCacheDuration},
				{CacheDuration.Long, longCacheDuration}
			};
			return cacheDuration;
		}

		public static void InitializeLoggingEnv()
		{
			TelemetryConfiguration.Active.InstrumentationKey = $"AIF-" + ConfigurationManager.AppSettings["Logging:InstrumentationKey"];
			TelemetryConfiguration.Active.TelemetryChannel.EndpointAddress = ConfigurationManager.AppSettings["Logging:EndPointUrl"];
			LogginConfig = new LoggingConfig
			{
				DiagnosticLevel =
					(DiagnosticLevel)
					Enum.Parse(typeof(DiagnosticLevel), ConfigurationManager.AppSettings["Logging:DiagnosticLevel"]),
				PerfMetricEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["Logging:PerfMetricEnabled"])
			};

		}

		private static void InitializeTelemetryEnv()
		{
			var userInitializer = new TelemetryUserDetailsInitializer();
			var businessProcessEventInitializer = new TelemetryBusinessProcessEventInitializer();
			var env = new Msit.Telemetry.Extensions.AI.EnvironmentInitializer();
			env.EnvironmentName = ConfigurationManager.AppSettings["TelemetryEnv:EnvironmentName"];
			env.ServiceOffering = ConfigurationManager.AppSettings["TelemetryEnv:ServiceOffering"];
			env.ServiceLine = ConfigurationManager.AppSettings["TelemetryEnv:ServiceLine"];
			env.Program = ConfigurationManager.AppSettings["TelemetryEnv:Program"];
			env.Capability = ConfigurationManager.AppSettings["TelemetryEnv:Capability"];
			env.ComponentName = ConfigurationManager.AppSettings["TelemetryEnv:ComponentName"];
			env.IctoId = ConfigurationManager.AppSettings["TelemetryEnv:IctoId"];
			TelemetryConfiguration.Active.TelemetryInitializers.Add((env));
			TelemetryConfiguration.Active.TelemetryInitializers.Add((userInitializer));
			TelemetryConfiguration.Active.TelemetryInitializers.Add((businessProcessEventInitializer));
		}
	}
}