using System.ComponentModel.DataAnnotations;

namespace Microsoft.PS.FXP.Service.Api.Models.LeftNav
{
    public class UserLeftNavRequest : UserRoleLeftNavRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserAlias { get; set; }
    }
}