using DAJDAJ.Entities;
using DAJDAJ.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAJDAJ.DataAccess
{
    public class ApplicationDbContext:IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
        }
        public DbSet<Category> categories { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Shoppingcart> Shoppingcarts { get; set; }
        public DbSet<ProductColorStock> ProductColorStocks { get; set; }
        public DbSet<ProductColorSizeStock> ProductColorSizeStocks { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<EmailOtp> EmailOtps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique index on Email to prevent duplicate emails
            modelBuilder.Entity<EmailOtp>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_EmailOtps_Email_Unique");
            });

            // Unique constraint for ProductColorSizeStock - one record per Product+Color+Size combination
            modelBuilder.Entity<ProductColorSizeStock>(entity =>
            {
                entity.HasIndex(e => new { e.ProductId, e.Color, e.Size })
                    .IsUnique()
                    .HasDatabaseName("IX_ProductColorSizeStock_ProductId_Color_Size_Unique");
            });
        }
    }
}
