using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.Product.Domain.Entities
{
    public class ProductEntity
    {
        public string Id { get; set; }

        public string CategoryId { get; set; }

        public string Description { get; set; }
    }
}
