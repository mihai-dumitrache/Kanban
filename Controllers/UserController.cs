using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.Controllers
{
    public class UserController : Controller
    {
        //private IBoardServices _boardService;
        private IUserServices _userService;
        //private ITaskServices _taskService;

        public UserController(IUserServices userService)
        {
            //_boardService = boardService;
            _userService = userService;
            //_taskService = taskService;
        }

        public IActionResult AddUser(User user)
        {
            if (_userService.AddUser(user) == 0)
            {
                return View("Views/Home/Index.cshtml");
            }
            else
            { return View("Views/User/RegisterAgain.cshtml"); }
        }
    }
}
