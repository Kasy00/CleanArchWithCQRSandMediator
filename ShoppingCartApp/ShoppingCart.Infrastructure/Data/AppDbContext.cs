using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CartEntity> Carts { get; set; }
    public DbSet<CartItemEntity> CartItems { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }

    protected override void OodelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartEntity>()
            .HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartEntity>()
            .HasIndex(c => new { c.UserId, c.Status });
    }
}