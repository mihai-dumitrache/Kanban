using Kanban.Models;
using Kanban.Models.ViewModels;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Kanban.Controllers
{
    public class UserController : Controller
    {
        private IBoardServices _boardService;
        private IUserServices _userService;
        private ITaskServices _taskService;
        private IUserBoardServices _userBoardService;

        public UserController(IUserServices userService, IBoardServices boardService, ITaskServices taskService, IUserBoardServices userBoardService)
        {
            _boardService = boardService;
            _userService = userService;
            _taskService = taskService;
            _userBoardService = userBoardService;
        }


        public IActionResult AddUser(User user)
        {
            if (_userService.AddUser(user) == 0)
            {
                return View("Views/Home/Index.cshtml");
            }
            else if (_userService.AddUser(user) == 1)         
            {
                ModelState.AddModelError("EmailAlreadyUsed", "Email already in use.");
                return View("Views/Home/Register.cshtml"); 
            }
            else if (_userService.AddUser(user) == 2)
            {
                ModelState.AddModelError("InvalidPassword", "Password must contain at least 8 characters and must contain at least 1 digit and 1 symbol!");
                return View("Views/Home/Register.cshtml");
            }
            else
            {
                    ModelState.AddModelError("UnknownError", "Unknown Error");
                    return View("Views/Home/Register.cshtml");
                
            }
        }

        public IActionResult LoginUser(User user)
        {
            Console.WriteLine("Bravo");
            if (ModelState.IsValid)
            {
                if (_userService.CheckUserByEmail(user.EmailAdress) == false)
                {
                    ModelState.AddModelError("InvalidUser", "Invalid User");
                    return View("Views/Home/Index.cshtml", user);

                }

                else if (_userService.CheckPasswordFromLogin(user) == false)
                {
                    ModelState.AddModelError("InvalidPassword", "Wrong Password");
                    return View("Views/Home/Index.cshtml", user);

                }
            };

            HttpContext.Session.SetString("_Email", String.Empty);
            HttpContext.Session.SetString("_Password", String.Empty);

            string loggedUser = HttpContext.Session.GetString("_Email");

            if (string.IsNullOrEmpty(loggedUser))
            {
                HttpContext.Session.SetString("_Email", user.EmailAdress);
                HttpContext.Session.SetString("_Password", user.Password);
            }

            IEnumerable<Board> boards = _boardService.GetBoardsByUser(_userService.GetUserByEmail(HttpContext.Session.GetString("_Email")));

            return View("Views/Board/Index.cshtml", boards);
        }

       
        public IActionResult AddUserToBoard(string userEmail,Board board)
        {
            board=_boardService.GetBoardById(board.Id);
            board.TasksList = _taskService.GetTasksByBoardId(board);
            if(_userService.CheckUserByEmail(userEmail)==true)
            {
                UserBoard userBoard = new UserBoard();
                userBoard.Board=board;
                userBoard.User=_userService.GetUserByEmail(userEmail);
                userBoard.IsAdmin = false;
                if(_userBoardService.AddUserBoard(userBoard)==1)
                {
                    ModelState.AddModelError("UserHasAccess", "User has already access on board!");
                    return View("Views/Board/ViewBoard.cshtml", board);
                }
                //de adaugat eroare ca userul deja are acces in Board
                return View("Views/Board/ViewBoard.cshtml", board);
            }
            ModelState.AddModelError("UserNotFound", "User not found");
            return View("Views/Board/ViewBoard.cshtml",board);
        }

        public IActionResult LogOutUser(User user)
        {
            HttpContext.Session.SetString("_Email", String.Empty);
            HttpContext.Session.SetString("_Password", String.Empty);
            return View("Views/Home/Index.cshtml");
        }

    }
           
}
