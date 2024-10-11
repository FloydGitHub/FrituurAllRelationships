using FrituurAllRelationships.Models;
using System.Collections;

namespace FrituurAllRelationships.ViewModels
{
    public class OrderWithOrderLineViewModel
    {

        public Order Order { get; set; }
        public List<OrderLine>? OrderLines { get; set; }
        decimal TotalPrice { get; set; }

    }
}

