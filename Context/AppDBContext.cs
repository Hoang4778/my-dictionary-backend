using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Context
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        public DbSet<UserAccount> UserAccount { get; set; }
        public DbSet<FavoriteWord> FavoriteWord { get; set; }
    }
}
