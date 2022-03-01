using Kanban.Models;
using Kanban.Models.Enums;
using Kanban.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;

namespace Kanban.Controllers
{
    public class TaskController : Controller
    {

        private ITaskServices _taskService;
        private IBoardServices _boardService;
        private IUserServices _userService;
        private IUserBoardServices _userBoardService;

        public TaskController(ITaskServices taskService, IBoardServices boardService, IUserServices userService, IUserBoardServices userBoardService)
        {
            _taskService = taskService;
            _boardService = boardService;
            _userService = userService;
            _userBoardService= userBoardService;
        }

        public IActionResult Index(Board board)
        {
            Task task = new Task();
            board = _boardService.GetBoardById(board.Id);
            task.Board = board; 
            task.TaskStatuses=_taskService.GetTaskStatusesByBoardId(board.Id);
            return View(task);
        }

        public IActionResult AddTask(Task task)
        {
            Task newTask = new Task();
            task.Board = _boardService.GetBoardById(task.Board.Id);
            newTask.Board = task.Board;
            newTask.Title = task.Title;
            newTask.Description = task.Description;
            UserBoard userBoard = new UserBoard();
            userBoard.Board = task.Board;
            if (_userService.GetUserByEmail(task.Responsible.EmailAdress) != null)
            {
                userBoard.User = _userService.GetUserByEmail(task.Responsible.EmailAdress);
                if (_userBoardService.CheckUserAccessOnBoard(userBoard) == true)
                {
                    //features to add: Check if user is on board before adding task / afisare task creator la ViewTask
                    newTask.Responsible = _userService.GetUserByEmail(task.Responsible.EmailAdress);
                    newTask.Status = task.Status;
                    newTask.CreatedBy = _userService.GetUserByEmail(HttpContext.Session.GetString("_Email"));
                    newTask.Board= _boardService.GetBoardById(task.Board.Id);
                    newTask.TaskStatuses = _taskService.GetTaskStatusesByBoardId(task.Board.Id);
                    
                    _taskService.AddTask(newTask);
                    newTask.Board.TasksList = _taskService.GetTasksByBoardId(task.Board);
                    Console.WriteLine("Bravo");
                    return View("Views/Board/ViewBoard.cshtml", newTask.Board);
                }
            }
            ModelState.AddModelError("UserNotOnBoard", "User has no access on board!");
            return View("Views/Task/Index.cshtml");
        }
        public IActionResult ViewTask(Task task)
        {
            task = _taskService.GetTaskById(task.Id);
            return View(task);
        }
        public IActionResult EditTask(Task task)
        {
            UserBoard userBoard = new UserBoard();
            userBoard.Board = _taskService.GetBoardByTaskId(task.Id);
            task.CreatedBy = _taskService.GetTaskById(task.Id).CreatedBy;
            if (_userService.CheckUserByEmail(task.Responsible.EmailAdress) == true)
            { userBoard.User = _userService.GetUserByEmail(task.Responsible.EmailAdress); }
            else {
                ModelState.AddModelError("UserNotFound", "User doesn't exist!");
                return View("Views/Task/UpdateTask.cshtml",task);
            }
            if (_userBoardService.CheckUserAccessOnBoard(userBoard) == true)
            {
                task = _taskService.EditTask(task);
                task.Board.TasksList = _taskService.GetTasksByBoardId(task.Board);
                return View("Views/Board/ViewBoard.cshtml", task.Board);
            }
            else
            {
                ModelState.AddModelError("UserNotOnBoard", "User has no access on board!");
                return View("Views/Task/UpdateTask.cshtml",task);
            }
        }

        public IActionResult UpdateTask(Task task)
        {
            task = _taskService.GetTaskById(task.Id);
            return View(task);
        }

        public IActionResult LoggedUserTasksExport()
        {
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(@"C:\Users\mihai\Desktop\My Tasks.xlsx",SpreadsheetDocumentType.Workbook);

            WorkbookPart workbookPart=spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "My Tasks"
            };

            Cell cellId = InsertCellInWorksheet("A", 1, worksheetPart);
            cellId.DataType = CellValues.String;
            cellId.CellValue = new CellValue("ID");
            Cell cellBoardTitle = InsertCellInWorksheet("B", 1, worksheetPart);
            cellBoardTitle.DataType = CellValues.String;
            cellBoardTitle.CellValue = new CellValue("Board Title");
            Cell cellTaskTitle = InsertCellInWorksheet("C", 1, worksheetPart);
            cellTaskTitle.DataType = CellValues.String;
            cellTaskTitle.CellValue = new CellValue("Task Title");
            Cell cellTaskDescription = InsertCellInWorksheet("D", 1, worksheetPart);
            cellTaskDescription.DataType = CellValues.String;
            cellTaskDescription.CellValue = new CellValue("Task Description");

            List<Task> loggedUserTasks = new List<Task>();
            string loggedUserEmail = HttpContext.Session.GetString("_Email");
            loggedUserTasks = _taskService.GetAllTasksOfLoggedUser(loggedUserEmail);

            var rowIndex = 2;
            for (int taskIndex=0;taskIndex<loggedUserTasks.Count;taskIndex++)
            {
                cellId = InsertCellInWorksheet("A", (uint)rowIndex, worksheetPart);
                cellId.DataType = CellValues.Number;
                cellId.CellValue = new CellValue(rowIndex-1);
                cellBoardTitle = InsertCellInWorksheet("B", (uint)rowIndex, worksheetPart);
                cellBoardTitle.DataType = CellValues.String;
                cellBoardTitle.CellValue = new CellValue(loggedUserTasks[taskIndex].Board.Title);
                cellTaskTitle = InsertCellInWorksheet("C", (uint)rowIndex, worksheetPart);
                cellTaskTitle.DataType = CellValues.String;
                cellTaskTitle.CellValue = new CellValue(loggedUserTasks[taskIndex].Title);
                cellTaskDescription = InsertCellInWorksheet("D", (uint)rowIndex, worksheetPart);
                cellTaskDescription.DataType = CellValues.String;
                cellTaskDescription.CellValue = new CellValue(loggedUserTasks[taskIndex].Description);
                rowIndex = rowIndex + 1;
            }

    

            sheets.Append(sheet);
            
            spreadsheetDocument.Save();
            spreadsheetDocument.Close();

            return RedirectToAction("Index","Board");

        }

        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }
    }
}
