using Kanban.Models;
using System.Collections.Generic;

namespace Kanban.Services.Interfaces
{
    public interface IUserBoardServices
    {
        public int AddUserBoard(UserBoard userboard);
        public bool CheckUserAccessOnBoard(UserBoard userboard);

        public bool CheckUserOnUserBoards(UserBoard userboard);

        public bool CheckIfAdmin(string userEmail, int boardId);

        
    }
}
