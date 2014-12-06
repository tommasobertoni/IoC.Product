using IoC.Domain.Contracts;
using IoC.Product.Domain.Contracts;
using IoC.Product.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.Product.EntityFram
{
    public class EntityFrameworkProductRepository : IProductRepository
    {
        private string _connectionString;

        public EntityFrameworkProductRepository(
            string connectionString = "Data Source=.;Initial Catalog=IoCProducts;Integrated Security=SSPI;")
        {
            _connectionString = connectionString;
        }

        IEnumerable<ProductEntity> IRepository<ProductEntity>.GetAll()
        {
            using (DbContext _dbContext = new ProductEntitiesDbContext(_connectionString))
            {
                var entity = _dbContext
                    .Set<ProductSqlTableEntity>()
                    .ToList<ProductSqlTableEntity>();

                return entity
                    .OrderBy(xx => xx.CreationDate)
                    .Select(
                        xx => new ProductEntity
                        {
                            Id = xx.Id,
                            CategoryId = xx.CategoryId,
                            Description = xx.Description
                        });
            }
        }

        ProductEntity IRepository<ProductEntity>.GetById(string id)
        {
            using (DbContext _dbContext = new ProductEntitiesDbContext(_connectionString))
            {
                var entity = _dbContext
                .Set<ProductSqlTableEntity>()
                .Where(xx => xx.Id == id)
                .ToList()
                .SingleOrDefault();

                if (entity != null)
                {
                    return new ProductEntity
                    {
                        Id = entity.Id,
                        CategoryId = entity.Id,
                        Description = entity.Description
                    };
                }
                else
                {
                    throw new ArgumentException("Id not found");
                }
            }
        }

        void IRepository<ProductEntity>.Insert(ProductEntity product)
        {
            if (product.Id == null)
                throw new ArgumentNullException("ProductId can't be null");

            using (DbContext _dbContext = new ProductEntitiesDbContext(_connectionString))
            {
                _dbContext.Set<ProductSqlTableEntity>().Add((ProductSqlTableEntity)product);
                _dbContext.SaveChanges();
            }
        }

        void IRepository<ProductEntity>.Update(ProductEntity product)
        {
            if (product.Id == null)
                throw new ArgumentNullException("Id can't be null");

            using (DbContext _dbContext = new ProductEntitiesDbContext(_connectionString))
            {
                ProductSqlTableEntity productToUpdate = _dbContext.Set<ProductSqlTableEntity>()
                    .Where(xx => xx.Id == product.Id).FirstOrDefault();

                if (productToUpdate != null)
                {
                    productToUpdate.CategoryId = product.CategoryId;
                    productToUpdate.Description = product.Description;

                    _dbContext.Entry(productToUpdate).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("Could't find product with id " + product.Id);
                }
            }
        }

        void IRepository<ProductEntity>.DeleteById(string id)
        {
            using (DbContext _dbContext = new ProductEntitiesDbContext(_connectionString))
            {
                ProductSqlTableEntity product = _dbContext.Set<ProductSqlTableEntity>()
                    .Where(xx => xx.Id == id).FirstOrDefault();

                if (product != null)
                {
                    _dbContext.Entry(product).State = EntityState.Deleted;
                    _dbContext.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("Could't find product with id " + id);
                }
            }
        }
    }

    internal class ProductEntitiesDbContext : DbContext
    {
        public ProductEntitiesDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ProductSqlTableEntity>()
                .HasKey(xx => xx.Id)
                .ToTable("Products");
        }
    }

    internal class ProductSqlTableEntity
    {
        public string Id { get; set; }

        public string CategoryId { get; set; }

        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public static explicit operator ProductSqlTableEntity(ProductEntity product)
        {
            return new ProductSqlTableEntity
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Description = product.Description,
                CreationDate = DateTime.Now
            };
        }
    }
}
