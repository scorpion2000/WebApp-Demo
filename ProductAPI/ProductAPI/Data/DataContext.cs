using Microsoft.EntityFrameworkCore;

namespace ProductAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

        public DbSet<Product> Products => Set<Product>();
    }
}
