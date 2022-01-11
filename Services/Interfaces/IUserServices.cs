using Kanban.Models;
using System.Collections.Generic;

namespace Kanban.Services.Interfaces
{
    public interface IUserServices
    {
        User GetUserByEmail(string userEmail);
        public int AddUser(User user);

        public string EncodePasswordToBase64(string password);
    }
}
