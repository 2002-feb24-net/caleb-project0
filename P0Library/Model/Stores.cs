using System;
using System.Collections.Generic;

namespace P0Library.Model
{
    public partial class Stores
    {
        public Stores()
        {
            Orders = new HashSet<Orders>();
        }

        public int Id { get; set; }
        public string Address { get; set; }
        public int? Stock { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
