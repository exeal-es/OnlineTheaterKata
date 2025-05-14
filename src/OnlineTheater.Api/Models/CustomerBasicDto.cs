
namespace OnlineTheater.Api.Controllers;

public record CustomerDto( long Id, string Name, string Email, CustomerStatusDto Status, DateTime? StatusExpirationDate, decimal MoneySpent, List<PurchasedMovieDto> PurchasedMovies);


   
 
