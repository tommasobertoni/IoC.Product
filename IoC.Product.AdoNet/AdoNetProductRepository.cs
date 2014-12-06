using IoC.Domain.Contracts;
using IoC.Product.Domain.Contracts;
using IoC.Product.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.Product.AdoNet
{
    public class AdoNetProductRepository : IProductRepository
    {
        private string _connectionString;

        public AdoNetProductRepository(
            string connectionString = "Data Source=.;Initial Catalog=IoCProducts;Integrated Security=SSPI;")
        {
            _connectionString = connectionString;
        }

        IEnumerable<ProductEntity> IRepository<ProductEntity>.GetAll()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT Id,
                                        CategoryId,
                                        Description
                                 FROM   dbo.Products
                                 ORDER BY CreationDate";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductEntity product = new ProductEntity
                            {
                                Id = (string)reader["Id"],
                                CategoryId = reader["CategoryId"] == DBNull.Value ? null : (string)reader["CategoryId"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"]
                            };

                            yield return product;
                        }
                    }
                }
            }
        }

        ProductEntity IRepository<ProductEntity>.GetById(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT Id,
                                        CategoryId,
                                        Description
                                 FROM   dbo.Products
                                 WHERE Id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@id", id));

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ProductEntity product = new ProductEntity
                            {
                                Id = (string)reader["Id"],
                                CategoryId = reader["CategoryId"] == DBNull.Value ? null : (string)reader["CategoryId"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"]
                            };

                            return product;
                        }

                        return null;
                    }
                }
            }
        }

        void IRepository<ProductEntity>.Insert(ProductEntity product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"INSERT INTO [dbo].[Products]
		                                     ([Id]
		                                     ,[CategoryId]
		                                     ,[Description]
                                             ,[CreationDate])

	                             VALUES      (@Id
		                                     ,@CategoryId
		                                     ,@Description
                                             ,@CreationDate)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", (object)product.Id ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@CategoryId", (object)product.CategoryId ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@Description", (object)product.Description ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@CreationDate", DateTime.Now));

                    int affectedRows = command.ExecuteNonQuery();
                }
            }
        }

        void IRepository<ProductEntity>.Update(ProductEntity product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"UPDATE [dbo].[Products]
                                 SET [CategoryId] = @CategoryId,
                                     [Description] = @Description
                                 WHERE ProductID = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@id", product.Id));
                    command.Parameters.Add(new SqlParameter("@CategoryId", (object)product.CategoryId ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@Description", (object)product.Description ?? DBNull.Value));

                    int affectedRows = command.ExecuteNonQuery();
                }
            }
        }

        void IRepository<ProductEntity>.DeleteById(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"DELETE FROM [dbo].[Products]
                                 WHERE Id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@id", id));
                    int affectedRows = command.ExecuteNonQuery();
                }
            }
        }
    }
}
