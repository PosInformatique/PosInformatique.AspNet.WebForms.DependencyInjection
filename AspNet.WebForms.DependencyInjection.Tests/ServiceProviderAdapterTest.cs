using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace PosInformatique.AspNet.WebForms.DependencyInjection.Tests
{
    public class ServiceProviderAdapterTest
    {
        [Fact]
        public void GetService_UsingServiceCollection_InServiceCollectionWithInjection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IService, Service>();
            serviceCollection.AddSingleton<IDependentService, DependentService>();

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceCollection, null);

            var result = serviceProviderAdapter.GetService(typeof(IService)).As<Service>();

            result.Should().NotBeNull();
            result.DependentService.Should().NotBeNull();

            // Calls again to check the internal cache have not problem
            var result2 = serviceProviderAdapter.GetService(typeof(IService)).As<Service>();

            result2.Should().NotBeNull();
            result2.DependentService.Should().NotBeNull();
        }

        [Fact]
        public void GetService_UsingServiceCollection_NotInServiceCollectionUsingNextProvider()
        {
            var serviceCollection = new ServiceCollection();

            var service = new Service(null);

            var nextServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nextServiceProvider.Setup(sp => sp.GetService(typeof(IService)))
                .Returns(service);

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceCollection, nextServiceProvider.Object);

            var result = serviceProviderAdapter.GetService(typeof(IService)).As<Service>();

            result.Should().BeSameAs(service);
        }

        [Fact]
        public void GetService_UsingServiceCollection_NotInServiceCollectionAndNotInTheNextProvider()
        {
            var serviceCollection = new ServiceCollection();

            var nextServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nextServiceProvider.Setup(sp => sp.GetService(typeof(DependentService)))
                .Returns(null);

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceCollection, nextServiceProvider.Object);

            var result = serviceProviderAdapter.GetService(typeof(DependentService)).As<DependentService>();

            result.Should().NotBeNull();
        }

        [Fact]
        public void GetService_UsingServiceCollection_WithInstantiationWithInjection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDependentService, DependentService>();

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceCollection, null);

            var result = serviceProviderAdapter.GetService(typeof(Service)).As<Service>();

            result.Should().NotBeNull();
            result.DependentService.Should().NotBeNull();

            // Calls again to check the internal cache have not problem
            var result2 = serviceProviderAdapter.GetService(typeof(Service)).As<Service>();

            result2.Should().NotBeNull();
            result2.DependentService.Should().NotBeNull();
        }

        [Fact]
        public void GetService_UsingServiceCollection_WithInstantiationInternalConstructor()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDependentService, DependentService>();

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceCollection, null);

            var result = serviceProviderAdapter.GetService(typeof(ServiceWithInternalConstructor)).As<ServiceWithInternalConstructor>();

            result.Should().NotBeNull();

            // Calls again to check the internal cache have not problem
            var result2 = serviceProviderAdapter.GetService(typeof(ServiceWithInternalConstructor)).As<ServiceWithInternalConstructor>();

            result2.Should().NotBeNull();
        }

        [Fact]
        public void GetService_UsingServiceCollection_WithInstantiationInternalClass()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDependentService, DependentService>();

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceCollection, null);

            var result = serviceProviderAdapter.GetService(typeof(ServiceInternal)).As<ServiceInternal>();

            result.Should().NotBeNull();

            // Calls again to check the internal cache have not problem
            var result2 = serviceProviderAdapter.GetService(typeof(ServiceInternal)).As<ServiceInternal>();

            result2.Should().NotBeNull();
        }

        [Fact]
        public void Stop_UsingServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDependentService, DependentService>();

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceCollection, null);

            var result = serviceProviderAdapter.GetService(typeof(IDependentService)).As<DependentService>();

            result.Should().NotBeNull();
            result.DisposeCalled.Should().BeFalse();

            // Stop() the adapter, it is mean we dispose the provider
            serviceProviderAdapter.Stop(true);

            result.DisposeCalled.Should().BeTrue();

            // Check the ObjectDisposedException if calling the GetService() method
            serviceProviderAdapter.Invoking(spa => spa.GetService(typeof(IDependentService))).Should().ThrowExactly<ObjectDisposedException>()
                .And.ObjectName.Should().Be("PosInformatique.AspNet.WebForms.DependencyInjection.ServiceProviderAdapter");
        }

        [Fact]
        public void GetService_UsingServiceProvider_InServiceCollectionWithInjection()
        {
            var service1 = Mock.Of<IService>();
            var service2 = Mock.Of<IService>();

            var serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProvider.SetupSequence(sp => sp.GetService(typeof(IService)))
                .Returns(service1)
                .Returns(service2);

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceProvider.Object, null);

            serviceProviderAdapter.GetService(typeof(IService)).Should().BeSameAs(service1);

            // Calls again to check the internal cache have not problem
            serviceProviderAdapter.GetService(typeof(IService)).Should().BeSameAs(service2);
        }

        [Fact]
        public void GetService_UsingServiceProvider_NotInServiceCollectionUsingNextProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProvider.Setup(sp => sp.GetService(typeof(IService)))
                .Returns(null);

            var service = new Service(null);

            var nextServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nextServiceProvider.Setup(sp => sp.GetService(typeof(IService)))
                .Returns(service);

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceProvider.Object, nextServiceProvider.Object);

            var result = serviceProviderAdapter.GetService(typeof(IService)).As<Service>();

            result.Should().BeSameAs(service);
        }

        [Fact]
        public void GetService_UsingServiceProvider_NotInServiceCollectionAndNotInTheNextProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProvider.Setup(sp => sp.GetService(typeof(DependentService)))
                .Returns(null);

            var nextServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nextServiceProvider.Setup(sp => sp.GetService(typeof(DependentService)))
                .Returns(null);

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceProvider.Object, nextServiceProvider.Object);

            var result = serviceProviderAdapter.GetService(typeof(DependentService));

            result.Should().NotBeNull();
        }

        [Fact]
        public void Stop_UsingServiceProvider()
        {
            var dependentService = Mock.Of<IDependentService>();

            var serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProvider.As<IDisposable>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IDependentService)))
                .Returns(dependentService);

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceProvider.Object, null);

            serviceProviderAdapter.GetService(typeof(IDependentService)).As<DependentService>();

            // Stop() the adapter, it is mean we dispose the provider BUT the no 
            serviceProviderAdapter.Stop(true);

            // Verify the Dispose() method is not called for a IServiceProvider which implement the IDisposable interface
            serviceProvider.As<IDisposable>().Verify(sp => sp.Dispose(), Times.Never);

            // Check the ObjectDisposedException if calling the GetService() method
            serviceProviderAdapter.Invoking(spa => spa.GetService(typeof(IDependentService))).Should().ThrowExactly<ObjectDisposedException>()
                .And.ObjectName.Should().Be("PosInformatique.AspNet.WebForms.DependencyInjection.ServiceProviderAdapter");
        }

        public interface IService
        {
        }

        public class Service : IService
        {
            public Service(IDependentService dependentService)
            {
                this.DependentService = dependentService;
            }

            public IDependentService DependentService
            {
                get;
                set;
            }
        }

        public interface IDependentService
        {
        }

        public class DependentService : IDependentService, IDisposable
        {
            public bool DisposeCalled
            {
                get;
                private set;
            }

            public void Dispose()
            {
                this.DisposeCalled = true;
            }
        }

        public class ServiceWithInternalConstructor
        {
            internal ServiceWithInternalConstructor()
            {
            }
        }

        internal class ServiceInternal
        {
            internal ServiceInternal()
            {
            }
        }
    }
}
