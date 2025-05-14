using Microsoft.AspNetCore.Mvc;
using OnlineTheater.Logic.Entities;
using OnlineTheater.Logic.Repositories;
using OnlineTheater.Logic.Services;

namespace OnlineTheater.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly MovieRepository _movieRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly CustomerService _customerService;

    public CustomersController(MovieRepository movieRepository, CustomerRepository customerRepository, CustomerService customerService)
    {
        _customerRepository = customerRepository;
        _movieRepository = movieRepository;
        _customerService = customerService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CustomerDto> Get(long id)
    {

        Customer customer = _customerRepository.GetById(id);
        if (customer == null)
        {
            return NotFound();
        }

        var customerstatusdto = (CustomerStatusDto)customer.Status;
        var purchasedmoviesdtos = customer.PurchasedMovies?
           .Select(x => new PurchasedMovieDto(x.Movie.Name, x.Price, x.PurchaseDate, x.ExpirationDate))
            .ToList();

        var customerdto = new CustomerDto(customer.Id, customer.Name, customer.Email, customerstatusdto, customer.StatusExpirationDate, customer.MoneySpent, purchasedmoviesdtos);

        return Ok(customer);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerBasicDto>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<CustomerBasicDto>> GetList()
    {
        IReadOnlyList<Customer> customers = _customerRepository.GetList();
        
        var customersdtos = customers
            .Select(x => new CustomerBasicDto( x.Id, x.Name, x.Email, (CustomerStatusDto)x.Status, x.StatusExpirationDate, x.MoneySpent))
            .ToList();

        return Ok(customersdtos);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Create([FromBody] Customer item)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_customerRepository.GetByEmail(item.Email) != null)
            {
                return BadRequest("Email is already in use: " + item.Email);
            }

            item.Id = 0;
            item.Status = CustomerStatus.Regular;
            _customerRepository.Add(item);
            _customerRepository.SaveChanges();

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
    public IActionResult Update(long id, [FromBody] Customer item)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return BadRequest("Invalid customer id: " + id);
            }

            customer.Name = item.Name;
            _customerRepository.SaveChanges();

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
            Movie movie = _movieRepository.GetById(movieId);
            if (movie == null)
            {
                return BadRequest("Invalid movie id: " + movieId);
            }

            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return BadRequest("Invalid customer id: " + id);
            }

            if (customer.PurchasedMovies.Any(x => x.MovieId == movie.Id && (x.ExpirationDate == null || x.ExpirationDate.Value >= DateTime.UtcNow)))
            {
                return BadRequest("The movie is already purchased: " + movie.Name);
            }

            _customerService.PurchaseMovie(customer, movie);

            _customerRepository.SaveChanges();

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
            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return BadRequest("Invalid customer id: " + id);
            }

            if (customer.Status == CustomerStatus.Advanced && (customer.StatusExpirationDate == null || customer.StatusExpirationDate.Value < DateTime.UtcNow))
            {
                return BadRequest("The customer already has the Advanced status");
            }

            bool success = _customerService.PromoteCustomer(customer);
            if (!success)
            {
                return BadRequest("Cannot promote the customer");
            }

            _customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }
}