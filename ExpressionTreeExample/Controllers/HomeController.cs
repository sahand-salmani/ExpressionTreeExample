using ExpressionTreeExample.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ExpressionTreeExample.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HomeController : MyController
    {
        [HttpGet]
        public IActionResult Index(string text)
        {
            // From Extension method
            return this.RedirectTo<OtherController>(controller => controller.Destination(text));
        }
        [HttpGet]
        public IActionResult Index2(int number)
        {
            // From custom MyController and inheritance
            return RedirectToAnotherAction<OtherController>(controller => controller.Destination2(number));
        }

       
    }
}
