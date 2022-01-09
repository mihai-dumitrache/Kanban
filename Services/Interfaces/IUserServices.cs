using Kanban.Models;

namespace Kanban.Services.Interfaces
{
    public interface IUserServices
    {
        User GetUserByEmail(string userEmail);
    
    }
}
