using Microsoft.EntityFrameworkCore;

namespace SmartHead.GatewayKey.Example.Dal
{
    public class ExampleDbContext : DbContext
    {
        public DbSet<Models.GatewayKey> GatewayKeys { get; set; }

        public ExampleDbContext(DbContextOptions<ExampleDbContext> options)
            :base(options)
        {
            
        }
    }
}