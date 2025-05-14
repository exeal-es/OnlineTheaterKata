using Newtonsoft.Json;

namespace OnlineTheater.Logic.Entities;

public class PurchasedMovie : Entity
{
    [JsonIgnore]
    public long MovieId { get; set; }

    public Movie Movie { get; set; }

    [JsonIgnore]
    public long CustomerId { get; set; }

    public decimal Price { get; set; }

    public DateTime PurchaseDate { get; set; }

    public DateTime? ExpirationDate { get; set; }
}