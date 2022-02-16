using Kanban.Models;
using System.Collections.Generic;

namespace Kanban.Services.Interfaces
{
    public interface ITaskServices
    {
        int AddTask(Task task);
        public List<Task> GetTasksByBoardId(Board board);

        public Board GetBoardByTaskId(int taskId);

        public List<Task> GetTasksOfLoggedUser(Board board, string userEmail);
        Task GetTaskById(int id);
        Task EditTask(Task task);
    }
}
