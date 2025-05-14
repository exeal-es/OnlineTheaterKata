using Microsoft.EntityFrameworkCore;
using OnlineTheater.Logic.Entities;

namespace OnlineTheater.Logic.Data;

public class OnlineTheaterDbContext(DbContextOptions<OnlineTheaterDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<PurchasedMovie> PurchasedMovies { get; set; }
} 