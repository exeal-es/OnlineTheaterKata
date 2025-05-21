using Microsoft.AspNetCore.Mvc;
using OnlineTheater.Api.Models;
using OnlineTheater.Logic.Entities;
using OnlineTheater.Logic.Repositories;
using CSharpFunctionalExtensions;

namespace OnlineTheater.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(MovieRepository movieRepository, CustomerRepository customerRepository)
    : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CustomerDto> Get(long id)
    {
        Customer customer = customerRepository.GetById(id);
        if (customer == null)
        {
            return NotFound();
        }

        var customerstatusdto = (CustomerStatusDto)customer.Status;
        var purchasedmoviesdtos = customer.PurchasedMovies?
           .Select(x => new PurchasedMovieDto(x.MovieId, x.Movie.Name, x.Price, x.PurchaseDate, x.ExpirationDate))
            .ToList();

        var customerdto = new CustomerDto(customer.Id, customer.Name, customer.Email.Valor, customerstatusdto, customer.StatusExpirationDate, customer.MoneySpent, purchasedmoviesdtos);

        return Ok(customerdto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerBasicDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<CustomerBasicDto>> GetList()
    {
        IReadOnlyList<Customer> customers = customerRepository.GetList();

        var customersdtos = customers
            .Select(x => new CustomerBasicDto(x.Id, x.Name, x.Email.Valor, (CustomerStatusDto)x.Status, x.StatusExpirationDate, x.MoneySpent))
            .ToList();

        return Ok(customersdtos);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Create([FromBody] CreateCustomerDto item)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<Email> emailResult = Email.Create(item.Email);
            if (emailResult.IsFailure)
            {
                return BadRequest(emailResult.Error);
            }

            Email email = emailResult.Value;

            if (customerRepository.GetByEmail(email) != null)
            {
                return BadRequest("Email is already in use: " + item.Email);
            }

            var customer = new Customer(item.Name, email);
            customerRepository.Add(customer);
            customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Update(long id, [FromBody] UpdateCustomerDto item)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = customerRepository.GetById(id);
            if (customer == null)
            {
                return BadRequest("Invalid customer id: " + id);
            }

            customer.UpdateName(item.Name);
            customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpPost("{id}/movies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PurchaseMovie(long id, [FromBody] long movieId)
    {
        try
        {
            Movie movie = movieRepository.GetById(movieId);
            if (movie == null)
            {
                return BadRequest("Invalid movie id: " + movieId);
            }

            Customer customer = customerRepository.GetById(id);
            if (customer == null)
            {
                return BadRequest("Invalid customer id: " + id);
            }

            if (customer.PurchasedMovies.Any(x => x.MovieId == movie.Id && (x.ExpirationDate == null || x.ExpirationDate.Value >= DateTime.UtcNow)))
            {
                return BadRequest("The movie is already purchased: " + movie.Name);
            }

            customer.Purchase(movie);

            customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpPost("{id}/promotion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult PromoteCustomer(long id)
    {
        try
        {
            Customer customer = customerRepository.GetById(id);
            if (customer == null)
            {
                return BadRequest("Invalid customer id: " + id);
            }

            if (customer.Status == CustomerStatus.Advanced && (customer.StatusExpirationDate == null || customer.StatusExpirationDate.Value < DateTime.UtcNow))
            {
                return BadRequest("The customer already has the Advanced status");
            }

            bool success = customer.Promote();
            if (!success)
            {
                return BadRequest("Cannot promote the customer");
            }

            customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }
}