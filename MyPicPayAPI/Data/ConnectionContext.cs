using Microsoft.EntityFrameworkCore;
using SimplePicPay.Helpers;
using SimplePicPay.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SimplePicPay.Data
{
    public class ConnectionContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }

        public ConnectionContext(DbContextOptions<ConnectionContext> options) : base(options)
        {   
        }
    }
}
