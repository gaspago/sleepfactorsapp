using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SleepFactorsApp.Domain;

namespace SleepFactorsApp.Data;

public sealed class SleepFactorsDbContext(DbContextOptions<SleepFactorsDbContext> options) : IdentityDbContext(options)
{
    public DbSet<DailyLog> DailyLogs => Set<DailyLog>();
    public DbSet<FactorBase> Factors => Set<FactorBase>();
    public DbSet<SimpleFactor> SimpleFactors => Set<SimpleFactor>();
    public DbSet<CompositeFactor> CompositeFactors => Set<CompositeFactor>();
    public DbSet<MealFactor> MealFactors => Set<MealFactor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DailyLog>()
            .HasMany(log => log.Factors)
            .WithOne(factor => factor.DailyLog)
            .HasForeignKey(factor => factor.DailyLogId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FactorBase>()
            .HasDiscriminator<string>("FactorType")
            .HasValue<SimpleFactor>("simple")
            .HasValue<CompositeFactor>("composite")
            .HasValue<MealFactor>("meal");

        modelBuilder.Entity<FactorBase>()
            .Property(factor => factor.Name)
            .HasMaxLength(120);

        modelBuilder.Entity<FactorBase>()
            .Property(factor => factor.Detail)
            .HasMaxLength(300);

        modelBuilder.Entity<FactorBase>()
            .HasMany(factor => factor.Children)
            .WithOne(factor => factor.ParentFactor)
            .HasForeignKey(factor => factor.ParentFactorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MealFactor>()
            .Property(meal => meal.MealType)
            .HasMaxLength(40);
    }
}
