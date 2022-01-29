﻿using Kanban.Models;
using System.Collections.Generic;
using System.Linq;

namespace Kanban.Services.Interfaces
{
    public interface IBoardServices
    {
        int AddBoard(Board board);
        public IEnumerable<Board> GetAllBoards();

        public IEnumerable<Board> GetBoardsByUser(User user);
        Board GetBoardById(int boardId);

        Board EditBoard(Board board);

        public void DeleteBoard(Board board);
    }
}
