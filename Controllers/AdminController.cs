using AutoMapper;
using Backend.Data;
using Backend.DTO.EngineerDto;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;

        public AdminController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPut("AssignEngineer/{requestId}")]
        public async Task<ActionResult> AssignEngineerToRequest(Guid requestId, [FromForm] Guid engineerId)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.Id == requestId);
            if (request == null)
            {
                return NotFound("Request Not Found");
            }

            var engineer = await _context.Engineers.FirstOrDefaultAsync(e => e.Id == engineerId);
            if (engineer == null)
            {
                return NotFound("Engineer Not Found");
            }

            // ✅ Business Rule: Only admin assigns engineers
            request.EngineerId = engineerId;
            request.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok("Engineer assigned successfully");
        }

        [HttpPost("service")]
        public async Task<ActionResult<Service>> CreateService([FromForm] CreateServiceDto service)
        {
            
            var newservice = _mapper.Map<Service>(service);

            _context.Services.Add(newservice);
            await _context.SaveChangesAsync();
            return Ok(newservice);
        }

        [HttpGet("service")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _context.Services.Where(x => x.IsDeleted == false).ToListAsync();

            return Ok(services);
        }

        [HttpGet("serviceRequest")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServiceRequests()
        {
            var servicerequests = await _context.ServiceRequests.Where(x => x.IsDeleted == false).ToListAsync();

            return Ok(servicerequests);
        }
    }
}
