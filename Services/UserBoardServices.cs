using Kanban.Models;
using Kanban.Services.Interfaces;
using Kanban.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace Kanban.Services
{
    public class UserBoardServices : IUserBoardServices
    {
        private MyContext _context;
        private IUserServices _userService;
        private IBoardServices _boardService;

        public UserBoardServices(IUserServices userService, IBoardServices boardService)
        {
            _context = new MyContext();
            _userService = userService;
            _boardService = boardService;
        }
        public int AddUserBoard(UserBoard userboard)
        {
            if (CheckUserAccessOnBoard(userboard) == false)
            {
                userboard.User=_context.Users.AsNoTracking().First(x => x == userboard.User);
                userboard.Board=_context.Boards.AsNoTracking().First(x => x == userboard.Board);
                _context.Add<UserBoard>(userboard);
                _context.Entry(userboard.User).State = EntityState.Detached;
                _context.Entry(userboard.Board).State = EntityState.Detached;
                _context.SaveChanges();
                return 0;
            }
            else
                return 1;
        }

        public bool CheckUserAccessOnBoard(UserBoard userboard)
        {
            if (_userService.GetUserByEmail(userboard.User.EmailAdress) != null)
            {
                if (CheckUserOnUserBoards(userboard) == false)
                {
                    return false;
                }
                return true;
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

        public bool CheckIfAdmin(string userEmail, int boardId)
        {
            UserBoard userBoard = new UserBoard();
            userBoard.User =_userService.GetUserByEmail(userEmail);
            userBoard.Board = _boardService.GetBoardById(boardId);
            userBoard = _context.UserBoards
                                            .Where(x => x.User == userBoard.User)
                                            .Where(x => x.Board == userBoard.Board)
                                            .FirstOrDefault<UserBoard>();
            if (userBoard.IsAdmin==true)
            {
                return true;
            }
            return false;
        }

        
    }
}
