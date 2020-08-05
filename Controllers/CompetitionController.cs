using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Comander.Data;
using Comander.Dtos;
using Comander.Models;
using Commander.Data;
using Commander.Models;
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
        private readonly IUserRepo _repositoryUsers;
        private readonly IMapper _mapper;
        private IConfiguration _config;

        public CompetitionController(IUserRepo repositoryUsers, ICompetitionRepo repositoryCompetition, IMapper mapper, IConfiguration config)
        {
            _repositoryCompetition = repositoryCompetition;
            _repositoryUsers = repositoryUsers;

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
            UserModel owner = UserHandler.GetUserLoginByToken(Request.Headers["authorization"]);

            owner = _repositoryUsers.GetUserByLogin(owner.UserLogin);

            competition.Id = Guid.NewGuid().ToString();
            competition.ownerId = owner.Id;
            _repositoryCompetition.Register(competition);
            _repositoryCompetition.SaveChanges();

            return Ok();
        }


        [HttpGet]
        public ActionResult<IEnumerable<CompetitionDto>> GetAllCompetitionForUser()
        {
            UserModel user = UserHandler.GetUserLoginByToken(Request.Headers["authorization"]);
            user = _repositoryUsers.GetUserByLogin(user.UserLogin);

            var competition = _repositoryCompetition.GetAllCompetitionForUser(user.Id);

            return Ok(_mapper.Map<IEnumerable<CompetitionDto>>(competition));
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
            UserModel user = UserHandler.GetUserLoginByToken(Request.Headers["authorization"]);
            user = _repositoryUsers.GetUserByLogin(user.UserLogin);

            var competition = _repositoryCompetition.GetCompetitionById(id);
            if (competition != null)
            {
                if(competition.ownerId != user.Id)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<CompetitionDto>(competition));
            }
            return NotFound();

        }

    }
}


