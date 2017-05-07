
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace restratp.Controllers
{
    [Route("/")]
    public class HealthStatusController : Controller
    {
        // GET 
        [HttpGet]
        public IActionResult Get()
        {
           return Ok();
        }
    }
}