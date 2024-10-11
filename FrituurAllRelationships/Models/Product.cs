using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrituurAllRelationships.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public decimal Price { get; set; }

    }
}
