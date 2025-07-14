using System.ComponentModel.DataAnnotations;

namespace CommonLib.Dtos.Responses
{
    public class MerchantWithBranchesResponse
    {
        [Required(ErrorMessage = "Merchant ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Merchant ID must be a valid positive number")]
        public int MerchantId { get; set; }

        [Required(ErrorMessage = "Merchant name in Arabic is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Merchant name in Arabic must be between 2 and 100 characters")]
        public string MerchantName_Ar { get; set; }

        [Required(ErrorMessage = "Merchant name in English is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Merchant name in English must be between 2 and 100 characters")]
        public string MerchantName_En { get; set; }

        public List<MerchantBranchResponse> Branches { get; set; } 
    }
}