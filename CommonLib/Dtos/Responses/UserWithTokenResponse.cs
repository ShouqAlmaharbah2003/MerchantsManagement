using System.ComponentModel.DataAnnotations;

namespace CommonLib.Dtos.Responses
{
    public class UserWithTokenResponse
    {
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a valid positive number")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Token is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Token must be between 10 and 500 characters")]
        public string Token { get; set; }
    }
}