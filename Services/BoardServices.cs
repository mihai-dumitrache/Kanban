using Kanban.Models;
using Kanban.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
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
                                   .Include(x => x.TasksList)
                                   .Include(x => x.UserBoards)
                                   .Where(x => x.Id == boardId)
                                   .FirstOrDefault<Board>();
            board.BoardTaskStatuses = _context.BoardTaskStatus.Where(x => x.Board.Id == boardId).Include(x => x.Status).Include(x => x.Board).ToList();
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

        public Board AddBoardTaskStatus(Board board, List<Status> boardTaskStatuses)
        {
           
            for (int boardTaskStatusesIndex = 0; boardTaskStatusesIndex < boardTaskStatuses.Count(); boardTaskStatusesIndex++)
            {
                BoardTaskStatus boardTaskStatus = new BoardTaskStatus();
                _context.Entry(boardTaskStatus).State = EntityState.Detached;
                boardTaskStatus.Board = board;
                boardTaskStatus.Status = boardTaskStatuses[boardTaskStatusesIndex];
                _context.Add<BoardTaskStatus>(boardTaskStatus);
                _context.SaveChanges();
            }
            
            board.BoardTaskStatuses = _context.BoardTaskStatus.Where(x => x.Id == board.Id).Include(x => x.Status).ToList();
            return board;
        }

        public void DeleteBoard(Board board)
        {
            _context.UserBoards.RemoveRange(_context.UserBoards.Where(x => x.Board.Id == board.Id));
            _context.BoardTaskStatus.RemoveRange(_context.BoardTaskStatus.Where(x => x.Board.Id == board.Id));
            _context.Entry(board).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public List<Status> GetDefaultTaskStatuses()
        {
            List<Status> defaultTaskStatuses = _context.TaskStatus.Where(x => x.DefaultStatus == true).ToList();
            return defaultTaskStatuses;
        }

        public List<string> GetBoardTaskStatusesName(string taskStatuses)
        {
            List<string> taskStatusesList=new List<string>();
            int commaIndex = taskStatuses.IndexOf(',');
            while(commaIndex!=-1)
            {
                taskStatusesList.Add(taskStatuses.Substring(0, commaIndex));
                taskStatuses = taskStatuses.Substring(commaIndex+1);
                commaIndex = taskStatuses.IndexOf(',');
            }
            taskStatusesList.Add(taskStatuses);
            return taskStatusesList;
        }

        public List<Status> GetTaskStatusesList(List<string> taskStatuses)
        {
            List<Status> taskStatusesList=new List<Status>(); 
            for (int taskStatusesListIndex=0;taskStatusesListIndex<taskStatuses.Count;taskStatusesListIndex++)
            {
                if (_context.TaskStatus.Where(x => x.StatusName == taskStatuses[taskStatusesListIndex]).FirstOrDefault()!=null)
                {
                    taskStatusesList.Add(_context.TaskStatus.Where(x => x.StatusName == taskStatuses[taskStatusesListIndex]).FirstOrDefault());
                }
                else
                {
                    Status newStatus=new Status();
                    newStatus.StatusName=taskStatuses[taskStatusesListIndex];
                    newStatus.DefaultStatus = false;
                    _context.Add<Status>(newStatus);
                    _context.SaveChanges();
                    taskStatusesList.Add(newStatus);
                }
            }
            return taskStatusesList;
        }

    }
}
