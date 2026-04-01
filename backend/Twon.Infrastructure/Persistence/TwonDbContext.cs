using Microsoft.EntityFrameworkCore;
using Twon.Domain.Enums;
using DomainEntities = Twon.Domain.Entities;

namespace Twon.Infrastructure.Persistence;

public class TwonDbContext(DbContextOptions<TwonDbContext> options) : DbContext(options)
{
    public DbSet<DomainEntities.User> Users => Set<DomainEntities.User>();
    public DbSet<DomainEntities.RefreshToken> RefreshTokens => Set<DomainEntities.RefreshToken>();
    public DbSet<DomainEntities.Product> Products => Set<DomainEntities.Product>();
    public DbSet<DomainEntities.Order> Orders => Set<DomainEntities.Order>();
    public DbSet<DomainEntities.OrderItem> OrderItems => Set<DomainEntities.OrderItem>();
    public DbSet<DomainEntities.Payment> Payments => Set<DomainEntities.Payment>();
    public DbSet<DomainEntities.PaymentConfig> PaymentConfigs => Set<DomainEntities.PaymentConfig>();
    public DbSet<DomainEntities.LibraryItem> LibraryItems => Set<DomainEntities.LibraryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<DomainEntities.User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Role).HasConversion<string>();
        });

        // RefreshToken
        modelBuilder.Entity<DomainEntities.RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.TokenHash).IsUnique();
            e.HasOne(x => x.User).WithMany(u => u.RefreshTokens)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Product
        modelBuilder.Entity<DomainEntities.Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ProductType).HasConversion<string>();
        });

        // Order
        modelBuilder.Entity<DomainEntities.Order>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<string>();
            e.HasOne(x => x.User).WithMany(u => u.Orders)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        // OrderItem
        modelBuilder.Entity<DomainEntities.OrderItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Order).WithMany(o => o.OrderItems)
                .HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        // Payment
        modelBuilder.Entity<DomainEntities.Payment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<string>();
            e.HasOne(x => x.Order).WithOne(o => o.Payment)
                .HasForeignKey<DomainEntities.Payment>(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
        });

        // PaymentConfig — singleton row
        modelBuilder.Entity<DomainEntities.PaymentConfig>(e =>
        {
            e.HasKey(x => x.Id);
        });

        // LibraryItem
        modelBuilder.Entity<DomainEntities.LibraryItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.User).WithMany(u => u.LibraryItems)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Product).WithMany(p => p.LibraryItems)
                .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
