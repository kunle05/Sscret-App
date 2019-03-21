using Microsoft.EntityFrameworkCore;
 
namespace Secrets.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {get; set;}
        public DbSet<Secret> Secrets {get; set;}
        public DbSet<Like> Likes {get; set;}
    }
}