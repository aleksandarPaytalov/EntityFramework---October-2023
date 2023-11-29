﻿namespace Artillery.Data;


using Models;
using Microsoft.EntityFrameworkCore;

public class ArtilleryContext : DbContext
{
    public ArtilleryContext() 
    { 
    }

    public ArtilleryContext(DbContextOptions options)
        : base(options) 
    { 
    }

    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<CountryGun> CountriesGuns { get; set; } = null!;
    public DbSet<Gun> Guns { get; set; } = null!;
    public DbSet<Manufacturer> Manufacturers { get; set; } = null!;
    public DbSet<Shell> Shells { get; set; } = null!;

     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseSqlServer(Configuration.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manufacturer>()
            .HasIndex(m => m.ManufacturerName)
            .IsUnique();

        modelBuilder.Entity<CountryGun>()
            .HasKey(cg => new
            {
                cg.CountryId,
                cg.GunId
            });
    }
}

