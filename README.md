IoC.Product
===========

The inversion of control allows to configure the effective implementation of the IRepository<ProductEntity>, using Castle Windsor.

Current implemented repositories are: AzureStorage, FileSystem, EntityFramework and Ado.NET.

For the configuration in the webpresentation, uncomment the right component in web.config, while for the configuration for the consoletest you have to write the correct class when creating the windsor container in the class ContractsResolverProvider.
