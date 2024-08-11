using CachingWebAPI.Data;
using CachingWebAPI.Models;
using CachingWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CachingWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly ILogger<DriverController> _logger;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _context;

        public DriverController(ICacheService cacheService, AppDbContext context, ILogger<DriverController> logger)
        {
            _cacheService = cacheService;
            _context = context; 
            _logger = logger;
        }


        [HttpGet("drivers")]
        public async Task<IActionResult> GetData()
        {
            //check cache data
            var cachedata = _cacheService.GetData<IEnumerable<Drivers>>("driver");
            if (cachedata != null && cachedata.Count() > 0) {
                return Ok(cachedata);
            }
            cachedata = await _context.Drivers.ToListAsync();

            //set expiry time
            var expirtyTime = DateTime.Now.AddSeconds(30);
            _cacheService.SetData<IEnumerable<Drivers>>("driver", cachedata, expirtyTime);
            return Ok(cachedata);
        }


        [HttpPost("AddDrivers")]
        public async Task<IActionResult> Post(Drivers value)
        {
            var addedObject = await _context.Drivers.AddAsync(value);

            var expirtyTime = DateTime.Now.AddSeconds(30);
            _cacheService.SetData<Drivers>($"driver{value.Id}", addedObject.Entity, expirtyTime);

            await _context.SaveChangesAsync();
            return Ok(addedObject.Entity);
        }


        [HttpDelete("DeleteDriver")]
        public async Task<IActionResult> Delate(int id)
        {
            var exist = await _context.Drivers.FirstOrDefaultAsync(a => a.Id == id);

            if (exist != null)
            {
                _context.Remove(exist);
                _cacheService.RemoveData($"driver{id}");
                await _context.SaveChangesAsync();

                return NoContent();
            }
            return NotFound();
        }
    }
}
