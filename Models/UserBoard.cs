using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanban.Models
{
    public class UserBoard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        public Board Board { get; set; }

        public User User { get; set; }

        public bool IsAdmin { get; set; }
    }
}
