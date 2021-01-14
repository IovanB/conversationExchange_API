using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(UserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseAPIController : ControllerBase
    {
 
    }
}
