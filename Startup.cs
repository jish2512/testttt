using Microsoft.Owin;
using Owin;
using System.Diagnostics.CodeAnalysis;

[assembly: OwinStartup(typeof(Microsoft.PS.FXP.Service.Api.Startup))]

namespace Microsoft.PS.FXP.Service.Api
{
    /// <summary>
    /// Owin Startup 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
