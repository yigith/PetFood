using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class OrderItem : BaseEntity
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public string PictureUri { get; set; }
        public decimal UnitPrice { get; set; }
        public int Units { get; set; }

    }
}
