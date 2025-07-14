using System.ComponentModel.DataAnnotations;

namespace CommonLib.Dtos.Requests
{
    public class MerchantRequest
    {

        [Required(ErrorMessage = "Merchant name in Arabic is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Merchant name in Arabic must be between 2 and 100 characters")]
        public string Name_Ar { get; set; }

        [Required(ErrorMessage = "Merchant name in English is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Merchant name in English must be between 2 and 100 characters")]
        public string Name_En { get; set; }

        [Required(ErrorMessage = "Business type is required")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid business type must be selected")]
        public int BusinessType { get; set; }

        [Required(ErrorMessage = "Merchant group ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid merchant group ID must be selected")]
        public int MerchantGroupId { get; set; }

        [StringLength(100, ErrorMessage = "Manager name must not exceed 100 characters")]
        public string ManagerName { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Range(0, int.MaxValue, ErrorMessage = "A valid status must be selected")]
        public int Status { get; set; }
    }
}