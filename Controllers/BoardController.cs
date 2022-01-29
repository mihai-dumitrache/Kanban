using Kanban.Models;
using Kanban.Models.Enums;
using Kanban.Models.ViewModels;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        public IActionResult Index(string sortOrder, string searchString)
        {
            if(HttpContext.Session.GetString("_Email")!=String.Empty && HttpContext.Session.GetString("_Email") != null)
                    {
                Console.WriteLine("Bravo");
                IEnumerable<Board> boards = _boardService.GetBoardsByUser(_userService.GetUserByEmail(HttpContext.Session.GetString("_Email")));
                if (!String.IsNullOrEmpty(searchString))
                {
                    boards = boards.Where(s => s.Title.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                return View(boards);
                    }
            return View("Views/Home/Index.cshtml");
        }

        public IActionResult Project()
        {            
            Console.WriteLine("Bravo");
            return View();
        }

        public IActionResult ViewBoard(Board board)
        {
            board =_boardService.GetBoardById(board.Id);
            board.TasksList = _taskService.GetTasksByBoardId(board);
            Console.WriteLine("Bravo");
            return View(board);
        }

        public IActionResult UpdateBoard(Board board)
        {
            Console.WriteLine("Bravo");
            board = _boardService.GetBoardById(board.Id);
            return View(board);
        }

        public IActionResult EditBoard(Board board, string sortOrder, string searchString)
        {
            Console.WriteLine("Bravo");
            _boardService.EditBoard(board);
            IEnumerable<Board> boardList = _boardService.GetBoardsByUser(_userService.GetUserByEmail(HttpContext.Session.GetString("_Email")));
            if (!String.IsNullOrEmpty(searchString))
            {
                boardList = boardList.Where(s => s.Title.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            return View("Views/Board/Index.cshtml",boardList);
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
            newBoard.ProjectStatus = (ProjectStatus)Enum.Parse(typeof(ProjectStatus),board.Status);
            newBoard.CreatedByUser = _userService.GetUserByEmail(HttpContext.Session.GetString("_Email"));
            UserBoard userboard=new UserBoard();
            userboard.Board = newBoard;
            userboard.User = _userService.GetUserByEmail(HttpContext.Session.GetString("_Email"));
            userboard.IsAdmin = true;
            _boardService.AddBoard(newBoard);
            _userBoardService.AddUserBoard(userboard);
            Console.WriteLine("Bravo");
            return RedirectToAction("Index");
        }
    }
}
