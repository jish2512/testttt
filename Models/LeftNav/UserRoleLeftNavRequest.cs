using System.ComponentModel.DataAnnotations;

namespace Microsoft.PS.FXP.Service.Api.Models.LeftNav
{
    public class UserRoleLeftNavRequest : RoleGroupLeftNavRequest
    {
        [Required(ErrorMessage = "UserRoleId is required")]
        public string UserRoleId { get; set; }
    }
}