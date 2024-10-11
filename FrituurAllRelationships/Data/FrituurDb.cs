using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrituurAllRelationships.Models;

namespace MVCp2Relatie.Data
{
    public class FrituurDb : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderLine> OrderLines { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsbuilder)
        {
            string connection = @"Data Source=.;Initial Catalog=FrituurDb;Integrated Security=True; TrustServerCertificate =True;";
            optionsbuilder.UseSqlServer(connection);
            //Dit is loggen, het laat zien wat er gebeurt in de database(inclusief query), het is handig voor debugging
            //.LogTo(Console.WriteLine);//

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer -> Orders (0 to Many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)           // An Order can have 0 or 1 Customer
                .WithMany(c => c.Orders)           // A Customer has 0 to many Orders
                .HasForeignKey(o => o.CustomerId)  // FK in the Order table, nullable
                .OnDelete(DeleteBehavior.SetNull); // Set CustomerId to null if Customer is deleted

            // Order -> OrderLines (One-to-Many)
            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLines)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderLine -> Product (Many-to-One)
            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Product)
                .WithMany()
                .HasForeignKey(ol => ol.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
