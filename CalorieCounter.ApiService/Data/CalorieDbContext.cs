using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace CalorieCounter.ApiService;

public class CalorieDbContext : IdentityDbContext<User, IdentityRole<int>,int>
{
    public CalorieDbContext(DbContextOptions<CalorieDbContext> options) : base(options)
    {
    }

    public DbSet<FoodItem> FoodItems => Set<FoodItem>();
    public DbSet<FoodEntry> FoodEntries => Set<FoodEntry>();
    public DbSet<BarcodeEntry> BarcodeEntries => Set<BarcodeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        /*
         * FoodItem
         */

        modelBuilder.Entity<FoodItem>()
                    .HasIndex(x=> x.InternalId)
                    .IsUnique();

        /*
         * BarcodeEntry
e       */

        modelBuilder.Entity<BarcodeEntry>()
                    .HasIndex(x=> x.Code)
                    .IsUnique();

        modelBuilder.Entity<BarcodeEntry>()
                    .HasOne(x=> x.FoodItem)
                    .WithMany()
                    .HasPrincipalKey(x=> x.InternalId)
                    .HasForeignKey(x=> x.FoodItemId);

        /*
         * FoodEntry
        */

        modelBuilder.Entity<FoodEntry>()
                    .HasIndex(x=> x.PublicIdentifier)
                    .IsUnique();

        modelBuilder.Entity<FoodEntry>()
                    .HasOne(x=> x.User)
                    .WithMany()
                    .HasForeignKey(x=> x.UserId);

        modelBuilder.Entity<FoodEntry>()
                    .HasOne(x=> x.FoodItem)
                    .WithMany()
                    .HasForeignKey(x=> x.FoodItemId)
                    .HasPrincipalKey(x=> x.InternalId);

    }

}
