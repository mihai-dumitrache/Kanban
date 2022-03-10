using Kanban.Models;
using Kanban.Models.Enums;
using Kanban.Models.ViewModels;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kanban.Controllers
{
    public class BoardController : Controller
    {
        private IBoardServices _boardService;
        private IUserServices _userService;
        private ITaskServices _taskService;
        private IUserBoardServices _userBoardService;

        public BoardController(IBoardServices boardService, IUserServices userService, ITaskServices taskService, IUserBoardServices userBoardService)
        {
            _boardService = boardService;
            _userService = userService;
            _taskService = taskService;
            _userBoardService = userBoardService;
        }


        public IActionResult Index(string sortOrder, string searchString, int? page)
        {
            if (HttpContext.Session.GetString("_Email") != String.Empty && HttpContext.Session.GetString("_Email") != null)
            {
                Console.WriteLine("Bravo");

                int pageIndex = 1;
                int pageSize = 10;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                IPagedList<Board> boards = _boardService.GetBoardsByUser(_userService.GetUserByEmail(HttpContext.Session.GetString("_Email")));
                if (!String.IsNullOrEmpty(searchString))
                {
                    boards = boards.Where(s => s.Title.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0).ToPagedList();
                }

                int pageNumber = (page ?? 1);

                return View(boards.ToPagedList(pageIndex, pageSize));
            }

            return View("Views/Home/Index.cshtml");

        }



        public IActionResult ShowPartialView(string sortOrder, string searchString, int? page, bool boardsWithAdmin)
        {
            int pageIndex = 1;
            int pageSize = 10;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<Board> boards2 = _boardService.GetBoardsWhereAdmin(_userService.GetUserByEmail(HttpContext.Session.GetString("_Email")), boardsWithAdmin);
            if (!String.IsNullOrEmpty(searchString))
            {
                boards2 = boards2.Where(s => s.Title.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0).ToPagedList();
            }

            int pageNumber = (page ?? 1);

            return PartialView("Views/PartialViews/_BoardIndex.cshtml", boards2.ToPagedList(pageIndex, pageSize));
        }

        public IActionResult Project()
        {
            Console.WriteLine("Bravo");
            return View();
        }


        public IActionResult ViewBoard(int id)
        {
            Board board = _boardService.GetBoardById(id);
            board.TasksList = _taskService.GetTasksByBoardId(board);
            Console.WriteLine("Bravo");
            return View(board);
        }

        public IActionResult ViewBoardWithLoggedUserTasks(int id)
        {
            Board board = _boardService.GetBoardById(id);
            string userEmail = HttpContext.Session.GetString("_Email");
            board.TasksList = _taskService.GetTasksOfLoggedUser(board, userEmail);
            Console.WriteLine("Bravo");
            return PartialView("Views/PartialViews/_OwnTasks.cshtml", board);
        }

        public IActionResult UpdateBoard(Board board)
        {
            Console.WriteLine("Bravo");
            board = _boardService.GetBoardById(board.Id);
            return View(board);
        }

        public IActionResult EditBoard(Board board, string sortOrder, string searchString, string taskStatuses)
        {
            Console.WriteLine("Bravo");
            
            if (_boardService.EditBoard(board,taskStatuses)==null)
            {
                ModelState.AddModelError("NoTaskStatus", "You must have at least 1 Task Status!");
                return View("Views/Board/UpdateBoard.cshtml",board);
            }
            _boardService.EditBoard(board, taskStatuses);
            IEnumerable<Board> boardList = _boardService.GetBoardsByUser(_userService.GetUserByEmail(HttpContext.Session.GetString("_Email")));
            if (!String.IsNullOrEmpty(searchString))
            {
                boardList = boardList.Where(s => s.Title.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteBoard(Board board, string sortOrder, string searchString)
        {
            _boardService.DeleteBoard(board);
            Console.WriteLine("Bravo");
            IEnumerable<Board> boardList = _boardService.GetBoardsByUser(_userService.GetUserByEmail(HttpContext.Session.GetString("_Email")));
            if (!String.IsNullOrEmpty(searchString))
            {
                boardList = boardList.Where(s => s.Title.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            return RedirectToAction("Index");
        }

        public IActionResult AddBoard(BoardViewModel board)
        {
            Console.WriteLine("Bravo");
            Board newBoard = new Board();
            newBoard.Title = board.Title;
            newBoard.Description = board.Description;
            if (board.Status == null)
            { newBoard.ProjectStatus = 0; }
            else
            { newBoard.ProjectStatus = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), board.Status); }
            newBoard.CreatedByUser = _userService.GetUserByEmail(HttpContext.Session.GetString("_Email"));
            UserBoard userboard = new UserBoard();
            userboard.Board = newBoard;
            userboard.User = _userService.GetUserByEmail(HttpContext.Session.GetString("_Email"));
            userboard.IsAdmin = true;

            List<string> taskStatuses = _boardService.GetBoardTaskStatusesName(board.taskStatuses);
            List<Status> boardTaskStatuses = new List<Status>();
            boardTaskStatuses = _boardService.GetTaskStatusesList(taskStatuses);
            //newBoard._boardService.GetAllTaskStatuses(board.taskStatuses)

            if (newBoard.Title == null)
            {
                ModelState.AddModelError("NoBoardName", "Board must contain a title!");
                return View("Views/Board/Project.cshtml");
            }
            else
            {
                _boardService.AddBoard(newBoard);
                _userBoardService.AddUserBoard(userboard);
                _boardService.AddBoardTaskStatus(newBoard, boardTaskStatuses);
                return RedirectToAction("Index");
            }

        }

        public List<Status> GetDefaultTaskStatuses()
        {
           return _boardService.GetDefaultTaskStatuses(); 
        }

        public List<Status> GetBoardTaskStatuses(int boardId)
        {
            return _boardService.GetTaskStatusesOfBoard(boardId);
        }

    }

}
