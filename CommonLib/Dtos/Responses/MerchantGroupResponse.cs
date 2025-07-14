using System.ComponentModel.DataAnnotations;

namespace CommonLib.Dtos.Responses
{
    public class MerchantGroupResponse
    {
        [Required(ErrorMessage = "Group ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Group ID must be a valid positive number")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Group name in Arabic is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Group name in Arabic must be between 2 and 100 characters")]
        public string Name_Ar { get; set; }

        [Required(ErrorMessage = "Group name in English is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Group name in English must be between 2 and 100 characters")]
        public string Name_En { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status must not exceed 50 characters")]
        public string Status { get; set; }
    }
}