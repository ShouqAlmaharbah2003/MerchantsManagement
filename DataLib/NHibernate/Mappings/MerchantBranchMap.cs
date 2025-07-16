using DataLib.Models;
using FluentNHibernate.Mapping;

namespace DataLib.NHibernate.Mappings
{
    public class MerchantBranchMap : ClassMap<MerchantBranch>
    {
        public MerchantBranchMap()
        {
            Table("MERCHANT_BRANCHES_SHOUQ");
            Id(x => x.Id).GeneratedBy.Sequence("SEQ_MERCHANT_BRANCHES_SHOUQ");

            Map(x => x.BranchName_Ar).Column("BRANCH_NAME_AR");
            Map(x => x.BranchName_En).Column("BRANCH_NAME_EN");
            Map(x => x.CityId).Column("CITY_ID");
            Map(x => x.GovernateId).Column("GOVERNATE_ID");
            Map(x => x.AlHat).Column("AL_HAT");
            Map(x => x.Address).Column("ADDRESS");
            Map(x => x.Region).Column("REGION");
            Map(x => x.Fax).Column("FAX");
            Map(x => x.Website).Column("WEBSITE");
            Map(x => x.Phone).Column("PHONE");
            Map(x => x.Mobile).Column("MOBILE");
            Map(x => x.Gps).Column("GPS");
            Map(x => x.Status).Column("STATUS"); 
            Map(x => x.MainBranch).Column("MAIN_BRANCH");
            Map(x => x.DeletedAt).Column("DELETED_AT");
            Map(x => x.CreatedAt).Column("CREATED_AT");
            Map(x => x.UpdatedAt).Column("UPDATED_AT");

            // Many-to-One: Each Branch belongs to a Merchant
            References(x => x.Merchant)
                .Column("MERCHANT_ID")
                .Not.Nullable()
                .Cascade.None();

            // Many-to-One: Each Branch has a Contact User
            References(x => x.User)
                .Column("USER_ID")
                .Nullable()
                .Cascade.None();
        }
    }
}