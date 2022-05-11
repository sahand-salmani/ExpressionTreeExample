using Microsoft.AspNetCore.Mvc;

namespace ExpressionTreeExample.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OtherController : ControllerBase
    {
        [HttpGet]
        public IActionResult Destination(string text)
        {
            return Ok($"This was called from Destination action and Other controller\n{text}");
        }

        [HttpGet]
        public IActionResult Destination2(int number)
        {
            return Ok($"This was called from Destination action and Other controller\n{number}");
        }
    }
}
