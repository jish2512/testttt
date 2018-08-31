using Microsoft.PS.FXP.Core.Enums;
using Microsoft.PS.FXP.Core.Extensions;
using Microsoft.PS.FXP.Core.Logging;
using Microsoft.PS.FXP.QueueMessageService;
using Microsoft.PS.FXP.Service.Api.Validators;
using Microsoft.PS.FXP.Services;
using Microsoft.PS.FXP.Services.NotificationTypes;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.PS.FXP.Service.Api.Controllers
{
    [RoutePrefix("api/v1")]
    public class NotificationController : ApiController
    {
        /// <summary>
        /// Queue service object
        /// </summary>
        private readonly IFxpMessageQueue _fxpMessageQueue;

        /// <summary>
        /// Logger service object
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// NotificationValidator object
        /// </summary>
        private readonly INotificationValidator _validator;

        public NotificationController(IFxpMessageQueue fxpMessageQueue, INotificationValidator notificationValidator, ILogger logger)
        {
            _fxpMessageQueue = fxpMessageQueue;
            _validator = notificationValidator;
            _logger = logger;
        }

        // GET: Notification
        [HttpPost]
        [Route("notification")]
        [Authorize]
        public async Task<IHttpActionResult> PublishNotification(NotificationRequest notification)
        {
            _logger.LogTrace(QueueServiceConstants.PublishNotificationRequest);

            var validationErrors = _validator.ValidatePublishNotificationRequest(notification);

            if (validationErrors != null)
            {
                _logger.LogWarning(QueueServiceConstants.InvalidParametersPublishNotification);
                return BadRequest(validationErrors);
            }

            List<string> errorMessages = new List<string>();
            notification.Source = SourceType.AuthorNotification;
            string response = null;

            response = await PublishNotificationInternal(notification.RoleGroupIdList, RecipientType.RoleGroup, notification.Source, notification.Message);
            if (!string.IsNullOrWhiteSpace(response))
            {
                errorMessages.Add(response);
            }

            response = await PublishNotificationInternal(notification.RoleIdList, RecipientType.Role, notification.Source, notification.Message);
            if (!string.IsNullOrWhiteSpace(response))
            {
                errorMessages.Add(response);
            }


            if (errorMessages.IsNullOrEmpty())
            {
                _logger.LogInfo(QueueServiceConstants.PublishNotificationSuccess);
                return Ok();
            }
            else
            {
                _logger.LogError(new System.Exception(QueueServiceConstants.PublishNotificationFailed), true);
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                errorMessage.Content = new StringContent(string.Join(";", errorMessages));
                throw new HttpResponseException(errorMessage);
            }
        }

        /// <summary>
        /// Publish notification for role/rolegroup list
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="recipientType"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<string> PublishNotificationInternal(List<int> idList, RecipientType recipientType, SourceType source, string message)
        {
            string response = null;
            if (!idList.IsNullOrEmpty())
            {
                var commaSeparatedIdList = string.Join(",", idList);
                response = await _fxpMessageQueue.QueueAsync(commaSeparatedIdList, recipientType, source, message, null, null);
            }
            return response;
        }
    }
}