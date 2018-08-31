using Microsoft.PS.FXP.Core.Entities;
using Microsoft.PS.FXP.Services;
using System.Web.Http.ModelBinding;
using System.Collections.Generic;
using Microsoft.PS.FXP.Core.Services.ServiceTypes;
using System;
using Microsoft.PS.FXP.Services.DashboardServiceTypes;
using System.Linq;

namespace Microsoft.PS.FXP.Service.Api.Validators
{
    public class DashboardValidator : IDashboardValidator
    {
        private int _throttleLimit;
        private const string LEFTNAVIGATION_VALIDATION_ERRORKEY = "leftNavigation";
        private const string USERPERSONALIZATIONNAVIGATIONROLE_ROLEGROUPID_VALIDATION_ERRORKEY = "userPersonalizationNavigationroleGroupId";
        private const string USERPERSONALIZATIONNAVIGATIONCOUNT_VALIDATION_ERRORKEY = "userPersonalizationNavigationCount";
        private const string USERPERSONALIZATIONNAVIGATIONID_VALIDATION_ERRORKEY = "userPersonalizationNavigationId";

        public DashboardValidator(string throttleLimit)
        {
            _throttleLimit = Convert.ToInt32(throttleLimit);
        }

        public ModelStateDictionary ValidateUpdateLeftNavigation(LeftNavigation leftNavigation)
        {
            ModelStateDictionary modalState = null;

            if (leftNavigation == null) //TODO : need to add more once entity is finalised
            {
                AddValidationError(ref modalState, LEFTNAVIGATION_VALIDATION_ERRORKEY, ServiceMessages.InvalidParamLeftNavigation);
            }

            return modalState;
        }

        /// <summary>
        ///  validating roleGroupId 
        /// </summary>
        /// <param name="roleGroupId"></param>
        /// <returns>ModelStateDictionary</returns>
        public ModelStateDictionary ValidateUserPersonalizationroleGroupId(string roleGroupId)
        {
            ModelStateDictionary modalState = null;
            if (string.IsNullOrEmpty(roleGroupId))
            {
                AddValidationError(ref modalState, USERPERSONALIZATIONNAVIGATIONROLE_ROLEGROUPID_VALIDATION_ERRORKEY, ServiceMessages.InvalidParamRoleGroupId);
            }
            return modalState;
        }


        /// <summary>
        /// validating UserPersonalizationRequest 
        /// </summary>
        /// <param name="userPersonalization"></param>
        /// <returns>ModelStateDictionary</returns>
        public ModelStateDictionary ValidateUserPersonalizationNavigationRequest(LeftNavPersonalizationRequest userPersonalization)
        {
            ModelStateDictionary modalState = null;

            if (userPersonalization == null || (userPersonalization.AddedItems == null && userPersonalization.RemovedItems == null))
            {
                AddValidationError(ref modalState, USERPERSONALIZATIONNAVIGATIONCOUNT_VALIDATION_ERRORKEY, ServiceMessages.InvalidArguments);
            }
            else
            {
                if ((userPersonalization.AddedItems != null && userPersonalization.AddedItems.Count > _throttleLimit) || (userPersonalization.RemovedItems != null && userPersonalization.RemovedItems.Count > _throttleLimit))
                {
                    AddValidationError(ref modalState, USERPERSONALIZATIONNAVIGATIONCOUNT_VALIDATION_ERRORKEY, string.Format(ServiceMessages.InvalidSettingsCount, _throttleLimit));
                }

                if ((userPersonalization.AddedItems != null && userPersonalization.AddedItems.Any(item => item <= 0)) || (userPersonalization.RemovedItems != null && userPersonalization.RemovedItems.Any(item => item <= 0)))
                {
                    AddValidationError(ref modalState, USERPERSONALIZATIONNAVIGATIONID_VALIDATION_ERRORKEY, ServiceMessages.InvalidParamUserPersonalizationNavigationId);

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