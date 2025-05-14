using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineTheater.Api;
using OnlineTheater.Logic.Data;
using OnlineTheater.Logic.Entities;

namespace OnlineTheater.AcceptanceTests;

public class CustomersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly long _movieId;

    public CustomersControllerTests(WebApplicationFactory<Program> factory)
    {
        var newFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptors = services.Where(
                    d => d.ServiceType == typeof(DbContextOptions<OnlineTheaterDbContext>)).ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }
                
                // Crear una instancia compartida de DbContextOptions
                var dbName = Guid.NewGuid().ToString();
                var optionsBuilder = new DbContextOptionsBuilder<OnlineTheaterDbContext>();
                optionsBuilder.UseInMemoryDatabase(dbName);

                var options = optionsBuilder.Options;

                // Registrar manualmente el contexto con estas opciones
                services.AddSingleton(options); // 👈 ESTA es la clave: misma instancia de options
                services.AddScoped<OnlineTheaterDbContext>();
            });
        });

        // Seed the database with at least one movie
        using (var scope = newFactory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OnlineTheaterDbContext>();
            context.Database.EnsureCreated();
            
            if (!context.Movies.Any())
            {
                var movie = new Movie
                {
                    Name = "Test Movie",
                    LicensingModel = LicensingModel.TwoDays
                };
                context.Movies.Add(movie);
                context.SaveChanges();

                _movieId = movie.Id;
            }
        }

        _client = newFactory.CreateClient();
    }

    private async Task<long> CreateCustomerAndGetId(Customer customer)
    {
        var createResponse = await _client.PostAsJsonAsync("/api/customers", customer);
        createResponse.EnsureSuccessStatusCode();
        
        // Get all customers to find the created one
        var getAllResponse = await _client.GetAsync("/api/customers");
        getAllResponse.EnsureSuccessStatusCode();
        var customers = await getAllResponse.Content.ReadFromJsonAsync<List<Customer>>();
        
        return customers.Last().Id;
    }

    [Fact]
    public async Task GetList_WhenNoCustomers_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/customers");
        
        // If we got a 500, read and display the error details
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Server error: {error}");
        }

        response.EnsureSuccessStatusCode();
        var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(customers);
        Assert.Empty(customers);
    }

    [Fact]
    public async Task Create_ValidCustomer_CanBeRetrieved()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            PurchasedMovies = new List<PurchasedMovie>()
        };

        // Act - Create customer
        var createResponse = await _client.PostAsJsonAsync("/api/customers", customer);
        
        // If we got an error, read and display the error details
        if (!createResponse.IsSuccessStatusCode)
        {
            var error = await createResponse.Content.ReadAsStringAsync();
            throw new Exception($"Server error: {error}");
        }

        createResponse.EnsureSuccessStatusCode();

        // Act - Get all customers
        var getAllResponse = await _client.GetAsync("/api/customers");
        getAllResponse.EnsureSuccessStatusCode();
        var customers = await getAllResponse.Content.ReadFromJsonAsync<List<Customer>>();

        // Assert
        Assert.NotNull(customers);
        Assert.Single(customers);
        Assert.Equal(customer.Name, customers[0].Name);
        Assert.Equal(customer.Email, customers[0].Email);
        Assert.Equal(CustomerStatus.Regular, customers[0].Status);
    }
    
    [Fact]
    public async Task Get_NonExistentCustomer_ReturnsNotFound()
    {
        // Arrange
        var nonExistentCustomerId = 999; // Un ID que no existe en la base de datos
    
        // Act
        var response = await _client.GetAsync($"/api/customers/{nonExistentCustomerId}");
    
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ExistingCustomer_ReturnsCustomer()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "Jane Doe",
            Email = "jane.doe@example.com",
            PurchasedMovies = new List<PurchasedMovie>()
        };

        // Create customer and get ID
        var customerId = await CreateCustomerAndGetId(customer);

        // Act - Get customer by ID
        var getByIdResponse = await _client.GetAsync($"/api/customers/{customerId}");
        
        // If we got a 500, read and display the error details
        if (getByIdResponse.StatusCode == HttpStatusCode.InternalServerError)
        {
            var error = await getByIdResponse.Content.ReadAsStringAsync();
            throw new Exception($"Server error: {error}");
        }

        getByIdResponse.EnsureSuccessStatusCode();
        var retrievedCustomer = await getByIdResponse.Content.ReadFromJsonAsync<Customer>();

        // Assert
        Assert.NotNull(retrievedCustomer);
        Assert.Equal(customerId, retrievedCustomer.Id);
        Assert.Equal(customer.Name, retrievedCustomer.Name);
        Assert.Equal(customer.Email, retrievedCustomer.Email);
        Assert.Equal(CustomerStatus.Regular, retrievedCustomer.Status);
    }

    [Fact]
    public async Task PromoteCustomer_CustomerNotFound_ReturnsNotFound()
    {
        // Arrange
        var nonExistentCustomerId = 999; // Un ID que no existe en la base de datos
    
        // Act
        var response = await _client.PostAsync($"/api/customers/{nonExistentCustomerId}/promote", null);
    
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PurchaseMovie_InvalidMovieId_ReturnsNotFound()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            PurchasedMovies = new List<PurchasedMovie>()
        };
    
        // Crear un cliente válido
        var customerId = await CreateCustomerAndGetId(customer);
    
        var invalidMovieId = -1; // ID de película inválido
    
        // Act
        var response = await _client.PostAsync($"/api/customers/{customerId}/movies/{invalidMovieId}/purchase", null);
    
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidCustomer_UpdatesNameSuccessfully()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "Original Name",
            Email = "original.email@example.com",
            PurchasedMovies = new List<PurchasedMovie>()
        };
    
        // Crear un cliente válido y obtener su ID
        var customerId = await CreateCustomerAndGetId(customer);
    
        // Act - Actualizar el nombre del cliente
        var updatedCustomer = new Customer
        {
            Name = "Updated Name",
            Email = customer.Email, // El email debe permanecer igual
            PurchasedMovies = customer.PurchasedMovies
        };
    
        var updateResponse = await _client.PutAsJsonAsync($"/api/customers/{customerId}", updatedCustomer);
    
        // Assert - Verificar que la actualización fue exitosa
        updateResponse.EnsureSuccessStatusCode();
    
        // Act - Obtener el cliente actualizado
        var getResponse = await _client.GetAsync($"/api/customers/{customerId}");
        getResponse.EnsureSuccessStatusCode();
        var retrievedCustomer = await getResponse.Content.ReadFromJsonAsync<Customer>();
    
        // Assert - Verificar que el nombre fue actualizado correctamente
        Assert.NotNull(retrievedCustomer);
        Assert.Equal("Updated Name", retrievedCustomer.Name);
        Assert.Equal(customer.Email, retrievedCustomer.Email);
    }

    [Fact]
    public async Task Update_InvalidCustomerId_ReturnsBadRequest()
    {
        // Arrange
        var invalidCustomerId = -1; // ID de cliente inválido
        var updatedCustomer = new Customer
        {
            Name = "Updated Name",
            Email = "updated.email@example.com",
            PurchasedMovies = new List<PurchasedMovie>()
        };
    
        // Act
        var response = await _client.PutAsJsonAsync($"/api/customers/{invalidCustomerId}", updatedCustomer);
    
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "duplicate.email@example.com",
            PurchasedMovies = new List<PurchasedMovie>()
        };
    
        // Crear el primer cliente con el correo electrónico
        var createResponse1 = await _client.PostAsJsonAsync("/api/customers", customer);
        createResponse1.EnsureSuccessStatusCode();
    
        // Intentar crear un segundo cliente con el mismo correo electrónico
        var createResponse2 = await _client.PostAsJsonAsync("/api/customers", customer);
    
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, createResponse2.StatusCode);
    }
    
    [Fact]
    public async Task PurchaseMovie_ValidMovieId_AddsMovieToPurchasedMovies()
    {
        // Arrange
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            PurchasedMovies = new List<PurchasedMovie>()
        };

        // Crear un cliente válido y obtener su ID
        var customerId = await CreateCustomerAndGetId(customer);

        // Obtener el ID de una película existente
        var movieId = _movieId;

        // Act - Comprar la película
        var purchaseResponse = await _client.PostAsJsonAsync($"/api/customers/{customerId}/movies", movieId);
        if (!purchaseResponse.IsSuccessStatusCode)
        {
            var error = await purchaseResponse.Content.ReadAsStringAsync();
            throw new Exception($"Server error: {error}");
        }

        // Act - Obtener el cliente actualizado
        var getResponse = await _client.GetAsync($"/api/customers/{customerId}");
        getResponse.EnsureSuccessStatusCode();
        var updatedCustomer = await getResponse.Content.ReadFromJsonAsync<Customer>();

        // Assert - Verificar que la película está en la lista de películas compradas
        Assert.NotNull(updatedCustomer);
        Assert.Contains(updatedCustomer.PurchasedMovies, pm => pm.MovieId == movieId);
    }
} 