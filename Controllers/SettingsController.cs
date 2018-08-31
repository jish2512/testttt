using Microsoft.PS.FXP.Core.CustomTypes;
using Microsoft.PS.FXP.Core.Logging;
using Microsoft.PS.FXP.Core.Services;
using Microsoft.PS.FXP.Core.Services.Enums;
using Microsoft.PS.FXP.Core.Services.ServiceTypes;
using Microsoft.PS.FXP.Service.Api.Constants;
using Microsoft.PS.FXP.Service.Api.Security;
using Microsoft.PS.FXP.Service.Api.Validators;
using Microsoft.PS.FXP.Services;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Microsoft.PS.FXP.Service.Api.Controllers
{
    /// <summary>
    /// Controller for settings
    /// </summary>
    [RoutePrefix("api/v1")]
    public class SettingsController : ApiController
    {
        /// <summary>
        /// Settings service object
        /// </summary>
        protected readonly ISettingsService _settingsService;

        /// <summary>
        /// Logger service object
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// SettingsValidator object
        /// </summary>
        protected readonly ISettingsValidator _validator;

        private const string SETTINGNAMEPREFIX = "Setting_";
        /// <summary>
        /// Initializes the base members
        /// </summary>
        /// <param name="settingsService"></param>
        /// <param name="logger"></param>
        public SettingsController(ISettingsService settingsService, ISettingsValidator validator, ILogger logger)
        {
            _settingsService = settingsService;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Get Settings by setting name, type and settingId
        /// </summary>
        /// <param name="SettingName">List of Setting Names</param>
        /// <param name="settingType">Type of Setting (User/UserGroup/Application)</param>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Returns a OK status code with SettingResponse list object</returns>
        [HttpGet]
        [Route("user/{userAlias}/settings")]
        [AuthorizeStore(ApiResource.UserSettings, ApiOperation.Read, true)]
        [ResponseType(typeof(List<SettingResponse>))]
        public async Task<IHttpActionResult> GetUserSettingsAsync([FromUri] List<string> settingNames, string userAlias)
        {
            LogGetSettingsTraceInfo(settingNames, SettingType.User, userAlias, ServiceMessages.GetUserSettingsRequest);

            var validationResult = _validator.ValidateGetRequest(settingNames);
            if (validationResult != null)
            {
                _logger.LogWarning(ServiceMessages.GetUserSettingsFailed);
                return BadRequest(validationResult);
            }

            List<SettingRequest> request = new List<SettingRequest>();

            settingNames.ForEach(setting => request.Add(new SettingRequest { SettingId = userAlias, SettingName = SETTINGNAMEPREFIX + setting, SettingType = SettingType.User }));

            var serviceResponse = await _settingsService.GetSettingsAsync<string>(request);

            if (serviceResponse != null)
            {
                serviceResponse.ForEach(setting => setting.SettingName = setting.SettingName.Replace(SETTINGNAMEPREFIX, ""));
                _logger.LogInfo(ServiceMessages.GetUserSettingsSuccess);
                return Ok(serviceResponse);
            }
            else
            {
                _logger.LogWarning(ServiceMessages.NoDataFound);
                return NotFound();
            }
        }

        /// <summary>
        /// Get Settings by setting name, type and settingId
        /// </summary>
        /// <param name="SettingName">List of Setting Names</param>
        /// <param name="settingType">Type of Setting (User/UserGroup/Application)</param>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Returns a OK status code with SettingResponse list object</returns>
        [HttpGet]
        [Route("app/{appId}/settings")]
        [AuthorizeStore]
        [ResponseType(typeof(List<SettingResponse>))]
        public async Task<IHttpActionResult> GetAppSettingsAsync([FromUri] List<string> settingNames, string appId)
        {
            LogGetSettingsTraceInfo(settingNames, SettingType.Application, appId, ServiceMessages.GetAppSettingsRequest);

            var validationResult = _validator.ValidateGetRequest(settingNames);
            if (validationResult != null)
            {
                _logger.LogWarning(ServiceMessages.GetAppSettingsFailed);
                return BadRequest(validationResult);
            }

            List<SettingRequest> request = new List<SettingRequest>();

            settingNames.ForEach(setting => request.Add(new SettingRequest { SettingId = appId, SettingName = SETTINGNAMEPREFIX + setting, SettingType = SettingType.Application }));

            var serviceResponse = await _settingsService.GetSettingsAsync<string>(request);
            if (serviceResponse != null)
            {
                serviceResponse.ForEach(setting => setting.SettingName = setting.SettingName.Replace(SETTINGNAMEPREFIX, ""));
                _logger.LogInfo(ServiceMessages.GetAppSettingsSuccess);
                return Ok(serviceResponse);
            }
            else
            {
                _logger.LogWarning(ServiceMessages.NoDataFound);
                return NotFound();
            }
        }

        /// <summary>
        /// Get Settings by setting name, type and settingId
        /// </summary>
        /// <param name="SettingName">List of Setting Names</param>
        /// <param name="settingType">Type of Setting (User/UserGroup/Application)</param>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Returns a OK status code with SettingResponse list object</returns>
        [HttpGet]
        [Route("usergroup/{roleGroupId}/settings")]
        [AuthorizeStore(ApiResource.UserGroupSettings, ApiOperation.Read)]
        [ResponseType(typeof(List<SettingResponse>))]
        public async Task<IHttpActionResult> GetUserGroupSettingsAsync([FromUri] List<string> settingNames, string roleGroupId)
        {
            LogGetSettingsTraceInfo(settingNames, SettingType.UserGroup, roleGroupId, ServiceMessages.GetUserGroupSettingsRequest);

            var validationResult = _validator.ValidateGetRequest(settingNames);
            if (validationResult != null)
            {
                _logger.LogWarning(ServiceMessages.GetUserGroupSettingsFailed);
                return BadRequest(validationResult);
            }

            List<SettingRequest> request = new List<SettingRequest>();

            settingNames.ForEach(setting => request.Add(new SettingRequest { SettingId = roleGroupId, SettingName = SETTINGNAMEPREFIX + setting, SettingType = SettingType.UserGroup }));

            var serviceResponse = await _settingsService.GetSettingsAsync<string>(request);

            if (serviceResponse != null)
            {
                serviceResponse.ForEach(setting => setting.SettingName = setting.SettingName.Replace(SETTINGNAMEPREFIX, ""));
                _logger.LogInfo(ServiceMessages.GetUserGroupSettingsSuccess);
                return Ok(serviceResponse);
            }
            else
            {
                _logger.LogWarning(ServiceMessages.NoDataFound);
                return NotFound();
            }
        }

        /// <summary>
        /// Save setting details
        /// </summary>
        /// <param name="settings">SettingRequest object as a input</param>
        /// <returns>Returns OK status code</returns>
        [HttpPost]
        [Route("user/{userAlias}/settings")]
        [AuthorizeStore(ApiResource.UserSettings, ApiOperation.Write, true)]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> SaveUserSettingsAsync([FromBody] SettingSaveRequest settings, [FromUri] string userAlias)
        {
            LogSaveSettingsTraceInfo(settings, SettingType.User, userAlias, ServiceMessages.SaveUserSettingsRequest);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.SaveUserSettingsFailed);
                return BadRequest(ModelState);
            }

            var validationResult = _validator.ValidateSaveRequest(settings);
            if (validationResult != null)
            {
                _logger.LogWarning(ServiceMessages.SaveUserSettingsFailed);
                return BadRequest(validationResult);
            }

            var result = await _settingsService.SaveSettingAsync(SETTINGNAMEPREFIX + settings.SettingName, SettingType.User, userAlias, settings.SettingValue);
            if (result)
            {
                _logger.LogInfo(ServiceMessages.SaveUserSettingsSuccess);
                return Ok(true);
            }
            else
            {
                _logger.LogWarning(ServiceMessages.FailedToSave);
                return NotFound();
            }
        }

        /// <summary>
        /// Save setting details
        /// </summary>
        /// <param name="settings">SettingRequest object as a input</param>
        /// <returns>Returns OK status code</returns>
        [HttpPost]
        [Route("app/{appId}/settings")]
        [AuthorizeStore(ApiResource.AppSettings, ApiOperation.Write)]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> SaveAppSettingsAsync([FromBody] SettingSaveRequest settings, [FromUri] string appId)
        {
            LogSaveSettingsTraceInfo(settings, SettingType.Application, appId, ServiceMessages.SaveAppSettingsRequest);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.SaveAppSettingsFailed);
                return BadRequest(ModelState);
            }

            var validationResult = _validator.ValidateSaveRequest(settings);
            if (validationResult != null)
            {
                _logger.LogWarning(ServiceMessages.SaveAppSettingsFailed);
                return BadRequest(validationResult);
            }

            var result = await _settingsService.SaveSettingAsync(SETTINGNAMEPREFIX + settings.SettingName, SettingType.Application, appId, settings.SettingValue);
            if (result)
            {
                _logger.LogInfo(ServiceMessages.SaveAppSettingsSuccess);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning(ServiceMessages.FailedToSave);
                return NotFound();
            }
        }

        /// <summary>
        /// Save setting details
        /// </summary>
        /// <param name="settings">SettingRequest object as a input</param>
        /// <returns>Returns OK status code</returns>
        [HttpPost]
        [Route("usergroup/{roleGroupId}/settings")]
        [AuthorizeStore(ApiResource.UserGroupSettings, ApiOperation.Write)]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> SaveUserGroupSettingsAsync([FromBody] SettingSaveRequest settings, [FromUri] string roleGroupId)
        {
            LogSaveSettingsTraceInfo(settings, SettingType.UserGroup, roleGroupId, ServiceMessages.SaveUserGroupSettingsRequest);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(ServiceMessages.SaveUserGroupSettingsFailed);
                return BadRequest(ModelState);
            }

            var validationResult = _validator.ValidateSaveRequest(settings);
            if (validationResult != null)
            {
                _logger.LogWarning(ServiceMessages.SaveUserGroupSettingsFailed);
                return BadRequest(validationResult);
            }

            var result = await _settingsService.SaveSettingAsync(SETTINGNAMEPREFIX + settings.SettingName, SettingType.UserGroup, roleGroupId, settings.SettingValue);

            if (result)
            {
                _logger.LogInfo(ServiceMessages.SaveUserGroupSettingsSuccess);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning(ServiceMessages.FailedToSave);
                return NotFound();
            }
        }

        private void LogGetSettingsTraceInfo(List<string> settingNames, SettingType settingType, string settingId, string logMessage)
        {
            var traceInfo = new Dictionary<string, string>();
            if (settingNames != null)
                traceInfo.Add("SettingNames", JsonConvert.SerializeObject(settingNames));
            traceInfo.Add("SettingType", settingType.ToString());
            traceInfo.Add("SettingId", settingId);
            _logger.LogTrace(logMessage, traceInfo);
        }

        private void LogSaveSettingsTraceInfo(SettingSaveRequest settings, SettingType settingType, string settingId, string logMessage)
        {
            var traceInfo = new Dictionary<string, string>();
            if (settings != null)
                traceInfo.Add("Settings", JsonConvert.SerializeObject(settings));
            traceInfo.Add("SettingType", settingType.ToString());
            traceInfo.Add("SettingId", settingId);
            _logger.LogTrace(logMessage, traceInfo);
        }
    }
}
