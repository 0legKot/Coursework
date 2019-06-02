using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsulService.Models
{
    public class DataContext : IdentityDbContext<User,Role,string>
    {
        public DataContext(DbContextOptions<DataContext> options)
              : base(options)
        {
        }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<LogMessage> Logs { get; set; }
    }
}
