using Microsoft.PS.FXP.Core.Extensions;
using Microsoft.PS.FXP.QueueMessageService;
using Microsoft.PS.FXP.Services.NotificationTypes;
using System.Web.Http.ModelBinding;

namespace Microsoft.PS.FXP.Service.Api.Validators
{
    public class NotificationValidator : INotificationValidator
    {
        private const string NOTIFICATION_VALIDATION_NULL_ERRORKEY = "NotificationError_NULL";
        private const string NOTIFICATION_VALIDATION_IDS_ERRORKEY = "Notification_IDS";
        private const string NOTIFICATION_VALIDATION_MSG_ERRORKEY = "Notification_MSG";

        public ModelStateDictionary ValidatePublishNotificationRequest(NotificationRequest notification)
        {
            ModelStateDictionary modalState = null;

            if (notification == null)
            {
                AddValidationError(ref modalState, NOTIFICATION_VALIDATION_NULL_ERRORKEY, QueueServiceConstants.InvalidMessageParamPublishNotification);
            }
            else
            {
                if (notification.RoleGroupIdList.IsNullOrEmpty() && notification.RoleIdList.IsNullOrEmpty())
                {
                    AddValidationError(ref modalState, NOTIFICATION_VALIDATION_IDS_ERRORKEY, QueueServiceConstants.InvalidIdParamsPublishNotification);
                }

                if (string.IsNullOrEmpty(notification.Message))
                {
                    AddValidationError(ref modalState, NOTIFICATION_VALIDATION_MSG_ERRORKEY, QueueServiceConstants.InvalidMessageParamPublishNotification);
                }
            }
            return modalState;
        }

        private void AddValidationError(ref ModelStateDictionary modelState, string errorKey, string errorMessage)
        {
            if (modelState == null)
                modelState = new ModelStateDictionary();
            modelState.AddModelError(errorKey, errorMessage);
        }
    }
}