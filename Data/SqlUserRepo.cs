using Commander.Data;
using Commander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commander.Data
{
    public class SqlUserRepo : IUserRepo
    {
        private readonly CommanderContext _context;

        public SqlUserRepo(CommanderContext context)
        {
            _context = context;
        }

        public UserModel GetUserById(string id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        public UserModel GetUserByLogin(string login)
        {
            return _context.Users.FirstOrDefault(x => x.UserLogin == login);
        }

        public IEnumerable<UserModel> GetUsers()
        {
            return _context.Users.ToList();
        }

        public bool isEmailInDb(UserModel user)
        {
            return _context.Users.Any(x => x.UserMail == user.UserMail);
        }

        public bool isUserInDb(UserModel user)
        {
            return _context.Users.Any(x => x.UserLogin == user.UserLogin);
        }

        public void Register(UserModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _context.Users.Add(user);

        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
