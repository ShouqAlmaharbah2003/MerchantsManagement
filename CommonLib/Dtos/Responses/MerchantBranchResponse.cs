using System.ComponentModel.DataAnnotations;

namespace CommonLib.Dtos.Responses
{
    public class MerchantBranchResponse
    {
        [Required(ErrorMessage = "Branch ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a valid positive number")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Branch name in Arabic is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Branch name in Arabic must be between 2 and 100 characters")]
        public string BranchName_Ar { get; set; }

        [Required(ErrorMessage = "Branch name in English is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Branch name in English must be between 2 and 100 characters")]
        public string BranchName_En { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status must not exceed 50 characters")]
        public string Status { get; set; }
    }
}