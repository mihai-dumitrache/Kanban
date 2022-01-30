using Kanban.Models;
using Kanban.Models.Enums;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Kanban.Controllers
{
    public class TaskController : Controller
    {

        private ITaskServices _taskService;
        private IBoardServices _boardService;
        private IUserServices _userService;
        private IUserBoardServices _userBoardService;

        public TaskController(ITaskServices taskService, IBoardServices boardService, IUserServices userService, IUserBoardServices userBoardService)
        {
            _taskService = taskService;
            _boardService = boardService;
            _userService = userService;
            _userBoardService= userBoardService;
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
            UserBoard userBoard = new UserBoard();
            userBoard.Board = task.Board;
            if (_userService.GetUserByEmail(task.Responsible.EmailAdress) != null)
            {
                userBoard.User = _userService.GetUserByEmail(task.Responsible.EmailAdress);
                if (_userBoardService.CheckUserAccessOnBoard(userBoard) == false)
                {
                    //features to add: Check if user is on board before adding task / afisare task creator la ViewTask
                    newTask.Responsible = _userService.GetUserByEmail(task.Responsible.EmailAdress);
                    newTask.Progress = task.Progress;
                    newTask.CreatedBy = _userService.GetUserByEmail(HttpContext.Session.GetString("_Email"));
                    _taskService.AddTask(newTask);
                    Console.WriteLine("Bravo");
                    return RedirectToAction("ViewBoard", "Board", newTask.Board);
                }
            }
            ModelState.AddModelError("UserNotOnBoard", "User has no access on board!");
            return View("Views/Task/Index.cshtml");
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
