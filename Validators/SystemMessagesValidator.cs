using Microsoft.PS.FXP.Core.Entities;
using Microsoft.PS.FXP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Microsoft.PS.FXP.Service.Api.Validators
{
    public class SystemMessagesValidator : ISystemMessagesValidator
    {
        private const string SYSTEMMESSAGES_PAGESIZE = "SYSTEMMESSAGES_PAGESIZE";
        private const string SYSTEMMESSAGES_PAGENO = "SYSTEMMESSAGES_PAGENO";
        private const string SYSTEMMESSAGES_SORTORDER = "SYSTEMMESSAGES_SORTORDER";
        public SystemMessagesValidator() { }
        public ModelStateDictionary ValidateGetSystemMessagesRequestParams(int pageSize = 0, int pageNo = 0, string sortOrder="Asc")
        {
            ModelStateDictionary modalState = null;

            if (pageSize <= 0)
                AddValidationError(ref modalState, SYSTEMMESSAGES_PAGESIZE, ServiceMessages.InvalidPageSize);

            if (pageNo <= 0)
                AddValidationError(ref modalState, SYSTEMMESSAGES_PAGENO, ServiceMessages.InvalidPageNo);
            
            if (!Enum.IsDefined(typeof(SortOrder), sortOrder.ToUpper()))
                AddValidationError(ref modalState, SYSTEMMESSAGES_SORTORDER, ServiceMessages.InvalidSortOrder);            

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