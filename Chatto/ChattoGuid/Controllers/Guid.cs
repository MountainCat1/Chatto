using Microsoft.AspNetCore.Mvc;

namespace GuidAPI.Controllers;

[ApiController]
[Route("/api/Guid")]
public class GuidController : Controller
{

    private readonly ILogger<GuidController> _logger;

    public GuidController(ILogger<GuidController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var guid = Guid.NewGuid().ToString();
        _logger.LogInformation($"Sending new guid... {guid}");
        return Ok(guid);
    }
}