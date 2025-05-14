using Microsoft.EntityFrameworkCore;
using OnlineTheater.Logic.Data;
using OnlineTheater.Logic.Repositories;
using OnlineTheater.Logic.Services;

namespace OnlineTheater.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add EF Core and custom services
        builder.Services.AddDbContext<OnlineTheaterDbContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connectionString = config["ConnectionString"];
            options.UseSqlite(connectionString);
        });
        
        // Add application services
        builder.Services.AddTransient<MovieRepository>();
        builder.Services.AddTransient<CustomerRepository>();
        builder.Services.AddTransient<MovieService>();
        builder.Services.AddTransient<CustomerService>();

        var app = builder.Build();

        // Run migrations
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<OnlineTheaterDbContext>();
            db.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}

