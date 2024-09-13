using Microsoft.EntityFrameworkCore;
using Hummpty_Collections.Models;

namespace Hummpty_Collections.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Barcode> Barcodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\MSSQLSERVER01;Database=master;Trusted_Connection=True;TrustServerCertificate=True;");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Barcode>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Barcodes)
                .HasForeignKey(b => b.CustomerId);
        }
    }
}