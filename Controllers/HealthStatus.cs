
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace restratp.Controllers
{
    [Route("/")]
    public class HealthStatusController : Controller
    {
        /// <summary>
        /// Endpoint to check the service availability.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns code="200">Service is available.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}