using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class HttpErrorController : BaseAPIController
    {
        private readonly DataContext dataContext;

        public HttpErrorController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [Authorize]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return Unauthorized();
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = dataContext.Users.Find(-1);

            if (thing == null) return NotFound();

            return NotFound();
        }
 

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing = dataContext.Users.Find(-1);

            var thingToReturn = thing.ToString();

            return thingToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest();
        }
    }
}
