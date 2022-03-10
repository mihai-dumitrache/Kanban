using Kanban.Models;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;

namespace Kanban.Services.Interfaces
{
    public interface IBoardServices
    {
        int AddBoard(Board board);

        public IEnumerable<Board> GetAllBoards();

        public IPagedList<Board> GetBoardsByUser(User user);
        Board GetBoardById(int boardId);

        Board EditBoard(Board board, string taskStatuses);

        public IPagedList<Board> GetBoardsWhereAdmin(User user, bool isAdmin);

        public void DeleteBoard(Board board);

        public Board AddBoardTaskStatus(Board board, List<Status> boardTaskStatuses);

        public List<Status> GetDefaultTaskStatuses();

        public List<string> GetBoardTaskStatusesName(string taskStatuses);

        public List<Status> GetTaskStatusesList(List<string> taskStatuses);

        public List<Status> GetTaskStatusesOfBoard(int boardId);

    }
}
