using OnlineTheater.Logic.Repositories;
using OnlineTheater.Logic.Services;
using OnlineTheater.Logic.Utils;

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

        // Add NHibernate and custom services
        builder.Services.AddSingleton<SessionFactory>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connectionString = config["ConnectionString"];
            return new SessionFactory(connectionString);
        });
        builder.Services.AddScoped<UnitOfWork>();
        builder.Services.AddTransient<MovieRepository>();
        builder.Services.AddTransient<CustomerRepository>();
        builder.Services.AddTransient<MovieService>();
        builder.Services.AddTransient<CustomerService>();

        var app = builder.Build();

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
