namespace OnlineTheater.Api.Controllers;

public record PurchasedMovieDto(long MovieId, string Name, decimal Price, DateTime PurchaseDate, DateTime? ExpirationDate);