using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class BasketUpdateQuantityViewModel
    {
        public string ProductPrice { get; set; }
        public string TotalPrice { get; set; }
        public int BasketItemsCount { get; set; }
    }
}
