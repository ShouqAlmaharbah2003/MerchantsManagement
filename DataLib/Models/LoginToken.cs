namespace DataLib.Models
{
    public class LoginToken
    {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual string Token { get; set; }
        public virtual DateTime ExpiryDate { get; set; }
        public virtual DateTime CreatedAt { get; set; }
    }
}