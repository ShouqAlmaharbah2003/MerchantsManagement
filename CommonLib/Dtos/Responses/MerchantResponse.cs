using System.ComponentModel.DataAnnotations;

namespace CommonLib.Dtos.Responses
{
    public class MerchantResponse
    {
        [Required(ErrorMessage = "Merchant ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Merchant ID must be a valid positive number")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Merchant name in Arabic is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Merchant name in Arabic must be between 2 and 100 characters")]
        public string Name_Ar { get; set; }

        [Required(ErrorMessage = "Merchant name in English is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Merchant name in English must be between 2 and 100 characters")]
        public string Name_En { get; set; }

        [Required(ErrorMessage = "Business type is required")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid business type must be selected")]
        public int BusinessType { get; set; }

        [StringLength(100, ErrorMessage = "Manager name must not exceed 100 characters")]
        public string ManagerName { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status must not exceed 50 characters")]
        public string Status { get; set; }
    }
}