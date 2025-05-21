using Newtonsoft.Json;

namespace OnlineTheater.Logic.Entities;

public class Movie : Entity
{
    public string Name { get; set; }

    [JsonIgnore]
    public LicensingModel LicensingModel { get; set; }

    public decimal CalculatePrice(Customer customer)
    {
        decimal price;
        switch (LicensingModel)
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

        if (customer.Status == CustomerStatus.Advanced && (customer.StatusExpirationDate == null || customer.StatusExpirationDate.Value >= DateTime.UtcNow))
        {
            price = price * 0.75m;
        }

        return price;
    }

    public DateTime? GetExpirationDate()
    {
        DateTime? result;

        switch (LicensingModel)
        {
            case LicensingModel.TwoDays:
                result = DateTime.UtcNow.AddDays(2);
                break;

            case LicensingModel.LifeLong:
                result = null;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        return result;
    }
}