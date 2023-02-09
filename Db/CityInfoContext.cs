using Microsoft.EntityFrameworkCore;
using CityInfoAPI.Entities;

namespace CityInfoAPI.Db {
  public class CityInfoContext : DbContext {
    public DbSet<City> Cities { get; set; } = null!;

    public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

    public CityInfoContext(DbContextOptions<CityInfoContext> opts) : base(opts) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.Entity<City>().HasData(
        new City("Nairobi") {
          Id = 1,
          Description = "Capital City of Kenya"
        },
        new City("Eldoret") {
          Id = 2,
          Description = "Home of Champions"
        },
        new City("Mombasa") {
          Id = 3,
          Description = "Oldest town in Kenya"
        },
        new City("Kisumu") {
          Id = 4,
          Description = "Oldest town in Kenya"
        }
      );
      modelBuilder.Entity<PointOfInterest>().HasData(
        new PointOfInterest("Nairobi National Park") {
          Id = 1,
          CityId = 1,
          Description = "Location at the middle of the city"
        },
        new PointOfInterest("KICC") {
          Id = 2,
          CityId = 1,
          Description = "Largest Internation conference center"
        },
        new PointOfInterest("Ukunda") {
          Id = 3,
          CityId = 2,
          Description = "Best tourist location in Mombasa"
        },
         new PointOfInterest("Fort Jesus") {
          Id = 4,
          CityId = 2,
          Description = "Oldest building used by arabs to trade"
        },
         new PointOfInterest("Kerio Valley") {
          Id = 5,
          CityId = 3,
          Description = "Best land mark in Kenya"
        }
      );
      base.OnModelCreating(modelBuilder);
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    //   optionsBuilder.UseSqlServer("connectionString");
    //   base.OnConfiguring(optionsBuilder);
    // }
  }
}