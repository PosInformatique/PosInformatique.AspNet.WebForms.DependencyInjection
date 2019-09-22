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
        public void GetService_InServiceCollectionWithInjection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IService, Service>();
            serviceCollection.AddSingleton<IDependentService, DependentService>();

            var serviceProviderAdapter = new ServiceProviderAdapter(null, serviceCollection);

            var result = serviceProviderAdapter.GetService(typeof(IService)).As<Service>();

            result.Should().NotBeNull();
            result.DependentService.Should().NotBeNull();

            // Calls again to check the internal cache have not problem
            var result2 = serviceProviderAdapter.GetService(typeof(IService)).As<Service>();

            result2.Should().NotBeNull();
            result2.DependentService.Should().NotBeNull();
        }

        [Fact]
        public void GetService_NotInServiceCollectionUsingNextProvider()
        {
            var serviceCollection = new ServiceCollection();

            var service = new Service(null);

            var nextServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nextServiceProvider.Setup(sp => sp.GetService(typeof(IService)))
                .Returns(service);

            var serviceProviderAdapter = new ServiceProviderAdapter(nextServiceProvider.Object, serviceCollection);

            var result = serviceProviderAdapter.GetService(typeof(IService)).As<Service>();

            result.Should().BeSameAs(service);
        }

        [Fact]
        public void GetService_NotInServiceCollectionAndNotInTheNextProvider()
        {
            var serviceCollection = new ServiceCollection();

            var nextServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nextServiceProvider.Setup(sp => sp.GetService(typeof(DependentService)))
                .Returns(null);

            var serviceProviderAdapter = new ServiceProviderAdapter(nextServiceProvider.Object, serviceCollection);

            var result = serviceProviderAdapter.GetService(typeof(DependentService)).As<DependentService>();

            result.Should().NotBeNull();
        }

        [Fact]
        public void GetService_WithInstantiationWithInjection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDependentService, DependentService>();

            var serviceProviderAdapter = new ServiceProviderAdapter(null, serviceCollection);

            var result = serviceProviderAdapter.GetService(typeof(Service)).As<Service>();

            result.Should().NotBeNull();
            result.DependentService.Should().NotBeNull();

            // Calls again to check the internal cache have not problem
            var result2 = serviceProviderAdapter.GetService(typeof(Service)).As<Service>();

            result2.Should().NotBeNull();
            result2.DependentService.Should().NotBeNull();
        }

        [Fact]
        public void GetService_WithInstantiationInternalConstructor()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDependentService, DependentService>();

            var serviceProviderAdapter = new ServiceProviderAdapter(null, serviceCollection);

            var result = serviceProviderAdapter.GetService(typeof(ServiceWithInternalConstructor)).As<ServiceWithInternalConstructor>();

            result.Should().NotBeNull();

            // Calls again to check the internal cache have not problem
            var result2 = serviceProviderAdapter.GetService(typeof(ServiceWithInternalConstructor)).As<ServiceWithInternalConstructor>();

            result2.Should().NotBeNull();
        }

        [Fact]
        public void GetService_WithInstantiationInternalClass()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDependentService, DependentService>();

            var serviceProviderAdapter = new ServiceProviderAdapter(null, serviceCollection);

            var result = serviceProviderAdapter.GetService(typeof(ServiceInternal)).As<ServiceInternal>();

            result.Should().NotBeNull();

            // Calls again to check the internal cache have not problem
            var result2 = serviceProviderAdapter.GetService(typeof(ServiceInternal)).As<ServiceInternal>();

            result2.Should().NotBeNull();
        }

        [Fact]
        public void Stop()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDependentService, DependentService>();

            var serviceProviderAdapter = new ServiceProviderAdapter(null, serviceCollection);

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
