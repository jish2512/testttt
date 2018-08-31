using Microsoft.PS.FXP.Core.Entities;
using System.Web.Http.ModelBinding;
using System.Collections.Generic;
using Microsoft.PS.FXP.Services.DashboardServiceTypes;

namespace Microsoft.PS.FXP.Service.Api.Validators
{
    public interface IDashboardValidator
    {
        /// <summary>
        /// Validate Update LeftNavigation
        /// </summary>
        /// <param name="leftNavigation"></param>
        /// <returns>ModelStateDictionary</returns>
        ModelStateDictionary ValidateUpdateLeftNavigation(LeftNavigation leftNavigation);
        /// <summary>
        /// Validate User Personalization roleGroupId
        /// </summary>
        /// <param name="roleGroupId"></param>
        /// <returns> ModelStateDictionary</returns>
        ModelStateDictionary ValidateUserPersonalizationroleGroupId(string roleGroupId);
        /// <summary>
        /// Validate User Personalization Navigation Request
        /// </summary>
        /// <param name="userPersonalizationRequest"></param>
        /// <returns>ModelStateDictionary</returns>
        ModelStateDictionary ValidateUserPersonalizationNavigationRequest(LeftNavPersonalizationRequest userPersonalization);
    }
}