using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class CacheController : ControllerBase
    {
        ISystemTime Clock;

        public CacheController(ISystemTime clock)
        {
            Clock = clock;
        }

        // This is "output caching".  This is all we can do for this.
        // If you hit this in Postman, Postman will not cache it and still return the current time.
        // We add a response header, and the client gets to honor it or not.
        // Cache-Control: public,max-age=10
        [HttpGet("/time1")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 10)]
        public ActionResult GetTheTime()
        {
            return Ok(Clock.GetCurrent().ToLongTimeString());
        }
    }
}
