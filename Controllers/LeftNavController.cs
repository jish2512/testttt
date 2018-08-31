using Microsoft.PS.FXP.Core.Enums;
using Microsoft.PS.FXP.Core.Logging;
using Microsoft.PS.FXP.Core.Services;
using Microsoft.PS.FXP.Service.Api.Constants;
using Microsoft.PS.FXP.Service.Api.Models.LeftNav;
using Microsoft.PS.FXP.Service.Api.Security;
using Microsoft.PS.FXP.Service.Api.Validators;
using Microsoft.PS.FXP.Services;
using Microsoft.PS.FXP.Services.DashboardEnums;
using Microsoft.PS.FXP.Services.DashboardServiceTypes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Microsoft.PS.FXP.Service.Api.Controllers
{
    /// <summary>
    /// Controller for dashboard
    /// </summary>
    [RoutePrefix("api/v1")]
    public class LeftNavController : ApiController
    {
        /// <summary>
        /// dashboard service object
        /// </summary>
        protected readonly IDashboardService _dashboardService;

        /// <summary>
        /// Logger service object
        /// </summary>
        protected readonly ILogger _logger;

        protected readonly IDashboardValidator _validator;

        /// <summary>
        /// Constructor for dashboard
        /// </summary>
        /// <param name="dashboardService"></param>
        /// <param name="logger"></param>
        public LeftNavController(IDashboardService dashboardService, IDashboardValidator validator, ILogger logger)
        {
            _dashboardService = dashboardService;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Gets Dashboard left navigation items
        /// </summary>
        /// <param name="settingType">Type of Setting (User/UserGroup/Application)</param>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>ServiceResponse of type LeftNavigation</returns>
        [HttpGet]
        [Route("user/{userAlias}/leftNav")]
        [AuthorizeStore(ApiResource.LeftNav, ApiOperation.Read, true)]
        [ResponseType(typeof(LeftNavigationResponse))]
        public async Task<IHttpActionResult> GetUserLeftNavigationAsync([FromUri]UserLeftNavRequest request)
        {
            LogGetLeftNavTraceInfo(request.UserAlias, ResourceType.User, ServiceMessages.GetUserLeftNavRequest);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.GetUserLeftNavFailed);
                return BadRequest(ModelState);
            }
            var result = await _dashboardService.GetUserLeftNavigationAsync(request.RoleGroupId,
                                    request.UserRoleId, request.UserAlias);

            if (result != null)
            {
                _logger.LogInfo(ServiceMessages.GetUserNavSuccess);
                return Ok(result);
            }
            else
            {
                _logger.LogInfo(ServiceMessages.NoDataFound);
                return NotFound();
            }
        }

        /// <summary>
        /// Get User Personalization Navigation items
        /// </summary>
        /// <param name="userAlias">userAlias</param>
        /// <param name="roleGroupId">roleGroupId</param>
        /// <returns>ServiceResponse of type PersonalizationNavResponse</returns>
        [HttpGet]
        [Route("user/{userAlias}/leftNav/personalization")]
        [AuthorizeStore(ApiResource.LeftNav, ApiOperation.Read)]
        [ResponseType(typeof(ServiceResponse<List<LeftNavigationItem>>))]
        public async Task<IHttpActionResult> GetUserPersonalizationNavigationAsync([FromUri]UserLeftNavRequest request)
        {
            LogGetLeftNavTraceInfo(request.RoleGroupId, ResourceType.Persona, ServiceMessages.GetUserPersonalizationNavigationRequest);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.GetUserPersonalizationFailed);
                return BadRequest(ModelState);
            }
            var result = await _dashboardService.GetPersonalizationNavigationAsync(request.RoleGroupId, request.UserRoleId, request.UserAlias);
            if (result != null)
            {
                _logger.LogInfo(ServiceMessages.GetUserPersonalizationNavigation);
                return Ok(result);
            }
            else
            {
                _logger.LogInfo(ServiceMessages.GetUserPersonalizationFailed);
                return NotFound();
            }
        }

        /// <summary>
        /// Get User Personalization Navigation items
        /// </summary>
        /// <param name="roleGroupId">roleGroupId</param>
        /// <returns>ServiceResponse of type PersonalizationNavResponse</returns>
        [HttpGet]
        [Route("userRole/{userRoleId}/leftNav/personalization")]
        [AuthorizeStore(ApiResource.LeftNav, ApiOperation.Read)]
        [ResponseType(typeof(ServiceResponse<List<LeftNavigationItem>>))]
        public async Task<IHttpActionResult> GetUserRolePersonalizationNavigationAsync([FromUri] UserRoleLeftNavRequest request)
        {
            LogGetLeftNavTraceInfo(request.RoleGroupId, ResourceType.Persona, ServiceMessages.GetRoleGroupPersonalizationNavigationRequest);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.GetUserPersonalizationFailed);
                return BadRequest(ModelState);
            }
            var result = await _dashboardService.GetPersonalizationNavigationAsync(request.RoleGroupId, request.UserRoleId);
            if (result != null)
            {
                _logger.LogInfo(ServiceMessages.GetUserRolePersonalizationNavigation);
                return Ok(result);
            }
            else
            {
                _logger.LogInfo(ServiceMessages.GetUserRolePersonalizationFailed);
                return NotFound();
            }
        }

        /// <summary>
        /// Get User Personalization Navigation items
        /// </summary>
        /// <param name="roleGroupId">roleGroupId</param>
        /// <returns>ServiceResponse of type PersonalizationNavResponse</returns>
        [HttpGet]
        [Route("roleGroup/{roleGroupId}/leftNav/personalization")]
        [AuthorizeStore(ApiResource.LeftNav, ApiOperation.Read)]
        [ResponseType(typeof(ServiceResponse<List<LeftNavigationItem>>))]
        public async Task<IHttpActionResult> GetRoleGroupPersonalizationNavigationAsync([FromUri]RoleGroupLeftNavRequest request)
        {
            LogGetLeftNavTraceInfo(request.RoleGroupId, ResourceType.Persona, ServiceMessages.GetRoleGroupPersonalizationNavigationRequest);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.GetRoleGroupPersonalizationNavigation);
                return BadRequest(ModelState);
            }

            var result = await _dashboardService.GetPersonalizationNavigationAsync(request.RoleGroupId);
            if (result != null)
            {
                _logger.LogInfo(ServiceMessages.GetRoleGroupPersonalizationNavigation);
                return Ok(result);
            }
            else
            {
                _logger.LogInfo(ServiceMessages.GetRoleGroupPersonalizationFailed);
                return NotFound();
            }
        }

        /// <summary>
        /// save roleGroup Request
        /// </summary>
        /// <param name="roleGroupId"></param>
        /// <param name="personalizationRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("roleGroup/{roleGroupId}/leftNav/personalization")]
        [AuthorizeStore(ApiResource.LeftNav, ApiOperation.Write)]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> UpdateRoleGroupLeftNavigationAsync(string roleGroupId, [FromBody] LeftNavPersonalizationRequest personalizationRequest)
        {
            LogPersonalizationNavigation(roleGroupId, ResourceType.Persona, personalizationRequest, ServiceMessages.SaveRoleGroupPersonalizationNavigationRequest);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.UpdateLeftNavigationPersonalisationFailed);
                return BadRequest(ModelState);
            }

            return await UpdateLeftNavigationPersonalisationAsync(LeftNavType.RoleGroup, roleGroupId, personalizationRequest);
        }

        /// <summary>
        /// save roleGroup Request
        /// </summary>
        /// <param name="userRoleId"></param>
        /// <param name="personalizationRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("userRole/{userRoleId}/leftNav/personalization")]
        [AuthorizeStore(ApiResource.LeftNav, ApiOperation.Write)]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> UpdateUserRoleLeftNavigationAsync(string userRoleId, [FromBody] LeftNavPersonalizationRequest personalizationRequest)
        {
            LogPersonalizationNavigation(userRoleId, ResourceType.Persona, personalizationRequest, ServiceMessages.SaveRolePersonalizationNavigationRequest);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.UpdateLeftNavigationPersonalisationFailed);
                return BadRequest(ModelState);
            }

            return await UpdateLeftNavigationPersonalisationAsync(LeftNavType.UserRole, userRoleId, personalizationRequest);
        }

        /// <summary>
        /// Gets Master left navigation items
        /// </summary>
        /// <returns>ServiceResponse of type GlobalMasterNavResponse</returns>
        [HttpGet]
        [Route("leftNav/master")]
        [AuthorizeStore(ApiResource.LeftNav, ApiOperation.Read)]
        [ResponseType(typeof(ServiceResponse<List<LeftNavigationItem>>))]
        public async Task<IHttpActionResult> GetLeftNavigationMasterAsync()
        {
            LogLeftNavigationMaster(ServiceMessages.GetLeftNavigationMasterRequest);
            var result = await _dashboardService.GetLeftNavigationMasterAsync();
            if (result != null)
            {
                _logger.LogInfo(ServiceMessages.GetLeftNavigationMasterSuccess);
                return Ok(result);
            }
            else
            {
                _logger.LogInfo(ServiceMessages.NoDataFound);
                return NotFound();
            }
        }

        /// <summary>
        /// save userPersonalization Request
        /// </summary>
        /// <param name="userAlias"></param>
        /// <param name="userPersonalizations"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("user/{userAlias}/leftNav/personalization")]
        [AuthorizeStore(ApiResource.LeftNav, ApiOperation.Write)]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> UpdateUserLeftNavigationAsync(string userAlias, [FromBody] LeftNavPersonalizationRequest userPersonalization)
        {
            LogPersonalizationNavigation(userAlias, ResourceType.User, userPersonalization, ServiceMessages.SaveUserPersonalizationNavigationRequest);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.UpdateLeftNavigationPersonalisationFailed);
                return BadRequest(ModelState);
            }

            return await UpdateLeftNavigationPersonalisationAsync(LeftNavType.User, userAlias, userPersonalization);
        }

        private async Task<IHttpActionResult> UpdateLeftNavigationPersonalisationAsync(LeftNavType leftNavType, string settingId, LeftNavPersonalizationRequest personalizationRequest)
        {
            var validationErrors = _validator.ValidateUserPersonalizationNavigationRequest(personalizationRequest);
            if (validationErrors != null)
            {
                _logger.LogWarning(ServiceMessages.UpdateLeftNavigationPersonalisationFailed);
                return BadRequest(validationErrors);
            }

            var result = await _dashboardService.UpdateLeftNavigationAsync(leftNavType, settingId, personalizationRequest);

            if (result)
            {
                _logger.LogInfo(ServiceMessages.UpdateLeftNavigationPersonalisationSuccess);
                return Ok(result);
            }
            else
            {
                _logger.LogInfo(ServiceMessages.UpdateLeftNavigationPersonalisationFailed);
                return NotFound();
            }
        }

        /// <summary>
        /// Log get left nav request trace
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceType"></param>
        /// <param name="logMessage"></param>
        private void LogGetLeftNavTraceInfo(string resourceId, ResourceType resourceType, string logMessage)
        {
            var traceInfo = new Dictionary<string, string>();
            traceInfo.Add("ResourceType", resourceType.ToString());
            traceInfo.Add("ResourceId", resourceId);
            _logger.LogTrace(logMessage, traceInfo);
        }

        /// <summary>
        /// Log save personalization request trace
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="resourceType"></param>
        /// <param name="userPersonalizationRequest"></param>
        /// <param name="logMessage"></param>
        private void LogPersonalizationNavigation(string resourceId, ResourceType resourceType, LeftNavPersonalizationRequest personalizationRequest, string logMessage)
        {
            var traceInfo = new Dictionary<string, string>();
            traceInfo.Add("ResourceType", resourceType.ToString());
            traceInfo.Add("ResourceId", resourceId);
            if (personalizationRequest != null)
                traceInfo.Add("UserPersonalization", JsonConvert.SerializeObject(personalizationRequest));
            _logger.LogTrace(logMessage, traceInfo);
        }

        private void LogLeftNavigationMaster(string logMessage)
        {
            _logger.LogTrace(logMessage);
        }
    }
}