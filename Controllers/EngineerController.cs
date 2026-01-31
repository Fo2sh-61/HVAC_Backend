using AutoMapper;
using Backend.Data;
using Backend.DTO.EngineerDto;
using Backend.DTO.ServiceRequestDto;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EngineerController : ControllerBase
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;

        public EngineerController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        

        [HttpGet("serviceRequest")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServiceRequests()
        {
            var servicerequests = await _context.ServiceRequests.Where(x => x.IsDeleted == false).ToListAsync();

            return Ok(servicerequests);
        }

        [HttpPut("requests/{id}/status")]
        public async Task<ActionResult> UpdateRequestStatus(Guid id, [FromForm] UpdateServiceRequestStatusDto updatedrequest)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(c => c.Id == id);
            if (request == null)
            {
                return NotFound("Request Not Found");
            }

            request.UpdatedAt = DateTime.Now;

            _mapper.Map(updatedrequest, request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("requests/{id}/price")]
        public async Task<ActionResult> UpdateRequestFinalPrice(Guid id, [FromForm] UpdateServiceRequestPriceDto updatedrequest)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(c => c.Id == id);
            if (request == null)
            {
                return NotFound("Request Not Found");
            }

            request.UpdatedAt = DateTime.Now;

            _mapper.Map(updatedrequest, request);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
