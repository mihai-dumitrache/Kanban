using Kanban.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Kanban.Models
{
    public class Board
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        
        public int Id { get; set; }
        public string Title { get; set; }

        public List<Task> TasksList { get; set; }

        //public List<User> UsersList { get; set; }

        public List<UserBoard> UserBoards { get; set; }

        public List<BoardTaskStatus> BoardTaskStatuses { get; set;}
        public string Description { get; set;}

        public User CreatedByUser { get; set; }

        public ProjectStatus ProjectStatus { get; set; }

        
    }

   
}
