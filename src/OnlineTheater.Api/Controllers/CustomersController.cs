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
    public ActionResult<Customer> Get(long id)
    {
        Customer customer = _customerRepository.GetById(id);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Customer>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyList<Customer>> GetList()
    {
        IReadOnlyList<Customer> customers = _customerRepository.GetList();
        return Ok(customers);
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

    [HttpPost("{id}/promotion")]
    public async Task PromoteCustomer_ValidCustomer_SuccessfulPromotion()
    {
        // Implementation of the method
    }

    [HttpPost("{id}/promotion")]
    public async Task PromoteCustomer_CustomerNotFound_ReturnsBadRequest()
    {
        // Implementation of the method
    }

    [HttpPost("{id}/promotion")]
    public async Task PromoteCustomer_CustomerAlreadyAdvanced_ReturnsBadRequest()
    {
        // Implementation of the method
    }

    [HttpPost("{id}/promotion")]
    public async Task PromoteCustomer_InsufficientMoviesPurchased_ReturnsBadRequest()
    {
        // Implementation of the method
    }

    [HttpPost("{id}/movies")]
    public async Task PurchaseMovie_ValidCustomerAndMovie_SuccessfulPurchase()
    {
        // Implementation of the method
    }

    [HttpPost("{id}/movies")]
    public async Task PurchaseMovie_InvalidMovieId_ReturnsBadRequest()
    {
        // Implementation of the method
    }

    [HttpPost("{id}/movies")]
    public async Task PurchaseMovie_InvalidCustomerId_ReturnsBadRequest()
    {
        // Implementation of the method
    }

    [HttpPost("{id}/movies")]
    public async Task PurchaseMovie_MovieAlreadyPurchased_ReturnsBadRequest()
    {
        // Implementation of the method
    }

    [HttpPost("{id}/movies")]
    public async Task PurchaseMovie_ExpiredMoviePurchase_AllowsRepurchase()
    {
        // Implementation of the method
    }

    [HttpPut("{id}")]
    public async Task Update_ValidCustomer_UpdatesNameSuccessfully()
    {
        // Implementation of the method
    }

    [HttpPut("{id}")]
    public async Task Update_InvalidCustomerId_ReturnsBadRequest()
    {
        // Implementation of the method
    }

    [HttpPut("{id}")]
    public async Task Update_InvalidModelState_ReturnsBadRequest()
    {
        // Implementation of the method
    }

    public async Task Create_DuplicateEmail_ReturnsBadRequest()
    {
        // Implementation of the method
    }

    public async Task Create_InvalidModelState_ReturnsBadRequest()
    {
        // Implementation of the method
    }

    public async Task Create_ExceptionThrown_Returns500()
    {
        // Implementation of the method
    }

    public async Task Get_NonExistentCustomer_ReturnsNotFound()
    {
        // Implementation of the method
    }
}