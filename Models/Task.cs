using Kanban.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kanban.Models
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Board Board { get; set; }

        public TaskPriority Priority { get; set; }


        //de schimbat in tabel Status
        public Status Status { get; set; }

        public User CreatedBy { get; set; }

        public User Responsible { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ClosingDate { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem> TaskStatuses { get; set; }

    }
}
