using DataLib.Models;
using FluentNHibernate.Mapping;

namespace DataLib.NHibernate.Mappings
{
    public class MerchantMap : ClassMap<Merchant>
    {
        public MerchantMap()
        {
            Table("MERCHANTS_SHOUQ");
            Id(x => x.Id).GeneratedBy.Sequence("SEQ_MERCHANTS_SHOUQ");

            Map(x => x.Name_Ar).Column("NAME_AR");
            Map(x => x.Name_En).Column("NAME_EN");
            Map(x => x.BusinessType).Column("BUSINESS_TYPE");
            Map(x => x.Status).Column("STATUS"); 
            Map(x => x.DeletedAt).Column("DELETED_AT");
            Map(x => x.CreatedAt).Column("CREATED_AT");
            Map(x => x.UpdatedAt).Column("UPDATED_AT");
            Map(x => x.ManagerName).Column("MANAGER_NAME");

            // Many-to-One: Each Merchant belongs to a MerchantGroup
            References(x => x.MerchantGroup)
                .Column("MERCHANT_GROUP_ID")
                .Not.Nullable()
                .Cascade.None();

            // One-to-Many: Each Merchant has many Branches
            HasMany(x => x.Branches)
                .KeyColumn("MERCHANT_ID")
                .Inverse()
                .Cascade.All();
        }
    }
}