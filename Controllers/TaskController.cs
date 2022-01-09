using Kanban.Models;
using Kanban.Models.Enums;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Kanban.Controllers
{
    public class TaskController : Controller
    {

        private ITaskServices _taskService;
        private IBoardServices _boardService;

        public TaskController(ITaskServices taskService, IBoardServices boardService)
        {
            _taskService = taskService;
            _boardService = boardService;
        }

        public IActionResult Index(Board board)
        {
            Task task = new Task();
            board = _boardService.GetBoardById(board.Id);
            task.Board = board;
            return View(task);
        }

        public IActionResult AddTask(Task task)
        {
            Task newTask = new Task();
            task.Board = _boardService.GetBoardById(task.Board.Id);
            newTask.Board = task.Board;
            newTask.Title = task.Title;
            newTask.Description = task.Description;
            newTask.Responsible=task.Responsible;
            newTask.Progress = task.Progress;
            //newTask.Priority = (TaskPriority)Enum.Parse(typeof(TaskPriority), task.Priority);
            _taskService.AddTask(newTask);
            Console.WriteLine("Bravo");
            return RedirectToAction("ViewBoard", "Board",newTask.Board);
        }
        public IActionResult ViewTask(Task task)
        {
            task = _taskService.GetTaskById(task.Id);
            return View(task);
        }
        public IActionResult EditTask(Task task)
        {
            task = _taskService.EditTask(task);
            task.Board.TasksList = _taskService.GetTasksByBoardId(task.Board);
            return View("Views/Board/ViewBoard.cshtml",task.Board);
        }

        public IActionResult UpdateTask(Task task)
        {
            task = _taskService.GetTaskById(task.Id);
            return View(task);
        }
    }
}
