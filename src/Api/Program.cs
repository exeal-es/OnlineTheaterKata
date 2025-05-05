using Logic.Repositories;
using Logic.Services;
using Logic.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add NHibernate and custom services
builder.Services.AddSingleton(new SessionFactory(builder.Configuration["ConnectionString"]));
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
