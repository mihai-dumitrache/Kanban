using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Kanban.Services
{
    public class TaskServices : ITaskServices
    {
        public int AddTask(Task task)
        {
            var context = new MyContext();
            context.Add<Task>(task);
            context.Entry(task.Board.User).State = EntityState.Detached;
            context.Entry(task.Board).State = EntityState.Detached;
            context.SaveChanges();
            return 0;
        }

        public List<Task> GetTasksByBoardId(Board board)

        {
            List<Task> tasksList = new List<Task>();
            var context = new MyContext();
            tasksList = context.Tasks.Where(x => x.Board==board).ToList();
            return tasksList;
        }

        public Task GetTaskById(int id)
        {
            Task task = new Task();
            var context = new MyContext();
            task = context.Tasks.Include(y => y.Board).SingleOrDefault(x => x.Id == id);
            return task;
        }

        public Task EditTask(Task task)
        {
            Task updatedTask = new Task();
            var context = new MyContext();
            updatedTask = context.Tasks.Include(y => y.Board).SingleOrDefault(x => x.Id == task.Id);
            updatedTask.Title = task.Title;
            updatedTask.Description = task.Description;
            updatedTask.Priority=task.Priority;
            updatedTask.Progress = task.Progress;
            context.SaveChanges();
            return updatedTask;
        }

    }
}
