using DataLib.Models;
using FluentNHibernate.Mapping;

namespace DataLib.NHibernate.Mappings
{
    public class MerchantGroupMap : ClassMap<MerchantGroup>
    {
        public MerchantGroupMap()
        {
            Table("MERCHANT_GROUPS_SHOUQ");
            Id(x => x.Id).GeneratedBy.Sequence("SEQ_MERCHANT_GROUPS_SHOUQ");
            Map(x => x.Name_Ar).Column("NAME_AR");
            Map(x => x.Name_En).Column("NAME_EN");
            Map(x => x.CreatedAt).Column("CREATED_AT");
            Map(x => x.UpdatedAt).Column("UPDATED_AT");
            Map(x => x.DeletedAt).Column("DELETED_AT");
        }
    }
}