using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Comander.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        //[Authorize(Policy = "company")]
        [HttpGet]
        public ActionResult test()
        {
            var x = 5;

            return Ok();
        }

        [HttpGet]
        public ActionResult test2()
        {
            var x = 5;

            return Ok();
        }

    }
}


