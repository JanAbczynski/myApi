using System;
using System.Collections.Generic;
using System.Data;
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
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Comander.Controllers
{




    [Route("api/[controller]/[action]")]
    //[Authorize]
    [ApiController]
    public class CompetitionController : ControllerBase
    {
        private readonly IRunRepo _repositoryRun;
        private readonly ICompetitionRepo _repositoryCompetition;
        private readonly IUserRepo _repositoryUsers;
        private readonly IMapper _mapper;
        private IConfiguration _config;

        public CompetitionController(IRunRepo repositoryRun, IUserRepo repositoryUsers, ICompetitionRepo repositoryCompetition, IMapper mapper, IConfiguration config)
        {
            _repositoryRun = repositoryRun;
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
        public ActionResult AddCompetition(CompetitionModel competition)
        {
            UserModel owner = UserHandler.GetUserDataByToken(Request.Headers["authorization"]);
            owner = UserHandler.GetUserDataFromDbByLogin(owner.UserLogin);

            #region dbAccess
            string sqlProc = "exec dbo.AddCompetition";
            Dictionary<string, object> queryParams = new Dictionary<string, object> {
                      { "@description",competition.description},
                      { "@startTime",competition.startTime.ToString("yyyy-MM-dd HH:mm:ss.fff") },
                      { "@endTime",competition.endTime.ToString("yyyy-MM-dd HH:mm:ss.fff")},
                      { "@placeOf",competition.placeOf },
                      { "@ownerId",competition.ownerId }
            };
            DbHandler dbHandler = new DbHandler();
            dbHandler.GenerateProcedure(sqlProc, queryParams);
            dbHandler.GenerateQuerryValues(sqlProc, queryParams);
            sqlProc = dbHandler.AddParamsToQuery(sqlProc, queryParams);
            try
            {
                UserModel userModel = new UserModel();
                DataSet dataSet = dbHandler.GetSetFromDb(sqlProc, queryParams);
                foreach (DataRow row in dataSet.Tables["tab"].Rows)
                {
                    userModel.UserMail = row["UserMail"].ToString();
                    userModel.UserLogin = row["UserLogin"].ToString();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(e.Message);
            }

            #endregion

            return Ok();
        }


        [HttpGet]
        public ActionResult<IEnumerable<CompetitionDto>> GetAllCompetitionForUser()
        {
            UserModel user = UserHandler.GetUserDataByToken(Request.Headers["authorization"]);
            user = _repositoryUsers.GetUserByLogin(user.UserLogin);

            var competition = _repositoryCompetition.GetAllCompetitionForUser(user.Id);

            return Ok(_mapper.Map<IEnumerable<CompetitionDto>>(competition));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CompetitionDto>> GetAllCompetition()
        {
            //DataSet dataset = new DataSet();
            //SqlDataAdapter adapter = new SqlDataAdapter();
            //adapter.SelectCommand = new SqlCommand(
            //"SELECT * FROM [users]", connectionString);
            //adapter.Fill(dataset);


            //SqlConnection connectionString = new SqlConnection("Server=(localdb)\\MyLocalDB; Initial Catalog=Commander_DB; User ID=CommanderAPI; Password=qwe123qwe;");
            //DataTable dataTable = new DataTable();
            //SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("SELECT * FROM [users]", connectionString);
            ////sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@OrderID", OrderID);
            //sqlDataAdapter.Fill(dataTable);

            //foreach (DataRow orderDataRow in dataTable.Rows)
            //{
            //    Console.WriteLine(orderDataRow);
            //}



                var competition = _repositoryCompetition.GetAllCompetition();

            return Ok(_mapper.Map<IEnumerable<CompetitionDto>>(competition));
        }



        [HttpGet("{id}")]
        public ActionResult<CompetitionDto> GetCompetitionById(string id)
        {
            UserModel user = UserHandler.GetUserDataByToken(Request.Headers["authorization"]);
            user = _repositoryUsers.GetUserByLogin(user.UserLogin);

            var competition = _repositoryCompetition.GetCompetitionById(id);
            if (competition != null)
            {
                if(user.UserType == UserType.person.ToString())
                {
                    return Ok(_mapper.Map<CompetitionDto>(competition));
                }
                if(competition.ownerId != user.Id && user.UserType == UserType.company.ToString())
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<CompetitionDto>(competition));
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult addRun(RunModel run)
        {
            UserModel owner = UserHandler.GetUserDataByToken(Request.Headers["authorization"]);

            owner = _repositoryUsers.GetUserByLogin(owner.UserLogin);

            run.Id = Guid.NewGuid().ToString();
            run.ownerId = owner.Id;
            _repositoryRun.Register(run);
            _repositoryRun.SaveChanges();

            return Ok();
        }


        [HttpGet("{id}")]
        public ActionResult<IEnumerable<RunDto>> GetRunByCompetitionId(string id)
        {
            UserModel user = UserHandler.GetUserDataByToken(Request.Headers["authorization"]);
            user = _repositoryUsers.GetUserByLogin(user.UserLogin);

            var run = _repositoryRun.GetRunByCompetitionId(id);

            return Ok(_mapper.Map<IEnumerable<RunDto>>(run));
        }


        [HttpPost]
        public ActionResult RegisterUserInRun(RunModel run)
        {


            return Ok();
        }





    }
}


