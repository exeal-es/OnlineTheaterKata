using Microsoft.EntityFrameworkCore;
using OnlineTheater.Logic.Data;
using OnlineTheater.Logic.Repositories;

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

        // Add EF Core
        builder.Services.AddDbContext<OnlineTheaterDbContext>((sp, options) => options.UseSqlite(builder.Configuration["ConnectionString"]));
        
        // Add application services
        builder.Services.AddTransient<MovieRepository>();
        builder.Services.AddTransient<CustomerRepository>();

        var app = builder.Build();

        // Run migrations
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<OnlineTheaterDbContext>();
            if (db.Database.IsRelational()) 
            {
                db.Database.Migrate();
            }
            else
            {
                db.Database.EnsureCreated();
            }
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

