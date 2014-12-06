using IoC.Product.Domain.Contracts;
using IoC.Product.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC.Product.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string separator = "----------------";

            Console.WriteLine("Test Started\n");
            Console.WriteLine("{0}", separator);

            try
            {
                IProductRepository irepository = ContractsResolverProvider.Instance.Container.Resolve<IProductRepository>();

                Console.WriteLine("INSERT: categoryId,productId\n");

                string input = Console.ReadLine();

                string[] tokens = input.Split(',');
                var categoryId = tokens[0];
                var productId = tokens[1];

                Console.WriteLine("RECEIVED: {0},{1}\n", categoryId, productId);

                //test REST
                var iproduct = new ProductEntity();
                iproduct.Id = productId;
                iproduct.CategoryId = categoryId;
                iproduct.Description = "Product Description";

                //INSERT
                irepository.Insert(iproduct);
                Console.WriteLine("\nProduct inserted\n");

                //GETALL
                {
                    Console.WriteLine("\nGetAll");
                    var products = irepository.GetAll();
                    foreach (var p in products)
                    {
                        Console.WriteLine("{0} - {1}: {2}", p.CategoryId, p.Id, p.Description);
                    }
                    Console.WriteLine();
                }

                //UPDATE
                iproduct.Description = "Product Description Changed";
                irepository.Update(iproduct);
                Console.WriteLine("\nProduct changed\n");

                //GETBYID
                var productModif = irepository.GetById(iproduct.Id);
                if (productModif != null)
                {
                    Console.WriteLine("\nGetProductById({3})\n{0} - {1}: {2}\n",
                        productModif.Id, iproduct.CategoryId, iproduct.Id, iproduct.Description);
                }

                //DELETE
                Console.WriteLine("Press enter to delete the product");
                Console.ReadLine();
                irepository.DeleteById(productModif.Id);
                Console.WriteLine("\nProdotto deleted\n");

                //GETALL
                {
                    Console.WriteLine("\nGetAll");
                    var products = irepository.GetAll();
                    foreach (var p in products)
                    {
                        Console.WriteLine("{0} - {1}: {2}", p.CategoryId, p.Id, p.Description);
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            Console.WriteLine("\n{0}", separator);
            Console.Write("Press enter to exit");
            Console.ReadLine();
        }
    }
}
