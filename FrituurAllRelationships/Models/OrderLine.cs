using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrituurAllRelationships.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }

        [Required]
        public int Amount { get; set; }
        [Required]
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
       
        [Required]
        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }
    }
}
