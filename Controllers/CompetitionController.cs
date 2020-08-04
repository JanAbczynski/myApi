using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Comander.Data;
using Comander.Dtos;
using Comander.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Comander.Controllers
{




    [Route("api/[controller]/[action]")]
    //[Authorize]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        private readonly ICompetitionRepo _repositoryCompetition;
        private readonly IMapper _mapper;
        private IConfiguration _config;

        public CompetitionController(ICompetitionRepo repositoryCompetition, IMapper mapper, IConfiguration config)
        {
            _repositoryCompetition = repositoryCompetition;

            _mapper = mapper;
            _config = config;
        }


        //[Authorize(Policy = "company")]
        [HttpGet]
        public ActionResult test()
        {
            var x = 5;

            return Ok();
        }

        [HttpPost]
        public ActionResult addCompetition(CompetitionModel competition)
        {
            competition.Id = Guid.NewGuid().ToString();
            _repositoryCompetition.Register(competition);
            _repositoryCompetition.SaveChanges();

            return Ok();
        }


        [HttpGet]
        public ActionResult<IEnumerable<CompetitionDto>> GetAllCompetition()
        {

            var competition = _repositoryCompetition.GetAllCompetition();

            return Ok(_mapper.Map<IEnumerable<CompetitionDto>>(competition));
        }

        [HttpGet("{id}")]
        public ActionResult<CompetitionDto> GetCompetitionById(string id)
        {
            var competition = _repositoryCompetition.GetCompetitionById(id);
            if (competition != null)
            {
                return Ok(_mapper.Map<CompetitionDto>(competition));
            }
            return NotFound();

        }

    }
}


