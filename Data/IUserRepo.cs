using Commander.Models;
using System.Collections.Generic;




namespace Commander.Data
{
    public interface IUserRepo
    {
        bool SaveChanges();
        IEnumerable<UserModel> GetUsers();
        UserModel GetUserByLogin(string loign);
        UserModel GetUserById(string id);
        void Register(UserModel user);
        bool isUserInDb(UserModel user);
        bool isEmailInDb(UserModel user);
    }
}
