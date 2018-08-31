using System.ComponentModel.DataAnnotations;

namespace Microsoft.PS.FXP.Service.Api.Models.LeftNav
{
    public class RoleGroupLeftNavRequest
    {
        [Required(ErrorMessage = "RoleGroupId is required")]
        public string RoleGroupId { get; set; }
    }
}