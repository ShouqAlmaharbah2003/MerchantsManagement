using System.ComponentModel.DataAnnotations;

namespace DataLib.Models
{
    public class Merchant
    {
        [Key]
        public virtual int Id { get; set; }

        [Required(ErrorMessage = "Merchant name in Arabic is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Merchant name in Arabic must be between 2 and 100 characters")]
        public virtual string Name_Ar { get; set; }

        [Required(ErrorMessage = "Merchant name in English is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Merchant name in English must be between 2 and 100 characters")]
        public virtual string Name_En { get; set; }

        [Required(ErrorMessage = "Business type is required")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid business type number must be selected")]
        public virtual int BusinessType { get; set; }

        [Required(ErrorMessage = "Merchant status is required")]
        [Range(0, int.MaxValue, ErrorMessage = "A valid status must be selected")]
        public virtual int Status { get; set; }

        public virtual DateTime DeletedAt { get; set; } // Nullable as it may not be deleted

        [Required(ErrorMessage = "Creation date is required")]
        public virtual DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Update date is required")]
        public virtual DateTime UpdatedAt { get; set; }

        [StringLength(100, ErrorMessage = "Manager name must not exceed 100 characters")]
        public virtual string ManagerName { get; set; }

        // Many-to-One
        [Required(ErrorMessage = "Merchant group is required")]
        public virtual MerchantGroup MerchantGroup { get; set; }

        // One-to-Many
        public virtual IList<MerchantBranch> Branches { get; set; }
    }
}