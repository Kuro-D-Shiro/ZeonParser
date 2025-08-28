using Microsoft.AspNetCore.Mvc;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController(IZeonParser zeonParser) : ControllerBase
    {
        [HttpGet]
        public IActionResult Test()
        {
            zeonParser.Parse();
            return Ok();
        }
    }
}
