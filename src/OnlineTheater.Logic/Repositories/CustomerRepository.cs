using Microsoft.EntityFrameworkCore;
using OnlineTheater.Logic.Data;
using OnlineTheater.Logic.Entities;

namespace OnlineTheater.Logic.Repositories;

public class CustomerRepository(OnlineTheaterDbContext context)
{
    public IReadOnlyList<Customer> GetList()
    {
        return context.Customers
            .Select(x => new Customer
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Status = x.Status,
                StatusExpirationDate = x.StatusExpirationDate,
                MoneySpent = x.MoneySpent
            })
            .ToList();
    }

    public Customer GetByEmail(Email email)
    {
        return context.Customers
            .FirstOrDefault(x => x.Email == email);
    }

    public void Add(Customer item)
    {
        context.Customers.Add(item);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }

    public Customer GetById(long id)
    {
        return context.Customers
            .Include(c => c.PurchasedMovies).ThenInclude(pm => pm.Movie)
            .FirstOrDefault(c => c.Id == id);
    }
}