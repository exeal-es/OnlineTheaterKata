namespace OnlineTheater.Api.Controllers;

public record CustomerBasicDto(long Id, string Name, string Email, CustomerStatusDto Status, DateTime? StatusExpirationDate, decimal MoneySpent);