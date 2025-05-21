namespace OnlineTheater.Logic.Entities;

public class Customer : Entity
{
    private Customer()
    {
        // For EF Core
    }

    public Customer(string name, Email email)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Name is too long", nameof(name));

        Id = 0;
        Name = name;
        Email = email;
        Status = CustomerStatus.Regular;
        StatusExpirationDate = null;
    }

    public string Name { get; protected set; }

    public Email Email { get; protected set; }

    public CustomerStatus Status { get; protected set; }

    public DateTime? StatusExpirationDate { get; protected set; }

    public decimal MoneySpent { get; set; }

    public List<PurchasedMovie> PurchasedMovies { get; set; } = [];

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        Name = name;
    }

    public bool Promote()
    {
        // at least 2 active movies during the last 30 days
        if (PurchasedMovies.Count(x => x.ExpirationDate == null || x.ExpirationDate.Value >= DateTime.UtcNow.AddDays(-30)) < 2)
            return false;

        // at least 100 dollars spent during the last year
        if (PurchasedMovies.Where(x => x.PurchaseDate > DateTime.UtcNow.AddYears(-1)).Sum(x => x.Price) < 100m)
            return false;

        Status = CustomerStatus.Advanced;
        StatusExpirationDate = DateTime.UtcNow.AddYears(1);

        return true;
    }
}