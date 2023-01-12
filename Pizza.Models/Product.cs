using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Models
{
    public class Product
    {

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Additional_ingredients { get; set; }

        [Required]
        [Range(1,500)]
        public double Prise{ get; set; }

        [ValidateNever]
        public string imgurl { get; set; }

        [Required]
        public int Categoryid { get; set;}
        [ForeignKey("Categoryid")]
        [ValidateNever]
        public  Category Category { get; set; }

    }
}
