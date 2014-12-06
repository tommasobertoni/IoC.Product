using IoC.Domain.Contracts;
using IoC.Product.Domain.Contracts;
using IoC.Product.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.Product.FileSystem
{
    public class FileSystemProductRepository : IProductRepository
    {
        private DirectoryInfo _di;

        public FileSystemProductRepository()
        {
            _di = new DirectoryInfo(@"C:\_productRepository\");
            if (!_di.Exists) _di.Create();
        }

        public FileSystemProductRepository(DirectoryInfo di)
        {
            _di = di;
        }

        IEnumerable<ProductEntity> IRepository<ProductEntity>.GetAll()
        {
            var fi = _di.EnumerateFiles()
                .Where(xx => xx.Name.EndsWith(".json"));

            foreach (var file in fi)
            {
                yield return FromFileToProduct(file);
            }
        }

        ProductEntity IRepository<ProductEntity>.GetById(string id)
        {
            var fi = _di.EnumerateFiles()
                .SingleOrDefault(xx => xx.Name == id + ".json");

            if (fi != null)
            {
                return FromFileToProduct(fi);
            }
            else
            {
                return null;
            }
        }

        private ProductEntity FromFileToProduct(FileInfo file)
        {
            var streamReader = file.OpenText();
            var json = streamReader.ReadToEnd();
            streamReader.Close();

            var product = JsonConvert.DeserializeObject<ProductEntity>(json);
            return product;
        }


        void IRepository<ProductEntity>.Insert(ProductEntity product)
        {
            var fi = _di.EnumerateFiles()
                .SingleOrDefault(xx => xx.Name == product.Id + ".json");

            if (fi != null)
                throw new ArgumentException("Item with id {0} alredy exists", product.Id);

            WriteEntity(product);
        }

        void IRepository<ProductEntity>.Update(ProductEntity product)
        {
            WriteEntity(product);
        }

        private void WriteEntity(ProductEntity product)
        {
            if (product.Id == null)
                throw new ArgumentNullException("Id can't be null");

            using (StreamWriter writer = new StreamWriter(_di.FullName + String.Format(@"\{0}.json", product.Id)))
            {
                string serializedItem = JsonConvert.SerializeObject(product);
                writer.Write(serializedItem);
            }
        }

        void IRepository<ProductEntity>.DeleteById(string id)
        {
            File.Delete(_di.FullName + String.Format(@"\{0}.json", id));
        }
    }
}
