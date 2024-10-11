using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrituurAllRelationships.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual ICollection<OrderLine>? OrderLines { get; set; }
        [Required]
        public bool Confirmed { get; set; } = false;
    }
}
