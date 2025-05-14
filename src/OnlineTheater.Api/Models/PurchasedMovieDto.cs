
namespace OnlineTheater.Api.Controllers;

public record PurchasedMovieDto ( string Name, decimal Price, DateTime PurchaseDate, DateTime? ExpirationDate);
