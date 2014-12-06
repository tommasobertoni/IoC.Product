using IoC.Product.Domain.Contracts;
using IoC.Product.Domain.Entities;
using IoC.Product.WebPresentation.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IoC.Product.WebPresentation.Controllers
{
    public class ProductApiController : ApiController
    {
        private IProductRepository _productRepository;

        public ProductApiController() : base()
        {
            _productRepository = ContractsResolverProvider.Instance.Container.Resolve<IProductRepository>();
        }

        public IEnumerable<ProductEntity> Get()
        {
            return _productRepository.GetAll();
        }

        public ProductEntity Get(string id)
        {
            return _productRepository.GetById(id);
        }

        public void Post([FromBody]ProductEntity product)
        {
            _productRepository.Insert(product);
        }

        public void Put(string id, [FromBody]ProductEntity product)
        {
            product.Id = id;
            _productRepository.Update(product);
        }

        public void Delete(string id)
        {
            _productRepository.DeleteById(id);
        }
    }
}
