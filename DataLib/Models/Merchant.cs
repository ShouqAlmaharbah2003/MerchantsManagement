using System.ComponentModel.DataAnnotations;

namespace DataLib.Models
{
    public class Merchant
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Name_Ar { get; set; }
        public virtual string Name_En { get; set; }
        public virtual int BusinessType { get; set; }
        public virtual int Status { get; set; }
        public virtual DateTime DeletedAt { get; set; } 
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
        public virtual string ManagerName { get; set; }
        // Many-to-One
        public virtual MerchantGroup MerchantGroup { get; set; }
        // One-to-Many
        public virtual IList<MerchantBranch> Branches { get; set; }
    }
}