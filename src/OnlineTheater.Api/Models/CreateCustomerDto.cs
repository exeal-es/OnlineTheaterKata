namespace OnlineTheater.Api.Models
{
    public record CreateCustomerDto(string Name, string Email);

    public record UpdateCustomerDto(string Name);
}