using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;

namespace Microsoft.PS.FXP.Service.Api.Validators
{
    public interface ISystemMessagesValidator
    {
        ModelStateDictionary ValidateGetSystemMessagesRequestParams(int pageSize = 0, int pageNo = 0, string sortOrder = "Asc");
    }
}
