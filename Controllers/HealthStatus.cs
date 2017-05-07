
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace restratp.Controllers
{
    [Route("/")]
    public class HealthStatusController : Controller
    {
        // GET 
        public IActionResult Get()
        {
           return Ok();
        }
    }
}