using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using IoC.Product.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IoC.Product.ConsoleTest
{
    public class ContractsResolverProvider
    {
        public static readonly ContractsResolverProvider Instance = new ContractsResolverProvider();

        public WindsorContainer Container { get; private set; }

        private ContractsResolverProvider()
        {
            Container = new WindsorContainer();

            Container.Register(Classes.FromThisAssembly(),
                Component.For<IProductRepository>()
                    .ImplementedBy<
                        IoC.Product.AzureStorage.TableStorageProductRepository
                        //IoC.Product.FileSystem.FileSystemProductRepository
                        //IoC.Product.EntityFram.EntityFrameworkProductRepository
                        //IoC.Product.AdoNet.AdoNetProductRepository
                            >());
        }
    }
}