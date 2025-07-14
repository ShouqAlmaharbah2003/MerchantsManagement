using System.ComponentModel.DataAnnotations;

namespace DataLib.Models
{
    public class MerchantGroup
    {
        [Key]
        public virtual int Id { get; set; }

        [Required(ErrorMessage = "Group name in Arabic is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Group name in Arabic must be between 2 and 100 characters")]
        public virtual string Name_Ar { get; set; }

        [Required(ErrorMessage = "Group name in English is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Group name in English must be between 2 and 100 characters")]
        public virtual string Name_En { get; set; }

        [Required(ErrorMessage = "Creation date is required")]
        public virtual DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Update date is required")]
        public virtual DateTime UpdatedAt { get; set; }

        public virtual DateTime DeletedAt { get; set; } // Nullable as it may not be deleted

        // One-to-Many
        public virtual IList<Merchant> Merchants { get; set; }
    }
}