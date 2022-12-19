using IPHandlerWebAPI.Data;
using IPInfoProvider.Services;
using Microsoft.EntityFrameworkCore;

namespace IPHandlerWebAPI.Repositories
{
    public class IPRepository : IIPRepository
    {
        private readonly IPHandlerWebApiDBContext dbContext;

        public IPRepository(IPHandlerWebApiDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IP> GetCachedIP(string ip)
        {
            var ipObject = await dbContext.IPs.FirstOrDefaultAsync(y => y.Ip == ip);
            return ipObject;
        }

        public async Task<IP> AddCachedIP(IP ipDetails)
        {
            await dbContext.IPs.AddAsync(ipDetails);
            await dbContext.SaveChangesAsync();

            return ipDetails;
        }
    
    }
    
}
