using IoC.Domain.Contracts;
using IoC.Product.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.Product.Domain.Contracts
{
    public interface IProductRepository : IRepository<ProductEntity>
    {
    }
}
