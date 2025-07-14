using DataLib.Models;
using FluentNHibernate.Mapping;

namespace DataLib.NHibernate.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("USERS_SHOUQ");
            Id(x => x.Id).GeneratedBy.Sequence("SEQ_USERS_SHOUQ");
            Map(x => x.Username).Column("USERNAME");
            Map(x => x.Email).Column("EMAIL");
            Map(x => x.PasswordHash).Column("PASSWORD_HASH");
            Map(x => x.CreatedAt).Column("CREATED_AT");
            Map(x => x.DeletedAt).Column("DELETED_AT").Nullable();
        }
    }
}
