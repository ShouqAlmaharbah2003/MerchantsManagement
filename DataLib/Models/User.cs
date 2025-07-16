using System.ComponentModel.DataAnnotations;

namespace DataLib.Models
{
    public class User
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime DeletedAt { get; set; } 
    }
}