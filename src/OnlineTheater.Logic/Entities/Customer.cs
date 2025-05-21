using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OnlineTheater.Logic.Entities;

public class Customer : Entity
{
    private Customer()
    {
        // For EF Core
    }

    public Customer(string name)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Name is too long", nameof(name));
        
        Name = name;
    }

    public string Name { get; protected set; }

    public Email Email { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public CustomerStatus Status { get; set; }

    public DateTime? StatusExpirationDate { get; set; }

    public decimal MoneySpent { get; set; }

    public List<PurchasedMovie> PurchasedMovies { get; set; } = [];

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        Name = name;
    }
}