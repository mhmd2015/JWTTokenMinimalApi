using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace WebApi50.Data
{
    public class DbUsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        //private readonly IConfiguration configuration;
        //public DbUsersContext(IConfiguration configuration) {
        //    this.configuration = configuration;
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //options.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=DbUsers;User Id=someuser;Password=*****");
            options.UseSqlite("Data Source=users.db");
        }
    }
}
