using AutoMapper;
using Comander.Dtos;
using Comander.Models;
using Commander.Dtos;
using Commander.Models;

namespace Commander.Profiles
{
    public class CommandsProfile : Profile 
    {
        public CommandsProfile()
        {
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<CommandUpdateDto, Command>();
            CreateMap<Command, CommandUpdateDto>();

            CreateMap<UserDto, UserModel>();
            CreateMap<UserModel, UserDto>();

            CreateMap<CodeModel, CodeDto>();
            CreateMap<CodeDto, CodeModel>();

            CreateMap<CompetitionModel, CompetitionDto>();
            CreateMap<CompetitionDto, CompetitionModel>();

            CreateMap<RunModel, RunDto>();
            CreateMap<RunDto, RunModel>();
        }
    }

}