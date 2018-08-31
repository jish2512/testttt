using Msit.Telemetry.Extensions.Web;
using Newtonsoft.Json.Serialization;
using System.Configuration;
using System.Web.Http;
using Unity.WebApi;

namespace Microsoft.PS.FXP.Service.Api
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());

            config.MessageHandlers.Add(new WebApiCorrelationHandler());

            var origins = ConfigurationManager.AppSettings["CorsOrigins"];
            var cors = new System.Web.Http.Cors.EnableCorsAttribute(origins, "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
