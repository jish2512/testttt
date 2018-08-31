using Microsoft.PS.FXP.Core.Services.ServiceTypes;
using System.Collections.Generic;
using System.Web.Http.ModelBinding;

namespace Microsoft.PS.FXP.Service.Api.Validators
{
    public interface ISettingsValidator
    {
        ModelStateDictionary ValidateSaveRequest(SettingSaveRequest settings);
        ModelStateDictionary ValidateGetRequest(List<string> settingNames);
    }
}