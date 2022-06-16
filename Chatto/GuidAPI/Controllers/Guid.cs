using Microsoft.AspNetCore.Mvc;

namespace GuidAPI.Controllers;

[ApiController]
[Route("/api/Guid")]
public class Guid : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return Ok(System.Guid.NewGuid().ToString());
    }
}