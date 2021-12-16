using System.Collections.Generic;

namespace Kanban.Models.ViewModels
{
    public class BoardViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string Status { get; set; }

        public string UserEmail { get; set; }

    }
}
