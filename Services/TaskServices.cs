using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Kanban.Services
{
    public class TaskServices : ITaskServices
    {

        private MyContext _context;
        private IUserServices _userService;

        public TaskServices(IUserServices userService)
        {
            _context = new MyContext();
            _userService = userService;
        }
        public int AddTask(Task task)
        {
            _context.Add<Task>(task);
            _context.Entry(task.Board.CreatedByUser).State = EntityState.Detached;
            _context.Entry(task.Board).State = EntityState.Detached;
            _context.Entry(task.Responsible).State = EntityState.Detached;
            _context.Entry(task.CreatedBy).State = EntityState.Detached;
            for (int i=0;i<task.Board.UserBoards.Count;i++)
            {
                _context.Entry(task.Board.UserBoards[i]).State = EntityState.Detached;
            }
            for (int i = 0; i < task.Board.TasksList.Count-1; i++)
            {
                _context.Entry(task.Board.TasksList[i]).State = EntityState.Detached;
            }
            _context.SaveChanges();
            return 0;
        }

        public List<Task> GetTasksByBoardId(Board board)

        {
            List<Task> tasksList = new List<Task>();
            tasksList = _context.Tasks.Where(x => x.Board==board).Include(x => x.Responsible).ToList();
            return tasksList;
        }

        public List<Task> GetTasksOfLoggedUser(Board board, string userEmail)
        {
            List<Task> tasksList = new List<Task>();
            tasksList = _context.Tasks.Where(x => x.Board == board).Where(x => x.Responsible.EmailAdress == userEmail).ToList();
            return tasksList;
        }

        public Task GetTaskById(int id)
        {
            Task task = new Task();
            task = _context.Tasks.Include(y => y.Board).Include(y => y.CreatedBy).Include(y => y.Responsible).SingleOrDefault(x => x.Id == id);
            return task;
        }

        public Task EditTask(Task task)
        {
            Task updatedTask = new Task();
            updatedTask = _context.Tasks.Include(y => y.Board).SingleOrDefault(x => x.Id == task.Id);
            updatedTask.Title = task.Title;
            updatedTask.Description = task.Description;
            updatedTask.Priority=task.Priority;
            updatedTask.Progress = task.Progress;
            updatedTask.Responsible = _userService.GetUserByEmail(task.Responsible.EmailAdress);
            updatedTask.CreatedBy = task.CreatedBy;
            _context.SaveChanges();
            return updatedTask;
        }

        public Board GetBoardByTaskId(int taskId)
        {
            Board board = new Board();
            board = _context.Tasks.Where(y => y.Id==taskId).Select(y => y.Board).FirstOrDefault();
            return board;
        }

    }
}
