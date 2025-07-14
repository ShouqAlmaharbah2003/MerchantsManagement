using DataLib.Models;
using NHibernate;
using NHibernate.Linq;

namespace DataLib.Repositories
{
    public class LoginTokenRepository
    {
        private readonly ISession _session;

        public LoginTokenRepository(ISession session)
        {
            _session = session;
        }

        public async Task SaveTokenAsync(LoginToken token)
        {
            await _session.SaveAsync(token).ConfigureAwait(false);
        }

        public async Task<LoginToken> GetTokenAsync(string token)
        {
            return await _session.Query<LoginToken>()
                .FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var loginToken = await GetTokenAsync(token);
            return loginToken != null && loginToken.ExpiryDate > DateTime.UtcNow;
        }
    }
}