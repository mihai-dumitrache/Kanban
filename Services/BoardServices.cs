using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using X.PagedList;

namespace Kanban.Services
{
    public class BoardServices : IBoardServices
    {
        private MyContext _context;

        public BoardServices()
        {
            _context = new MyContext();
            
        }

        public int AddBoard(Board board)
        {
            _context.Add<Board>(board);
            _context.Entry(board.CreatedByUser).State = EntityState.Detached;
            _context.SaveChanges();
            return 0;

        }

        public IEnumerable<Board> GetAllBoards()
        {
            return _context.Boards.Include(x => x.CreatedByUser).ToList();
        }

        public IPagedList<Board> GetBoardsByUser(User user)
        {
                IPagedList<Board> boardsByUser = _context.UserBoards.Where(x => x.User == user).Select(x => x.Board).ToPagedList();
                foreach (Board board in boardsByUser)
                {
                    board.CreatedByUser = _context.Boards.Where(x => x.Id == board.Id).Select(x => x.CreatedByUser).FirstOrDefault();
                }
                return boardsByUser;            
        }

        public IPagedList<Board> GetBoardsWhereAdmin(User user, bool isAdmin)
        {
            IPagedList<Board> boardsByUser = _context.UserBoards.Where(x => x.User == user).Where(x => x.IsAdmin == true).Select(x => x.Board).ToPagedList();
            foreach (Board board in boardsByUser)
            {
                board.CreatedByUser = _context.Boards.Where(x => x.Id == board.Id).Select(x => x.CreatedByUser).FirstOrDefault();
            }
            return boardsByUser;
        }

        public Board GetBoardById(int boardId)
        {

            Board board = new Board();
            board = _context.Boards
                                   .Include(x => x.CreatedByUser)
                                   .Where(x => x.Id == boardId)
                                   .FirstOrDefault<Board>();
            return board;
        }

        public Board EditBoard(Board board)
        {
            Board updatedBoard = new Board();
            updatedBoard = _context.Boards.SingleOrDefault(x => x.Id == board.Id);
            updatedBoard.Title = board.Title;
            updatedBoard.Description = board.Description;
            updatedBoard.ProjectStatus = board.ProjectStatus;
            _context.SaveChanges();
            return updatedBoard;
        }

        public void DeleteBoard(Board board)
        {
            _context.UserBoards.RemoveRange(_context.UserBoards.Where(x => x.Board.Id == board.Id));
            _context.Entry(board).State = EntityState.Deleted;
            _context.SaveChanges();
        }

    }
}
