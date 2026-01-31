using AutoMapper;
using Backend.Data;
using Backend.DTO.EngineerDto;
using Backend.DTO.ReviewDto;
using Backend.DTO.ServiceRequestDto;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;

    public CustomerController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
        [HttpGet("service")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _context.Services.Where(x => x.IsDeleted == false).ToListAsync();

            return Ok(services);
        }

        [HttpGet("Requests")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServiceRequests()
        {
            var servicerequests = await _context.ServiceRequests.Where(x => x.IsDeleted == false).ToListAsync();

            return Ok(servicerequests);
        }

        [HttpPost("request")]
        public async Task<ActionResult<ServiceRequest>> CreateServiceRequest([FromForm] CreateServiceRequestDto requestDto)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == requestDto.ServiceId);

            if (service == null)
            {
                return NotFound("Service Not Found");
            }

            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newRequest = _mapper.Map<ServiceRequest>(requestDto);
            newRequest.CustomerId = Guid.Parse(customerId);
            newRequest.EstimatedPrice = service.BasePrice * requestDto.AcCount;
            newRequest.CreatedAt = DateTime.Now;
            newRequest.Status = "Pending";

            _context.ServiceRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return Ok(newRequest);
        }

        [HttpPost("Review")]
        public async Task<ActionResult<Review>> CreateReview([FromForm] CreateReviewDto reviewDto)
        {
            // ✅ Business Rule: Review only after completion
            var request = await _context.ServiceRequests
                .FirstOrDefaultAsync(r => r.Id == reviewDto.ProductId);

            if (request == null)
            {
                return NotFound("Service Request Not Found");
            }

            if (request.Status != "Completed")
            {
                return BadRequest("Can only review after service is completed");
            }

            var newReview = _mapper.Map<Review>(reviewDto);
            newReview.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            newReview.CreatedAt = DateTime.Now;

            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();

            return Ok(newReview);
        }

        [HttpGet("{RequestId}")]
        public async Task<ActionResult<ServiceRequest>> GetMyRequest(Guid RequestId)
        {
            var request = await _context.ServiceRequests.Where(x => x.IsDeleted == false).FirstOrDefaultAsync(c => c.CustomerId == RequestId);
            return Ok(request);
        }
    }
}
