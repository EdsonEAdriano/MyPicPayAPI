using Microsoft.EntityFrameworkCore;
using SimplePicPay.Helpers;
using SimplePicPay.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SimplePicPay.Data
{
    public class AppConnectionDBContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("DataSource=app.db;Cache=Shared");
    }
}
