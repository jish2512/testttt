using Microsoft.PS.FXP.Core.Enums;
using Microsoft.PS.FXP.Core.Logging;
using Microsoft.PS.FXP.Core.Services;
using Microsoft.PS.FXP.Service.Api.Constants;
using Microsoft.PS.FXP.Service.Api.Models;
using Microsoft.PS.FXP.Service.Api.Models.LeftNav;
using Microsoft.PS.FXP.Service.Api.Security;
using Microsoft.PS.FXP.Service.Api.Validators;
using Microsoft.PS.FXP.Services;
using Microsoft.PS.FXP.Services.DashboardEnums;
using Microsoft.PS.FXP.Services.DashboardServiceTypes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Microsoft.PS.FXP.Service.Api.Controllers
{
    /// <summary>
    /// Controller for dashboard
    /// </summary>
    [RoutePrefix("api/v1")]
    public class AdminController : ApiController
    {
        /// <summary>
        /// dashboard service object
        /// </summary>
        protected readonly IAdminService _adminService;

        /// <summary>
        /// Logger service object
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Constructor  
        /// </summary>
        /// <param name="logger"></param>
        public AdminController(ILogger logger, IAdminService adminService)
        {
            _adminService = adminService;
            _logger = logger;
        }

        /// <summary>
        /// Gets Admin tiles
        /// </summary>
        /// <param name="settingType">Type of Setting (User/UserGroup/Application)</param>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>ServiceResponse of type LeftNavigation</returns>
        [HttpGet]
        [Route("adminTiles")]
        [AuthorizeStore]
        [ResponseType(typeof(List<AdminTileGroup>))]
        public async Task<IHttpActionResult> GetUserLeftNavigationAdminAsync()
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("X-UserClaimsToken");
            var requestHeader = headerValues.FirstOrDefault();
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.GetAdminDataRequestFailed);
                return BadRequest(ModelState);
            }
                 var result = await _adminService.GetAdminTilesAsync(requestHeader);

            if (result != null)
            {
                _logger.LogInfo(ServiceMessages.GetAdminDataSuccess);
                return Ok(result);
            }
            else
            {
                _logger.LogInfo(ServiceMessages.NoDataFound);
                return NotFound();
            }
        }

        
  
    }
}