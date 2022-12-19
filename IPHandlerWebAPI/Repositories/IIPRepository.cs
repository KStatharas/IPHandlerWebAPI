using IPInfoProvider.Services;

namespace IPHandlerWebAPI.Repositories
{
    public interface IIPRepository
    {
        public Task<IP> GetCachedIP(string ip);
        public Task<IP> AddCachedIP(IP ipDetails);
    }
}
