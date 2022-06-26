using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChattoAuth.Controllers;

[ApiController]
[Route("")]
public class HomeController : Controller
{
    // GET
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(User);
    }
}