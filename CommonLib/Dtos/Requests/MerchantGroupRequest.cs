using System.ComponentModel.DataAnnotations;

namespace CommonLib.Dtos.Requests
{
    public class MerchantGroupRequest
    {
        [Required(ErrorMessage = "Group name in Arabic is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Group name in Arabic must be between 2 and 100 characters")]
        public string Name_Ar { get; set; }

        [Required(ErrorMessage = "Group name in English is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Group name in English must be between 2 and 100 characters")]
        public string Name_En { get; set; }
    }
}