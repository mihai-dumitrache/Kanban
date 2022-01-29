using Kanban.Models;

namespace Kanban.Services.Interfaces
{
    public interface IUserBoardServices
    {
        public int AddUserBoard(UserBoard userboard);
        public bool CheckUserAccessOnBoard(UserBoard userboard);

        public bool CheckUserOnUserBoards(UserBoard userboard);
    }
}
