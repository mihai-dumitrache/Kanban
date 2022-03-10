using ClosedXML.Excel;
using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Kanban.Services
{
    public class TaskServices : ITaskServices
    {

        private MyContext _context;
        private IUserServices _userService;
        private IHttpContextAccessor _contextAccessor;

        public TaskServices(IUserServices userService, IHttpContextAccessor contextAccessor)
        {
            _context = new MyContext();
            _userService = userService;
            _contextAccessor = contextAccessor;
        }

        public int AddTask(Task task)
        {
            //Case if task status is not chosen
            if (task.Status == null)
            {
                task.Status = task.Board.BoardTaskStatuses[0].Status;
            }
            //Populate task object
            //code added from here
            if (task.Responsible == null)
            {
                task.Responsible = null;
            }
            //to here
            else
            {
                task.Responsible = _userService.GetUserByEmail(task.Responsible.EmailAdress);
            }
            if (_userService.GetUserByEmail(_contextAccessor.HttpContext.Session.GetString("_Email")) != null)
            {
                task.CreatedBy = _userService.GetUserByEmail(_contextAccessor.HttpContext.Session.GetString("_Email")); 
            }
            else 
            { 
                task.CreatedBy = null; 
            }
            task.TaskStatuses = GetTaskStatusesByBoardId(task.Board.Id);
            task.Status = _context.TaskStatus.Where(x => x.StatusName == task.Status.StatusName).AsNoTracking().FirstOrDefault();
            task.CreationDate = DateTime.Today.Date;

            _context.Add<Task>(task);
            _context.Entry(task.Board.CreatedByUser).State = EntityState.Detached;
            _context.Entry(task.Board).State = EntityState.Detached;
            if (task.Responsible != null)
            {
                _context.Entry(task.Responsible).State = EntityState.Detached;
            }
            _context.Entry(task.CreatedBy).State = EntityState.Detached;
            _context.Entry(task.Status).State = EntityState.Detached;

            for (int taskIndex = 0; taskIndex < task.Board.UserBoards.Count; taskIndex++)
            {
                _context.Entry(task.Board.UserBoards[taskIndex]).State = EntityState.Detached;
            }
            for (int taskIndex = 0; taskIndex < task.Board.BoardTaskStatuses.Count; taskIndex++)
            {
                _context.Entry(task.Board.BoardTaskStatuses[taskIndex]).State = EntityState.Detached;
            }
            for (int taskIndex = 0; taskIndex < task.Board.BoardTaskStatuses.Count; taskIndex++)
            {
                _context.Entry(task.Board.BoardTaskStatuses[taskIndex].Status).State = EntityState.Detached;
            }
            for (int taskIndex = 0; taskIndex < task.Board.TasksList.Count-1; taskIndex++)
            {
                _context.Entry(task.Board.TasksList[taskIndex].Status).State = EntityState.Detached;
                if (task.Board.TasksList[taskIndex].Responsible != null)
                {
                    _context.Entry(task.Board.TasksList[taskIndex].Responsible).State = EntityState.Detached;
                }
                _context.Entry(task.Board.TasksList[taskIndex].CreatedBy).State = EntityState.Detached;
                _context.Entry(task.Board.TasksList[taskIndex]).State = EntityState.Detached;
            }
            //for (int taskIndex = 0; taskIndex < task.Board.TasksList.Count; taskIndex++)
            //{
            //    _context.Entry(task.Board.TasksList[taskIndex]).State = EntityState.Detached;
            //}

            _context.SaveChanges();
            return 0;
        }

        public List<Task> GetTasksByBoardId(Board board)

        {
            List<Task> tasksList = new List<Task>();
            tasksList = _context.Tasks.Where(x => x.Board == board).Include(x => x.Responsible).Include(x => x.Status).Include(x => x.Board.BoardTaskStatuses).ToList();
            return tasksList;
        }

        public List<Task> GetTasksOfLoggedUser(Board board, string userEmail)
        {
            List<Task> tasksList = new List<Task>();
            tasksList = _context.Tasks.Where(x => x.Board == board).Where(x => x.Responsible.EmailAdress == userEmail).Include(x => x.Responsible).Include(x => x.Status).Include(x => x.Board).ToList();
            return tasksList;
        }

        public List<Task> GetAllTasksOfLoggedUserFromActiveBoard(Board board, string userEmail)
        {
            List<Task> tasksList = new List<Task>();
            tasksList = _context.Tasks.Where(x => x.Board == board).Where(x => x.Responsible.EmailAdress == userEmail).Include(x => x.Responsible).Include(x => x.Status).Include(x => x.Board).ToList();
            return tasksList;
        }

        public List<Task> GetAllTasksOfLoggedUser(string userEmail)
        {
            List<Task> tasksList = new List<Task>();
            tasksList = _context.Tasks.Where(x => x.Responsible.EmailAdress == userEmail).Include(x => x.Responsible).Include(x => x.Status).Include(x => x.Board).ToList();
            return tasksList;
        }

        public Task GetTaskById(int id)
        {
            Task task = new Task();
            task = _context.Tasks.Include(y => y.Board).Include(y => y.CreatedBy).Include(y => y.Responsible).Include(y => y.Status).SingleOrDefault(x => x.Id == id);
            task.TaskStatuses = _context.BoardTaskStatus.Where(x => x.Board.Id == task.Board.Id).Select(x => new SelectListItem
            {
                Value = x.Status.StatusName,
                Text = x.Status.StatusName
            }).ToList();

            return task;
        }

        public IEnumerable<SelectListItem> GetTaskStatusesByBoardId(int boardId)
        {
            Task task = new Task();
            task.TaskStatuses = _context.BoardTaskStatus.Where(x => x.Board.Id == boardId).Select(x => new SelectListItem
            {
                Value = x.Status.StatusName,
                Text = x.Status.StatusName
            }).ToList();
            return task.TaskStatuses;
        }

        public Task EditTask(Task task)
        {
            task.Board = GetBoardByTaskId(task.Id);
            if (task.Status == null)
            {
                task.Status = task.Board.BoardTaskStatuses[0].Status;
            }
            

            Task updatedTask = new Task();
            updatedTask = _context.Tasks.Include(y => y.Board).SingleOrDefault(x => x.Id == task.Id);
            updatedTask.Title = task.Title;
            updatedTask.Description = task.Description;
            updatedTask.Priority = task.Priority;
            updatedTask.Status = _context.TaskStatus.Where(x => x.StatusName == task.Status.StatusName).FirstOrDefault();
            if (task.Responsible.EmailAdress == null)
            {
                updatedTask.Responsible = null;
            }
            else
            {
                updatedTask.Responsible = _userService.GetUserByEmail(task.Responsible.EmailAdress);
                _context.Entry(updatedTask.Responsible).State = EntityState.Detached;
            }
            updatedTask.Board.BoardTaskStatuses = _context.BoardTaskStatus.Where(x => x.Board.Id == updatedTask.Board.Id).Include(x => x.Status).ToList();
            _context.Entry(updatedTask.CreatedBy).State = EntityState.Detached;
            
            _context.SaveChanges();
            return updatedTask;
        }

        public Board GetBoardByTaskId(int taskId)
        {
            Board board = new Board();
            board = _context.Tasks.Where(y => y.Id == taskId).Select(y => y.Board).FirstOrDefault();
            board.BoardTaskStatuses = _context.BoardTaskStatus.Where(x => x.Board.Id == board.Id).Include(x => x.Status).Include(x => x.Board).ToList();
            return board;
        }

        public MemoryStream OpenAndAddToSpreadsheetStream(string reportType, Board board)
        {
            MemoryStream memoryStream = new MemoryStream();
            LoadOptions loadOptions=new LoadOptions();
            
            //using XLWorkbook workbook= new XLWorkbook(templatePath, loadOptions);

            if (reportType == "MyTasks")
            {
                string templatePath = Directory.GetCurrentDirectory() + @"\Resources\MyTasks.xlsx";
                using XLWorkbook workbook = new XLWorkbook(templatePath, loadOptions);

                var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == "MyTasks");
                int currentRow = 2;
                
                List<Task> loggedUserTasks = new List<Task>();
                string loggedUserEmail = _contextAccessor.HttpContext.Session.GetString("_Email");
                loggedUserTasks = GetAllTasksOfLoggedUserFromActiveBoard(board, loggedUserEmail);
               
                for (int taskIndex = 0; taskIndex < loggedUserTasks.Count; taskIndex++)
                {
                    worksheet.Cell(currentRow, 1).Value = currentRow - 1;
                    worksheet.Cell(currentRow, 2).Value = loggedUserTasks[taskIndex].Board.Title;
                    worksheet.Cell(currentRow, 3).Value = loggedUserTasks[taskIndex].Title;
                    worksheet.Cell(currentRow, 4).Value = loggedUserTasks[taskIndex].Description;
                    currentRow = currentRow + 1;
                }

                worksheet.Rows().AdjustToContents();
                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(memoryStream);
                
            }
            if (reportType == "AllTasks")
            {
                string templatePath = Directory.GetCurrentDirectory() + @"\Resources\AllTasks.xlsx";
                using XLWorkbook workbook = new XLWorkbook(templatePath, loadOptions);

                var worksheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == "AllTasks");
                int currentRow = 2;

                List<Task> allBoardTasks = new List<Task>();
                allBoardTasks = GetTasksByBoardId(board);

                for (int taskIndex = 0; taskIndex < allBoardTasks.Count; taskIndex++)
                {
                    worksheet.Cell(currentRow, 1).Value = currentRow - 1;
                    worksheet.Cell(currentRow, 2).Value = allBoardTasks[taskIndex].Board.Title;
                    worksheet.Cell(currentRow, 3).Value = allBoardTasks[taskIndex].Title;
                    worksheet.Cell(currentRow, 4).Value = allBoardTasks[taskIndex].Description;
                    if (allBoardTasks[taskIndex].Responsible == null)
                    {
                        worksheet.Cell(currentRow, 5).Value = null;
                    }
                    else
                    {
                        worksheet.Cell(currentRow, 5).Value = allBoardTasks[taskIndex].Responsible.Name;
                    }
                    currentRow = currentRow + 1;
                }

                worksheet.Rows().AdjustToContents();
                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(memoryStream);
                
            }
            memoryStream.Seek(0,SeekOrigin.Begin);
            
            return memoryStream;
        }
       
        public int CountTasksWithStatusType(string taskStatusName,int boardId)
        {
            //Status status = _context.TaskStatus.Where(x => x.StatusName == taskStatusName).FirstOrDefault();
            int taskCounter = _context.Tasks.Where(x => x.Status.StatusName == taskStatusName).Where(x => x.Board.Id == boardId).Count();
            return taskCounter;
        }
    }
}
