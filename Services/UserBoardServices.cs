using Kanban.Models;
using Kanban.Services.Interfaces;
using Kanban.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Kanban.Services
{
    public class UserBoardServices : IUserBoardServices
    {
        private MyContext _context;
        private IUserServices _userService;
        public UserBoardServices(IUserServices userService)
        {
            _context = new MyContext();
            _userService = userService;
        }
        public int AddUserBoard(UserBoard userboard)
        {
            if (CheckUserAccessOnBoard(userboard) == true)
            {
                _context.Add<UserBoard>(userboard);
                _context.Entry(userboard.User).State = EntityState.Detached;
                _context.Entry(userboard.Board.CreatedByUser).State = EntityState.Detached;
                _context.Entry(userboard.Board).State = EntityState.Detached;
                _context.SaveChanges();
                return 0;
            }
            else
                return 1;
        }

        public bool CheckUserAccessOnBoard(UserBoard userboard)
        {
            if (_userService.GetUserByEmail(userboard.User.EmailAdress)!=null )
            {
                if (CheckUserOnUserBoards(userboard)==false)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool CheckUserOnUserBoards(UserBoard userboard)
        {
            UserBoard userboards = new UserBoard();
            userboards = _context.UserBoards
                                    .Where(x => x.User == userboard.User)
                                    .Where(x => x.Board == userboard.Board)
                                    .FirstOrDefault<UserBoard>();
            if (userboards == null)
            { return false; }
            return true;
        }

    }
}
