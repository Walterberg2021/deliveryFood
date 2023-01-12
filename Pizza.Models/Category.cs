using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pizza.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Діапазон значень 1-100")]
        public int DisplayOrder { get; set; }
        public DateTime CreatedDataTime { get; set; } = DateTime.Now;


    }
}
