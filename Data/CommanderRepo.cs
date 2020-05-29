using System.Collections.Generic;
using Commander.Models;


namespace Commander.Data
{
     public interface ICommanderRepo
    {
        bool SaveChanges();
        IEnumerable<Command> GetAllCommands();
        Command GetCommandById(int id);
        void CreateCommande(Command command);
    }
}