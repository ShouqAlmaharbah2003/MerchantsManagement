using DataLib.Models;
using FluentNHibernate.Mapping;

namespace DataLib.NHibernate.Mappings
{
    public class LoginTokenMap : ClassMap<LoginToken>
    {
        public LoginTokenMap()
        {
            Table("LOGIN_TOKENS_SHOUQ");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.UserId).Column("UserId").Not.Nullable();
            Map(x => x.Token).Column("Token").Length(4000).Not.Nullable();
            Map(x => x.ExpiryDate).Column("ExpiryDate").Not.Nullable();
            Map(x => x.CreatedAt).Column("CreatedAt").Default("SYSDATE").Not.Nullable();
        }
    }
}