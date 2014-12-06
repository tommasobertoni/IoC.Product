using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IoC.Product.WebPresentation.Providers
{
    public class ContractsResolverProvider
    {
        public static readonly ContractsResolverProvider Instance = new ContractsResolverProvider();

        public WindsorContainer Container { get; private set; }

        private ContractsResolverProvider()
        {
            Container = new WindsorContainer(new XmlInterpreter());
        }
    }
}