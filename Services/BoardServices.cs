using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kanban.Services
{
    public class BoardServices : IBoardServices
    {
        public int AddBoard(Board board)
        {
                var context=new MyContext();
                
            
                context.Add<Board>(board);
                context.Entry(board.User).State = EntityState.Detached;
                context.SaveChanges(); 
                return 0;            
        }

        public IEnumerable<Board> GetAllBoards()
        {
            var context= new MyContext();
            return context.Boards.Include(x => x.User).ToList();
        }

        public Board GetBoardById(int boardId)
        {
            
            Board board=new Board();
            var context=new MyContext();
            board = context.Boards
                                   .Include(x => x.User)
                                   .Where(x => x.Id == boardId)
                                   .FirstOrDefault<Board>();
            return board;
        }

        public Board EditBoard(Board board)
        {
            Board updatedBoard = new Board();
            var context = new MyContext();
            updatedBoard = context.Boards.SingleOrDefault(x => x.Id == board.Id);
            updatedBoard.Title=board.Title;
            updatedBoard.Description=board.Description;
            updatedBoard.ProjectStatus = board.ProjectStatus;
            context.SaveChanges();
            return updatedBoard;
        }

        public void DeleteBoard(Board board)
        {
            var context = new MyContext();
            context.Entry(board).State = EntityState.Deleted;
            context.SaveChanges();
        }

    }
}
