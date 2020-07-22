using Commander.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace Comander.Controllers
{
    [Route("api/[controller]/[action]/{id?}")]
    [ApiController]

    public class CodeController : ControllerBase
    {
        private readonly ICodeRepo _repositoryUsers;
        public CodeController(ICodeRepo repository, IMapper mapper, IConfiguration config)
        {

        }

        [HttpPost]
        public void Test()
        {
            Console.WriteLine("");
        }
    }
}