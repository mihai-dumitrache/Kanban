using Kanban.Models;
using Kanban.Models.Enums;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.IO;

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
            _userBoardService = userBoardService;
        }

        public IActionResult Index(Board board)
        {
            Task task = new Task();
            task.Board = _boardService.GetBoardById(board.Id);
            task.TaskStatuses = _taskService.GetTaskStatusesByBoardId(board.Id);
            return View(task);
        }

        public IActionResult AddTask(Task task)
        {
            task.Board = _boardService.GetBoardById(task.Board.Id);
            UserBoard userBoard = new UserBoard();
            userBoard.Board = task.Board;
            //code added from here
            if (task.Responsible.EmailAdress==null)
            {
                _taskService.AddTask(task);
                return View("Views/Board/ViewBoard.cshtml", task.Board);
            }
            //to here
            if (_userService.GetUserByEmail(task.Responsible.EmailAdress) != null)
            {
                userBoard.User = _userService.GetUserByEmail(task.Responsible.EmailAdress);
                if (_userBoardService.CheckUserAccessOnBoard(userBoard) == true)
                {
                    _taskService.AddTask(task);
                    return View("Views/Board/ViewBoard.cshtml", task.Board);
                }
            }
            ModelState.AddModelError("UserNotOnBoard", "User has no access on board!");
            return View("Views/Task/Index.cshtml",task);
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
            if (task.Responsible.EmailAdress==null)
            {
                task = _taskService.EditTask(task);
                task.Board.TasksList = _taskService.GetTasksByBoardId(task.Board);
                return View("Views/Board/ViewBoard.cshtml", task.Board);
            }
            if (_userService.CheckUserByEmail(task.Responsible.EmailAdress) == true)
            { userBoard.User = _userService.GetUserByEmail(task.Responsible.EmailAdress); }
            else {
                ModelState.AddModelError("UserNotFound", "User doesn't exist!");
                return View("Views/Task/UpdateTask.cshtml", task);
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
                return View("Views/Task/UpdateTask.cshtml", task);
            }
        }

        public IActionResult UpdateTask(Task task)
        {
            task = _taskService.GetTaskById(task.Id);
            return View(task);
        }

        public IActionResult TasksExport(int boardId, string reportType)
        {
            Board board=_boardService.GetBoardById(boardId);

            MemoryStream memoryStream=new MemoryStream();

            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            if (reportType == "MyTasks" || reportType=="AllTasks")
            {
               
                memoryStream = _taskService.OpenAndAddToSpreadsheetStream(reportType, board);
                
            }
            else
            {
                ModelState.AddModelError("NoChosenOption", "Please choose a report type");
                return View(board);
            }

            return new FileStreamResult(memoryStream, contentType);

        }

        public int CountTasksWithStatusType(string taskStatusName, int boardId)
        {
            return _taskService.CountTasksWithStatusType(taskStatusName,boardId);
        }

    }
}
