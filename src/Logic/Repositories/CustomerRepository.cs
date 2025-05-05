﻿using Logic.Entities;
using Logic.Utils;

namespace Logic.Repositories;

public class CustomerRepository(UnitOfWork unitOfWork) : Repository<Customer>(unitOfWork)
{
    public IReadOnlyList<Customer> GetList()
    {
        return _unitOfWork
            .Query<Customer>()
            .ToList()
            .Select(x =>
            {
                x.PurchasedMovies = null;
                return x;
            })
            .ToList();
    }

    public Customer GetByEmail(string email)
    {
        return _unitOfWork
            .Query<Customer>()
            .SingleOrDefault(x => x.Email == email);
    }
}