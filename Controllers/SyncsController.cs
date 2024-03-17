using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformWellDataSync.Data;
using PlatformWellDataSync.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

// POST: api/Syncs/StartSync
[ApiController]
[Route("api/[controller]")]
public class SyncsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "http://test-demo.aemenersol.com";

    public SyncsController(AppDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }

    private async Task<string> GetTokenAsync()
    {
        var loginData = new { username = "user@aemenersol.com", password = "Test@123" };
        var loginResponse = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Account/login", loginData);
        loginResponse.EnsureSuccessStatusCode();
        var token = await loginResponse.Content.ReadAsStringAsync();
        return token.Trim('"');
    }


    [HttpPost("StartSync")]
    public async Task<IActionResult> StartSync([FromBody] SyncRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.EndpointUrl))
        {
            return BadRequest("A valid endpoint URL is required.");
        }

        var token = await GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync(request.EndpointUrl);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, $"Failed to fetch data from {request.EndpointUrl}.");
        }


        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, // Makes the deserializer case-insensitive
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Ignores null values during serialization
        };

        var platformsData = await response.Content.ReadFromJsonAsync<List<PlatformData>>(options);

        if (platformsData == null) return BadRequest("Failed to fetch or parse platform data.");

        foreach (var platformData in platformsData)
        {
            var platform = await _context.Platforms.Include(p => p.Wells)
                .FirstOrDefaultAsync(p => p.UniqueName == platformData.UniqueName);

            if (platform == null)
            {
                platform = new Platform
                {
                    UniqueName = platformData.UniqueName,
                    Latitude = platformData.Latitude,
                    Longitude = platformData.Longitude,
                    CreatedAt = platformData.CreatedAt,
                    UpdatedAt = platformData.UpdatedAt
                };
                _context.Platforms.Add(platform);
            }
            else
            {
                platform.Latitude = platformData.Latitude;
                platform.Longitude = platformData.Longitude;
                platform.CreatedAt = platformData.CreatedAt;
                platform.UpdatedAt = platformData.UpdatedAt;
            }

            await _context.SaveChangesAsync();

            foreach (var wellData in platformData.Well)
            {
                var well = platform.Wells.FirstOrDefault(w => w.UniqueName == wellData.UniqueName);

                if (well == null)
                {
                    well = new Well
                    {
                        PlatformId = platform.Id,
                        UniqueName = wellData.UniqueName,
                        Latitude = wellData.Latitude,
                        Longitude = wellData.Longitude,
                        CreatedAt = wellData.CreatedAt,
                        UpdatedAt = wellData.UpdatedAt
                    };
                    platform.Wells.Add(well);
                }
                else
                {
                    well.Latitude = wellData.Latitude;
                    well.Longitude = wellData.Longitude;
                    well.CreatedAt = wellData.CreatedAt;
                    well.UpdatedAt = wellData.UpdatedAt;
                }
            }
        }

        await _context.SaveChangesAsync();
        return Ok("Data synchronized successfully.");
    }

}
