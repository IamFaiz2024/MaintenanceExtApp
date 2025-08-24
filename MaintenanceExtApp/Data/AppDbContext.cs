using MaintenanceExtApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace MaintenanceExtApp.Data
{
    public class MaintenanceDbContext : DbContext
    {
        public MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options)
            : base(options)
        {
        }

        public DbSet<LogEntry> Logs { get; set; }
        public DbSet<Models.Host> Hosts { get; set; }
    }
}
