using IoC.Domain.Contracts;
using IoC.Product.Domain.Contracts;
using IoC.Product.Domain.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.Product.AzureStorage
{
    public class TableStorageProductRepository : IProductRepository
    {
        internal const string DEFAULT_PARTITION_KEY = "default";

        private CloudTable _cloudTable;

        public TableStorageProductRepository()
        {
            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");

            initCloudTable(storageAccount);
        }

        public TableStorageProductRepository(string storageName, string storageKey)
        {
            var storageCredentials = new StorageCredentials(storageName, storageKey);

            var storageAccount = new CloudStorageAccount(storageCredentials, false);

            initCloudTable(storageAccount);
        }

        private void initCloudTable(CloudStorageAccount storageAccount)
        {
            var tableClient = storageAccount.CreateCloudTableClient();

            _cloudTable = tableClient.GetTableReference("productstable");
            _cloudTable.CreateIfNotExists();
        }

        IEnumerable<ProductEntity> IRepository<ProductEntity>.GetAll()
        {
            TableQuery<ProductAzureTableEntity> getAllQuery = new TableQuery<ProductAzureTableEntity>();

            var productTableEntities = _cloudTable.ExecuteQuery(getAllQuery);

            return productTableEntities
                .OrderBy(pte => pte.CreationDate)
                .Select(
                    pte => new ProductEntity
                    {
                        Id = pte.Id,
                        CategoryId = pte.CategoryId,
                        Description = pte.Description
                    });
        }

        ProductEntity IRepository<ProductEntity>.GetById(string id)
        {
            var productTableEntity = GetProductTableEntityById(DEFAULT_PARTITION_KEY, id);

            return new ProductEntity
            {
                Id = productTableEntity.Id,
                CategoryId = productTableEntity.CategoryId,
                Description = productTableEntity.Description
            };
        }

        void IRepository<ProductEntity>.Insert(ProductEntity product)
        {
            var productTableEntity = (ProductAzureTableEntity)product; //explicit operator

            TableOperation insert = TableOperation.Insert(productTableEntity);
            _cloudTable.Execute(insert);
        }

        void IRepository<ProductEntity>.Update(ProductEntity product)
        {
            var productTableEntity = GetProductTableEntityById(DEFAULT_PARTITION_KEY, product.Id);

            if (productTableEntity != null)
            {
                productTableEntity.CategoryId = product.CategoryId;
                productTableEntity.Description = product.Description;

                TableOperation update = TableOperation.Replace(productTableEntity);
                _cloudTable.Execute(update);
            }
        }

        void IRepository<ProductEntity>.DeleteById(string id)
        {
            var productTableEntity = GetProductTableEntityById(DEFAULT_PARTITION_KEY, id);

            if (productTableEntity != null)
            {
                TableOperation deleteByIdOperation = TableOperation.Delete(productTableEntity);
                _cloudTable.Execute(deleteByIdOperation);
            }
        }

        private ProductAzureTableEntity GetProductTableEntityById(string partitionKey, string rowKey)
        {
            TableOperation getByIdOperation = TableOperation.Retrieve<ProductAzureTableEntity>(partitionKey, rowKey);

            var productTableEntity = _cloudTable.Execute(getByIdOperation).Result as ProductAzureTableEntity;
            return productTableEntity;
        }
    }

    internal class ProductAzureTableEntity : TableEntity
    {
        public string Id { get; set; }

        public string CategoryId { get; set; }

        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public static explicit operator ProductAzureTableEntity(ProductEntity product)
        {
            return new ProductAzureTableEntity
            {
                PartitionKey = TableStorageProductRepository.DEFAULT_PARTITION_KEY,
                RowKey = product.Id,
                Id = product.Id,
                CategoryId = product.CategoryId,
                Description = product.Description,
                CreationDate = DateTime.Now
            };
        }
    }
}
