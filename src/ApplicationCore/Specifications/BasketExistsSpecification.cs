using ApplicationCore.Entities;
using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Specifications
{
    public class BasketExistsSpecification : Specification<Basket>
    {
        public BasketExistsSpecification(string buyerId)
        {
            Query.Where(x => x.BuyerId == buyerId);
        }
    }
}
