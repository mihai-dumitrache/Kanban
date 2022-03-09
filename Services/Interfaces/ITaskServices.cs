using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Kanban.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;

namespace Kanban.Services.Interfaces
{
    public interface ITaskServices
    {
        int AddTask(Task task);
        public List<Task> GetTasksByBoardId(Board board);

        public Board GetBoardByTaskId(int taskId);

        public List<Task> GetTasksOfLoggedUser(Board board, string userEmail);
        public List<Task> GetAllTasksOfLoggedUser(string userEmail);
        Task GetTaskById(int id);
        Task EditTask(Task task);

        public IEnumerable<SelectListItem> GetTaskStatusesByBoardId(int boardId);
        public MemoryStream OpenAndAddToSpreadsheetStream(string templatePath, string reportType, Board board);

        public List<Task> GetAllTasksOfLoggedUserFromActiveBoard(Board board, string userEmail);
    }
}
