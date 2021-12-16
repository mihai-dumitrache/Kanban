using Kanban.Models;
using Kanban.Services.Interfaces;
using System.Linq;

namespace Kanban.Services
{
    public class UserServices : IUserServices
    {
        public User GetUserByEmail(string userEmail)
        {
            User user = new User();

            var ctx = new MyContext();
            user = ctx.Users
                                   .Where(x => x.EmailAdress == userEmail)
                                   .FirstOrDefault<User>();

            return user;
        }
    }
}
