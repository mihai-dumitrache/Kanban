using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Kanban.Services
{
    public class TaskServices : ITaskServices
    {

        private MyContext _context;

        public TaskServices()
        {
            _context = new MyContext();
        }
        public int AddTask(Task task)
        {
            _context.Add<Task>(task);
            _context.Entry(task.Board.CreatedByUser).State = EntityState.Detached;
            _context.Entry(task.Board).State = EntityState.Detached;
            _context.Entry(task.Responsible).State = EntityState.Detached;
            _context.SaveChanges();
            return 0;
        }

        public List<Task> GetTasksByBoardId(Board board)

        {
            List<Task> tasksList = new List<Task>();
            tasksList = _context.Tasks.Where(x => x.Board==board).ToList();
            return tasksList;
        }

        public Task GetTaskById(int id)
        {
            Task task = new Task();
            task = _context.Tasks.Include(y => y.Board).Include(y => y.CreatedBy).SingleOrDefault(x => x.Id == id);
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
            _context.SaveChanges();
            return updatedTask;
        }

    }
}
