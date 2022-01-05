using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

    }
}
