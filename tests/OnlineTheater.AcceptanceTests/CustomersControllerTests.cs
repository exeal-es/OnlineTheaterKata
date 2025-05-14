using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using OnlineTheater.Api;
using OnlineTheater.Logic.Entities;

namespace OnlineTheater.AcceptanceTests;

public class CustomersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private const string TestDbPath = "OnlineTheater.Test.db";

    public CustomersControllerTests(WebApplicationFactory<Program> factory)
    {
        // Delete test database if it exists
        if (File.Exists(TestDbPath))
        {
            File.Delete(TestDbPath);
        }

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionString"] = $"Data Source={TestDbPath};Version=3;"
                });
            });
        });

        _client = _factory.CreateClient();
    }

    private async Task<long> CreateCustomerAndGetId(Customer customer)
    {
        var createResponse = await _client.PostAsJsonAsync("/api/customers", customer);
        createResponse.EnsureSuccessStatusCode();
        
        // Get all customers to find the created one
        var getAllResponse = await _client.GetAsync("/api/customers");
        getAllResponse.EnsureSuccessStatusCode();
        var customers = await getAllResponse.Content.ReadFromJsonAsync<List<Customer>>();
        
        return customers.LastOrDefault()?.Id ?? 1;
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
        
        // If we got a 500, read and display the error details
        if (createResponse.StatusCode == HttpStatusCode.InternalServerError)
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
    public async Task PromoteCustomer_CustomerNotFound_ReturnsBadRequest()
    {
    }

    [Fact]
    public async Task PurchaseMovie_InvalidMovieId_ReturnsBadRequest()
    {
    }

    [Fact]
    public async Task PurchaseMovie_InvalidCustomerId_ReturnsBadRequest()
    {
    }

    [Fact]
    public async Task Update_ValidCustomer_UpdatesNameSuccessfully()
    {
    }

    [Fact]
    public async Task Update_InvalidCustomerId_ReturnsBadRequest()
    {
    }

    [Fact]
    public async Task Create_DuplicateEmail_ReturnsBadRequest()
    {
    }

    [Fact]
    public async Task Create_FirstCustomer_WithEmail_Success()
    {
    }

    [Fact]
    public async Task Get_NonExistentCustomer_ReturnsNotFound()
    {
    }

    [Fact]
    public async Task Create_InvalidModelState_ReturnsBadRequest()
    {
    }

    [Fact]
    public async Task Update_InvalidModelState_ReturnsBadRequest()
    {
    }
} 