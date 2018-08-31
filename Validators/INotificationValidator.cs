using Microsoft.PS.FXP.Services.NotificationTypes;
using System.Web.Http.ModelBinding;

namespace Microsoft.PS.FXP.Service.Api.Validators
{
    public interface INotificationValidator
    {
        ModelStateDictionary ValidatePublishNotificationRequest(NotificationRequest notification);
    }
}