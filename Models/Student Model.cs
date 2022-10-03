using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ajax_curd.Models
{
    public class StudentModel
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Name")]
        [Required(ErrorMessage = "This Field is required.")]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Email")]
        [Required(ErrorMessage = "This Field is required.")]
        public string Email { get; set; } = string.Empty;
    }
}
