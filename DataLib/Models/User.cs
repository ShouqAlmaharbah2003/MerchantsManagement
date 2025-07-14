using System.ComponentModel.DataAnnotations;

namespace DataLib.Models
{
    public class User
    {
        [Key]
        public virtual int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
        public virtual string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
        public virtual string Email { get; set; }

        [Required(ErrorMessage = "Password hash is required")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Password hash must be between 6 and 256 characters")]
        public virtual string PasswordHash { get; set; }

        [Required(ErrorMessage = "Creation date is required")]
        public virtual DateTime CreatedAt { get; set; }

        public virtual DateTime DeletedAt { get; set; } // Nullable as it may not be deleted
    }
}