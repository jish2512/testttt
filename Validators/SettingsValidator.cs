using Microsoft.PS.FXP.Core.Extensions;
using Microsoft.PS.FXP.Core.Services.ServiceTypes;
using Microsoft.PS.FXP.Services;
using System;
using System.Collections.Generic;
using System.Web.Http.ModelBinding;

namespace Microsoft.PS.FXP.Service.Api.Validators
{
    public class SettingsValidator : ISettingsValidator
    {
        private string _throttleLimit;
        private const string SETTINGS_VALIDATION_ERRORKEY = "Settings";
        private const string SETTINGNAME_VALIDATION_ERRORKEY = "SettingName";
        private const string SETTINGVALUE_VALIDATION_ERRORKEY = "SettingValue";
        private const string SETTINGNAMES_VALIDATION_ERRORKEY = "SettingNames";
        public SettingsValidator(string throttleLimit)
        {
            _throttleLimit = throttleLimit;
        }
        
        public ModelStateDictionary ValidateSaveRequest(SettingSaveRequest settings)
        {
            ModelStateDictionary modalState = null;

            if (settings == null)
                AddValidationError(ref modalState, SETTINGS_VALIDATION_ERRORKEY, ServiceMessages.InvalidParamSettings);
            else
            {
                if (string.IsNullOrWhiteSpace(settings.SettingName))
                    AddValidationError(ref modalState, SETTINGNAME_VALIDATION_ERRORKEY, ServiceMessages.InvalidParamSettingName);
                if (string.IsNullOrWhiteSpace(settings.SettingValue))
                    AddValidationError(ref modalState, SETTINGVALUE_VALIDATION_ERRORKEY, ServiceMessages.InvalidParamSettingValue);
            }

            return modalState;
        }

        public ModelStateDictionary ValidateGetRequest(List<string> settingNames)
        {
            ModelStateDictionary modalState = null;

            if (settingNames.IsNullOrEmpty())
            {
                AddValidationError(ref modalState, SETTINGNAMES_VALIDATION_ERRORKEY, ServiceMessages.InvalidParamSettingNames);
            }
            else if (settingNames.Count > Convert.ToInt32(_throttleLimit))
            {
                AddValidationError(ref modalState, SETTINGNAMES_VALIDATION_ERRORKEY, string.Format(ServiceMessages.InvalidSettingsCount, _throttleLimit));
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