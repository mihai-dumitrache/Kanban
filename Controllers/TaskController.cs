using Kanban.Models;
using Kanban.Models.Enums;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

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
            task.TaskStatuses=_taskService.GetTaskStatusesByBoardId(board.Id);
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
                if (_userBoardService.CheckUserAccessOnBoard(userBoard) == true)
                {
                    //features to add: Check if user is on board before adding task / afisare task creator la ViewTask
                    newTask.Responsible = _userService.GetUserByEmail(task.Responsible.EmailAdress);
                    newTask.Status = task.Status;
                    newTask.CreatedBy = _userService.GetUserByEmail(HttpContext.Session.GetString("_Email"));
                    newTask.Board= _boardService.GetBoardById(task.Board.Id);
                    newTask.TaskStatuses = _taskService.GetTaskStatusesByBoardId(task.Board.Id);
                    
                    _taskService.AddTask(newTask);
                    newTask.Board.TasksList = _taskService.GetTasksByBoardId(task.Board);
                    Console.WriteLine("Bravo");
                    return View("Views/Board/ViewBoard.cshtml", newTask.Board);
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
            UserBoard userBoard = new UserBoard();
            userBoard.Board = _taskService.GetBoardByTaskId(task.Id);
            task.CreatedBy = _taskService.GetTaskById(task.Id).CreatedBy;
            if (_userService.CheckUserByEmail(task.Responsible.EmailAdress) == true)
            { userBoard.User = _userService.GetUserByEmail(task.Responsible.EmailAdress); }
            else {
                ModelState.AddModelError("UserNotFound", "User doesn't exist!");
                return View("Views/Task/UpdateTask.cshtml",task);
            }
            if (_userBoardService.CheckUserAccessOnBoard(userBoard) == true)
            {
                task = _taskService.EditTask(task);
                task.Board.TasksList = _taskService.GetTasksByBoardId(task.Board);
                return View("Views/Board/ViewBoard.cshtml", task.Board);
            }
            else
            {
                ModelState.AddModelError("UserNotOnBoard", "User has no access on board!");
                return View("Views/Task/UpdateTask.cshtml",task);
            }
        }

        public IActionResult UpdateTask(Task task)
        {
            task = _taskService.GetTaskById(task.Id);
            return View(task);
        }
    }
}
