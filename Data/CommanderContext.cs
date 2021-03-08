using Comander.Models;
using Commander.Models;
using Microsoft.EntityFrameworkCore;

namespace Commander.Data
{
    public class CommanderContext :DbContext
    {
        public CommanderContext(DbContextOptions<CommanderContext> opt) : base(opt)
        {

        }

        public DbSet<Command> Commands { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CodeModel> Code { get; set; }
        public DbSet<RunModel> Run { get; set; }
        public DbSet<CompetitionModel> Competition { get; set; }
        public DbSet<ShooterModel> ShootersAtRun { get; set; }
    }

}
