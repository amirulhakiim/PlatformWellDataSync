using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformWellDataSync.Data;
using PlatformWellDataSync.Models;

[Route("api/[controller]")]
[ApiController]
public class GetPlatformWell : ControllerBase
{
    private readonly AppDbContext _context;

    public GetPlatformWell(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/PlatformWell/Platforms
    [HttpGet("Platforms")]
    public async Task<ActionResult<IEnumerable<Platform>>> GetPlatforms()
    {
        return await _context.Platforms.ToListAsync();
    }

    // GET: api/PlatformWell/Wells
    [HttpGet("Wells")]
    public async Task<ActionResult<IEnumerable<Well>>> GetWells()
    {
        return await _context.Wells.ToListAsync();
    }
}
