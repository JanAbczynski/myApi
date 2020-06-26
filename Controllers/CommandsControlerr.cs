using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Comander.Models;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Comander.Controllers
{
    //api/commands
    // [EnableCors("_Policy")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController: ControllerBase
    {
        private readonly ICommanderRepo _repository;
        private readonly IMapper _mapper;
        private IConfiguration _config;


        public CommandsController(ICommanderRepo repository, IMapper mapper, IConfiguration config)
        {
            _repository = repository;
            _mapper = mapper;
            _config = config;
        }

        // GET api/commands/login
        [HttpGet("login")]

        // private readonly MockCommanderRepo _repository = new MockCommanderRepo();
        //api/commands

        
        [Authorize]
        [HttpGet]
        public ActionResult <IEnumerable<CommandReadDto>> GetAllCommands()
        {
            
            var commandItems = _repository.GetAllCommands();

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }

        // GET api/commands/{id}
        [HttpGet("{id}", Name="GetCommandById")]
        public ActionResult <CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _repository.GetCommandById(id);
            if (commandItem != null){
            return Ok(_mapper.Map<CommandReadDto>(commandItem));
            }
            return NotFound();

        }


        [HttpPost]
        public ActionResult <CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var commonModel = _mapper.Map<Command>(commandCreateDto);   
            _repository.CreateCommande(commonModel);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(commonModel);

            return CreatedAtRoute(nameof(GetCommandById), new {Id = commandReadDto.Id}, commandReadDto);
            // return Ok(commonModel);
        }

        //PUT api/commands/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            	
            var commandModelFromRepo = _repository.GetCommandById(id);
            if(commandModelFromRepo == null){
                return NotFound();
            }
            _mapper.Map(commandUpdateDto, commandModelFromRepo);
            _repository.Update(commandModelFromRepo);
            _repository.SaveChanges();
            return NoContent();
        }


        //PATCH api/commands/{id}
        [HttpPatch("{id}")]
        public ActionResult PatchCommand(int id, JsonPatchDocument<CommandUpdateDto> patchDoc){

            var commandModelFromRepo = _repository.GetCommandById(id);
            if(commandModelFromRepo == null){
                return NotFound();
            }

            var CommandToPatch = _mapper.Map<CommandUpdateDto>(commandModelFromRepo);
            patchDoc.ApplyTo(CommandToPatch, ModelState);
            if(!TryValidateModel(CommandToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(CommandToPatch, commandModelFromRepo);

            _repository.Update(commandModelFromRepo);

            _repository.SaveChanges();


            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCommand (int id){

            var commandModelFromRepo = _repository.GetCommandById(id);
            if(commandModelFromRepo == null){
                return NotFound();
            }

            _repository.DeleteCommand(commandModelFromRepo);
            _repository.SaveChanges();
            return NoContent();
        }

    }
}