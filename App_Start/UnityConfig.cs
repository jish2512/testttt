using Microsoft.Practices.Unity;
using Microsoft.PS.FXP.Core.Caching;
using Microsoft.PS.FXP.Core.Logging;
using Microsoft.PS.FXP.Core.Repositories;
using Microsoft.PS.FXP.Core.Repository;
using Microsoft.PS.FXP.Core.Security;
using Microsoft.PS.FXP.Core.Services;
using Microsoft.PS.FXP.Core.StoreAccess;
using Microsoft.PS.FXP.QueueMessageService;
using Microsoft.PS.FXP.Service.Api.App_Start;
using Microsoft.PS.FXP.Service.Api.Validators;
using Microsoft.PS.FXP.Services;
using Microsoft.PS.FXP.Services.Repository;
using Microsoft.PS.FXP.Settings.Repository;
using Microsoft.PS.FXP.StoreAccess;
using Msit.Telemetry.Extensions;
using StackExchange.Redis;
using System;
using System.Configuration;

namespace Microsoft.PS.FXP.Service.Api
{
    public static class UnityConfig
    {
        private static readonly Lazy<IUnityContainer> _container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterComponents(container);

            return container;
        });

        private static void RegisterComponents(IUnityContainer container)
        {
            container.RegisterLogger();
            container.RegisterCaching();
            container.RegisterCorelationProvider();

            container.RegisterType<IStoreAccess, DocumentDbStoreAccess>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUserClaimsPrincipal, UserClaimsPrincipal>();
            container.RegisterType<IUserClaimsService, UserClaimsService>();
            container.RegisterType<IAuthorizationService, AuthorizationService>();
            container.RegisterType<ICustomSettingsManager, CustomSettingsManager>();
            container.RegisterType<ICustomSettingsResolver, CustomSettingsResolver>(new InjectionConstructor(AppBootstrapConfig.CustomSettingsInfoFile ?? ""));
            container.RegisterType<ISettingsRepository, SettingsRepository>();
            container.RegisterType<ISettingsService, SettingsService>();
            container.RegisterType<ILeftNavHelper, LeftNavHelper>();
            container.RegisterType<ILeftNavProcessor, LeftNavProcessor>();
            container.RegisterType<IAdminService, AdminService>();
            container.RegisterType<IDashboardService, DashboardService>();
            container.RegisterType<ISettingsValidator, SettingsValidator>(new InjectionConstructor(AppBootstrapConfig.MaxSettingsPerRequest));
            container.RegisterType<IDashboardValidator, DashboardValidator>(new InjectionConstructor(AppBootstrapConfig.MaxPersonalizationIdsPerRequest));
            container.RegisterType<ISystemMessagesService, SystemMessagesService>();
            container.RegisterType<ISystemMessagesRepository, SystemMessagesRepository>();
            container.RegisterType<ISystemMessagesValidator, SystemMessagesValidator>();
            container.RegisterType<IFxpMessageQueue, FxpMessageQueue>();
            container.RegisterType<IFxpMessageQueueFactory, FxpMessageQueueFactory>();
            container.RegisterType<IStorageQueueRepository, StorageQueueRepository>();
            container.RegisterType<INotificationValidator, NotificationValidator>();
            container.RegisterInstance<IUnityContainer>(container);
        }
		private static void Howdoyodo() {
		}
        private static void RegisterCorelationProvider(this IUnityContainer container)
        {
            var correlationIdHeader = ConfigurationManager.AppSettings["CorrelationIdHeader"];
            var webCorrelationProvider = new WebCorrelationProvider(correlationIdHeader);
            container.RegisterInstance<ICorrelationProvider>(webCorrelationProvider);
        }
        private static void RegisterLogger(this IUnityContainer container)
        {
            container.RegisterType<ILoggingProvider, AppInsightsLoggingProvider>();
            container.RegisterInstance(AppBootstrapConfig.LogginConfig);
            container.RegisterType<ILogger, DefaultLogger>();
        }

        private static void RegisterCaching(this IUnityContainer container)
        {
            container.RegisterType<IMemoryCache, InMemoryCache>();
            container.RegisterInstance(AppBootstrapConfig.CacheConfig);
            container.RegisterInstance(AppBootstrapConfig.MultiRegionRedisConfig);
            container.RegisterInstance(AppBootstrapConfig.RedisCacheConfig);
            container.RegisterType<ICacheProvider, MultiRegionRedisCacheProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICacheService, CacheService>(new ContainerControlledLifetimeManager());
            container.RegisterInstance<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(AppBootstrapConfig.RedisCacheConfig.ConfigurationOptions));
        }

        public static IUnityContainer GetConfiguredContainer()
        {
            return _container.Value;
        }
    }
}