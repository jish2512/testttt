using Microsoft.PS.FXP.Core.Entities;
using Microsoft.PS.FXP.Core.Logging;
using Microsoft.PS.FXP.Core.Services;
using Microsoft.PS.FXP.Core.Services.ServiceTypes;
using Microsoft.PS.FXP.Service.Api.Constants;
using Microsoft.PS.FXP.Service.Api.Security;
using Microsoft.PS.FXP.Service.Api.Validators;
using Microsoft.PS.FXP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Microsoft.PS.FXP.Service.Api.Controllers
{
    [RoutePrefix("api/v1")]
    public class SystemMessagesController : ApiController
    {
        /// <summary>
        /// SystemMessages service object
        /// </summary>
        protected readonly ISystemMessagesService _systemMessagesService;

        /// <summary>
        /// Logger service object
        /// </summary>
        protected readonly ILogger _logger;

        protected readonly ISystemMessagesValidator _validator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="systemMessagesService"></param>
        /// <param name="logger"></param>
        public SystemMessagesController(ISystemMessagesService systemMessagesService, ISystemMessagesValidator validator, ILogger logger)
        {
            _systemMessagesService = systemMessagesService;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Get System Messages Pagination
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(SystemMessagesResponse))]
        [Route("systemmessages")]
        [Authorize]
        public async Task<IHttpActionResult> GetSystemMessages([FromUri] int pageSize = 0, int pageNo = 0, string sortOrder = "ASC")
        {
            _logger.LogTrace(ServiceMessages.GetSystemMessagesRequest);

            var validationErrors = _validator.ValidateGetSystemMessagesRequestParams(pageSize, pageNo, sortOrder);

            if (validationErrors != null)
            {
                _logger.LogWarning(ServiceMessages.GetSystemMessagesFailed);
                return BadRequest(validationErrors);
            }

            var serviceResponse = await _systemMessagesService.GetSystemMessagesAsync(pageSize, pageNo, sortOrder);

            if (serviceResponse != null)
            {
                _logger.LogInfo(ServiceMessages.GetSystemMessagesSuccess);
                return Ok(serviceResponse);
            }
            else
            {
                _logger.LogWarning(ServiceMessages.GetSystemMessagesFailed);
                return NotFound();
            }
        }

        /// <summary>
        /// Save system Message
        /// </summary>
        /// <param name="systemMessage">Entity</param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(string))]
        [Route("systemmessages")]
        [AuthorizeStore(ApiResource.SystemMessages, ApiOperation.Write)]
        public async Task<IHttpActionResult> SaveSystemMessage([FromBody]SystemMessageEntity systemMessage)
        {
            _logger.LogTrace(ServiceMessages.SaveSystemMessagesRequest);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _systemMessagesService.SaveSystemMessagesAsync(systemMessage);
            if (!string.IsNullOrEmpty(response))
            {
                _logger.LogInfo(ServiceMessages.SaveSystemMessagesSuccess);
                return Ok(new { Id = response });
            }
            else
            {
                _logger.LogWarning(ServiceMessages.SaveSystemMessagesFailed);
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete system Message by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("systemmessages")]
        [AuthorizeStore(ApiResource.SystemMessages, ApiOperation.Write)]
        public async Task<IHttpActionResult> DeleteSystemMessage([FromUri] string id)
        {
            _logger.LogTrace(ServiceMessages.DeleteSystemMessageRequest);

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(ServiceMessages.InvalidSystemMessageId);
            }
            var response = await _systemMessagesService.DeleteSystemMessagesAsync(id);

            if (response)
            {
                _logger.LogInfo(ServiceMessages.DeleteSystemMessageSuccess);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning(ServiceMessages.DeleteSystemMessagesFailed);
                return NotFound();
            }
        }

        [HttpGet]
        [ResponseType(typeof(SystemMessagesResponse))]
        [Route("systemmessages/getplanneddowntime")]
        [Authorize]
        public async Task<IHttpActionResult> GetPlannedDowntimeMessage([FromUri] int interval = -1)
        {
            _logger.LogTrace(ServiceMessages.GetPlannedDownTimeRequest);

            var serviceResponse = await _systemMessagesService.GetPlannedDowntimeAsync(interval);

            if (serviceResponse != null)
            {
                _logger.LogInfo(ServiceMessages.GetPlannedDownTimeSuccess);
                return Ok(serviceResponse);
            }
            else
            {
                _logger.LogWarning(ServiceMessages.GetPlannedDownTimeFailed);
                return NotFound();
            }
        }
    }
}