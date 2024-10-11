using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrituurAllRelationships.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
