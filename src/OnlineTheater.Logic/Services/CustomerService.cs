using OnlineTheater.Logic.Entities;

namespace OnlineTheater.Logic.Services;

public class CustomerService(MovieService movieService)
{
    private decimal CalculatePrice(CustomerStatus status, DateTime? statusExpirationDate, LicensingModel licensingModel)
    {
        decimal price;
        switch (licensingModel)
        {
            case LicensingModel.TwoDays:
                price = 4;
                break;

            case LicensingModel.LifeLong:
                price = 8;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        if (status == CustomerStatus.Advanced && (statusExpirationDate == null || statusExpirationDate.Value >= DateTime.UtcNow))
        {
            price = price * 0.75m;
        }

        return price;
    }

    public void PurchaseMovie(Customer customer, Movie movie)
    {
        DateTime? expirationDate = movieService.GetExpirationDate(movie.LicensingModel);
        decimal price = CalculatePrice(customer.Status, customer.StatusExpirationDate, movie.LicensingModel);

        var purchasedMovie = new PurchasedMovie
        {
            MovieId = movie.Id,
            CustomerId = customer.Id,
            ExpirationDate = expirationDate,
            Price = price
        };

        customer.PurchasedMovies.Add(purchasedMovie);
        customer.MoneySpent += price;
    }
}