
namespace OnlineTheater.Api.Controllers;

public record CustomerDto( string Name, CustomerStatusDto Status, DateTime? StatusExpirationDate, decimal MoneySpent, List<PurchasedMovieDto> PurchasedMovies);


   
 
