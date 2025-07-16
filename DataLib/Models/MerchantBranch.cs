using System.ComponentModel.DataAnnotations;

namespace DataLib.Models
{
    public class MerchantBranch
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string BranchName_Ar { get; set; }
        public virtual string BranchName_En { get; set; }
        public virtual int ContactPersonId { get; set; }
        public virtual int CityId { get; set; }
        public virtual int GovernateId { get; set; }
        public virtual string AlHat { get; set; }
        public virtual string Address { get; set; }
        public virtual string Region { get; set; }
        public virtual string Fax { get; set; }
        public virtual string Website { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Gps { get; set; }
        public virtual int Status { get; set; }
        public virtual string MainBranch { get; set; }
        public virtual DateTime DeletedAt { get; set; } 
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
        // Many-to-One
        public virtual Merchant Merchant { get; set; }
        public virtual User User { get; set; }
    }
}