using System.Collections.Generic;
using Commander.Models;

namespace Commander.Data
{
    public class MockCommanderRepo : ICommanderRepo
    {
        public void CreateCommande(Command command)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteCommand(Command command)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Command> GetAllCommands()
        {
            var commands = new List<Command>
            {
                new Command{Id = 0, HowTo = "boil eg", Line = "Boil water", Platform = "Jj"},
                new Command{Id = 1, HowTo = "cut bread", Line = "eat", Platform = "Jj"},
                new Command{Id = 2, HowTo = "Make tea", Line = "pleace tea", Platform = "Jj"}
            };

            return commands;
        }

        public Command GetCommandById(int id)
        {
            return new Command{Id = 0, HowTo = "boil egg", Line = "Boil water", Platform = "Jj"};
        }

        public bool SaveChanges()
        {
            throw new System.NotImplementedException();
        }

        public void Update(Command cmd)
        {
            throw new System.NotImplementedException();
        }
    }
}