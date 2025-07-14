using System.ComponentModel.DataAnnotations;

namespace DataLib.Models
{
    public class MerchantBranch
    {
        [Key]
        public virtual int Id { get; set; }

        [Required(ErrorMessage = "Branch name in Arabic is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Branch name in Arabic must be between 2 and 100 characters")]
        public virtual string BranchName_Ar { get; set; }

        [Required(ErrorMessage = "Branch name in English is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Branch name in English must be between 2 and 100 characters")]
        public virtual string BranchName_En { get; set; }

        [Required(ErrorMessage = "Contact person ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid contact person ID must be selected")]
        public virtual int ContactPersonId { get; set; }

        [Required(ErrorMessage = "City ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid city ID must be selected")]
        public virtual int CityId { get; set; }

        [Required(ErrorMessage = "Governate ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid governate ID must be selected")]
        public virtual int GovernateId { get; set; }

        [StringLength(50, ErrorMessage = "Phone number must not exceed 50 characters")]
        public virtual string AlHat { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
        public virtual string Address { get; set; }

        [StringLength(100, ErrorMessage = "Region must not exceed 100 characters")]
        public virtual string Region { get; set; }

        [StringLength(50, ErrorMessage = "Fax number must not exceed 50 characters")]
        [RegularExpression(@"^(\+?\d{1,4}[\s-]?)?\d{7,15}$", ErrorMessage = "Invalid fax number format")]
        public virtual string Fax { get; set; }

        [StringLength(100, ErrorMessage = "Website URL must not exceed 100 characters")]
        [Url(ErrorMessage = "Invalid website URL format")]
        public virtual string Website { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(50, ErrorMessage = "Phone number must not exceed 50 characters")]
        [RegularExpression(@"^(\+?\d{1,4}[\s-]?)?\d{7,15}$", ErrorMessage = "Invalid phone number format")]
        public virtual string Phone { get; set; }

        [StringLength(50, ErrorMessage = "Mobile number must not exceed 50 characters")]
        [RegularExpression(@"^(\+?\d{1,4}[\s-]?)?\d{7,15}$", ErrorMessage = "Invalid mobile number format")]
        public virtual string Mobile { get; set; }

        [StringLength(100, ErrorMessage = "GPS coordinates must not exceed 100 characters")]
        [RegularExpression(@"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?),\s*[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$",
            ErrorMessage = "Invalid GPS coordinates format (e.g., latitude,longitude)")]
        public virtual string Gps { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Range(0, int.MaxValue, ErrorMessage = "A valid status must be selected")]
        public virtual int Status { get; set; }

        [StringLength(50, ErrorMessage = "Main branch indicator must not exceed 50 characters")]
        public virtual string MainBranch { get; set; }

        public virtual DateTime DeletedAt { get; set; } // Nullable as it may not be deleted

        [Required(ErrorMessage = "Creation date is required")]
        public virtual DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Update date is required")]
        public virtual DateTime UpdatedAt { get; set; }

        // Many-to-One
        [Required(ErrorMessage = "Merchant is required")]
        public virtual Merchant Merchant { get; set; }

        [Required(ErrorMessage = "User is required")]
        public virtual User User { get; set; }
    }
}