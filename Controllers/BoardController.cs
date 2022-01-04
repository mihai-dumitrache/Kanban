using Kanban.Models;
using Kanban.Models.Enums;
using Kanban.Models.ViewModels;
using Kanban.Services.Interfaces;
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

        public BoardController(IBoardServices boardService, IUserServices userService)
        {
            _boardService = boardService;
            _userService = userService;
        }
        
        public IActionResult Index(string sortOrder, string searchString)
        {
            Console.WriteLine("Bravo");
            IEnumerable<Board> boards = _boardService.GetAllBoards();
            if (!String.IsNullOrEmpty(searchString))
            {
                boards = boards.Where(s => s.Title.IndexOf(searchString, StringComparison.OrdinalIgnoreCase)>=0) ;
            }
            return View(boards);
        }

        public IActionResult Project()
        {            
            Console.WriteLine("Bravo");
            return View();
        }

        public IActionResult ViewBoard(Board board)
        {
            board=_boardService.GetBoardById(board.Id);
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
            IEnumerable<Board> boardList = _boardService.GetAllBoards();
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
            IEnumerable<Board> boardList = _boardService.GetAllBoards();
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
            User user=_userService.GetUserByEmail(board.UserEmail);
            newBoard.User=user;
            _boardService.AddBoard(newBoard);
            Console.WriteLine("Bravo");
            return RedirectToAction("Index");
        }
    }
}
