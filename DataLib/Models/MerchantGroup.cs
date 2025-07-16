using System.ComponentModel.DataAnnotations;

namespace DataLib.Models
{
    public class MerchantGroup
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Name_Ar { get; set; }
        public virtual string Name_En { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
        public virtual DateTime DeletedAt { get; set; } 
        // One-to-Many
        public virtual IList<Merchant> Merchants { get; set; }
    }
}