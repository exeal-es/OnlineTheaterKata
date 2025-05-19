using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace OnlineTheater.Logic.Entities;

public class Customer : Entity
{
    [Required]
    [MaxLength(100, ErrorMessage = "Name is too long")]
    public string Name { get; set; }

    public Email Email { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public CustomerStatus Status { get; set; }

    public DateTime? StatusExpirationDate { get; set; }

    public decimal MoneySpent { get; set; }

    public List<PurchasedMovie> PurchasedMovies { get; set; } = [];
}