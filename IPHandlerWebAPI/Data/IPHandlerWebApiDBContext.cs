using IPInfoProvider.Services;
using Microsoft.EntityFrameworkCore;

namespace IPHandlerWebAPI.Data

{
    /**
     * This is Entity's Framework DBContext. It's used for the creation of Database's schema.
     */
    public class IPHandlerWebApiDBContext : DbContext
    {
        public IPHandlerWebApiDBContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<IP> IPs { get; set; }
    }
}
