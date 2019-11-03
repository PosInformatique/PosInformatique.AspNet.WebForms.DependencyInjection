//-----------------------------------------------------------------------
// <copyright file="ServiceProviderAdapter.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace PosInformatique.AspNet.WebForms.DependencyInjection
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Web.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Adapter used to retrieves the service from a <see cref="ServiceCollection"/>
    /// and an existing <see cref="IServiceProvider"/> (optional).
    /// </summary>
    internal sealed class ServiceProviderAdapter : IServiceProvider, IRegisteredObject
    {
        /// <summary>
        /// Existing <see cref="IServiceProvider"/> where to find the service if no service has been found in the <see cref="serviceProvider"/>.
        /// </summary>
        private readonly IServiceProvider nextProvider;

        /// <summary>
        /// Instance of the <see cref="ServiceProvider"/> used to provides the services.
        /// </summary>
        private readonly Lazy<IServiceProvider> serviceProvider;

        /// <summary>
        /// Cache of <see cref="IServiceFactory"/> used to build instances of the services when the service has not been registered
        /// in the <see cref="serviceProvider"/> and not avaiable on the <see cref="nextProvider"/>.
        /// </summary>
        private readonly ConcurrentDictionary<Type, IServiceFactory> factoryServices;

        /// <summary>
        /// Indicates if the current <see cref="ServiceProviderAdapter"/> has been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAdapter"/> class which will build a <see cref="IServiceProvider"/>
        /// from an existing <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"><see cref="IServiceCollection"/> which contains the service to provide in the <see cref="serviceProvider"/>.</param>
        /// <param name="nextProvider">Existing <see cref="IServiceProvider"/> where to find the service if no service
        /// has been found in the <paramref name="serviceCollection"/>.</param>
        public ServiceProviderAdapter(IServiceCollection serviceCollection, IServiceProvider nextProvider)
        {
            this.nextProvider = nextProvider;
            this.factoryServices = new ConcurrentDictionary<Type, IServiceFactory>();

            this.serviceProvider = new Lazy<IServiceProvider>(serviceCollection.BuildServiceProvider, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderAdapter"/> class which retrieve the service from an existing
        /// <see cref="serviceProvider"/>.
        /// </summary>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/> which contains the service to provide in the <see cref="serviceProvider"/>.</param>
        /// <param name="nextProvider">Existing <see cref="IServiceProvider"/> where to find the service if no service
        /// has been found in the <paramref name="serviceProvider"/>.</param>
        public ServiceProviderAdapter(IServiceProvider serviceProvider, IServiceProvider nextProvider)
        {
            this.nextProvider = nextProvider;
            this.factoryServices = new ConcurrentDictionary<Type, IServiceFactory>();

            this.serviceProvider = new Lazy<IServiceProvider>(() => new NonDisposableServiceProvider(serviceProvider), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Represents a factory to creates instance of a service.
        /// </summary>
        private interface IServiceFactory
        {
            /// <summary>
            /// Creates an instance of the <paramref name="serviceType"/>.
            /// </summary>
            /// <param name="serviceProvider"><see cref="IServiceProvider"/> used to retrieve additional dependent services.</param>
            /// <param name="serviceType">Type of the service to create.</param>
            /// <returns>An instance of the <paramref name="serviceType"/>.</returns>
            object Create(IServiceProvider serviceProvider, Type serviceType);
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            var service = this.serviceProvider.Value.GetService(serviceType);

            if (service != null)
            {
                return service;
            }

            // Try the next provider if specified
            if (this.nextProvider != null)
            {
                service = this.nextProvider.GetService(serviceType);

                if (service != null)
                {
                    return service;
                }
            }

            // Creates an instance of the service
            // Some Microsoft modules and service have an internal constructor (like OutputCacheModule,...)
            // we can not use the CreateInstance() method which required public constructor.
            // So we will manually instantiate them by determine if the type have only not public constructor.
            // We use factories (created by the MS ActivatorUtilities class or manually instantiate for type with internal constructors)
            // to reuse it for performance reason to avoid performance issues...
            var factory = this.factoryServices.GetOrAdd(serviceType, this.BuildFactoryObject);

            return factory.Create(this.serviceProvider.Value, serviceType);
        }

        /// <summary>
        /// Called by the <see cref="HostingEnvironment"/> infrastructure when the application is stopped
        /// to dispose the <see cref="serviceProvider"/>.
        /// </summary>
        /// <param name="immediate"><see langword="true"/> to indicate the registered object should unregister from the hosting environment
        ///  before returning; otherwise, <see langword="false"/>.</param>
        public void Stop(bool immediate)
        {
            if (this.serviceProvider.IsValueCreated)
            {
                if (this.serviceProvider.Value is IDisposable disposableServiceProvider)
                {
                    disposableServiceProvider.Dispose();
                }
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// Creates an implementation of the <see cref="IServiceFactory"/>
        /// depending if the <paramref name="serviceType"/> is publicy visible.
        /// </summary>
        /// <param name="serviceType">Type of the service which will be build by the <see cref="IServiceFactory"/>.</param>
        /// <returns>An implementation of the <see cref="IServiceFactory"/>
        /// depending if the <paramref name="serviceType"/> is publicy visible.</returns>
        private IServiceFactory BuildFactoryObject(Type serviceType)
        {
            if (!serviceType.IsVisible)
            {
                return NotVisibleConstructorServiceFactory.Instance;
            }

            if (!serviceType.GetConstructors().Any(constructor => constructor.IsPublic))
            {
                return NotVisibleConstructorServiceFactory.Instance;
            }

            return DefaultServiceFactory.Instance;
        }

        /// <summary>
        /// Wraps a <see cref="IServiceProvider"/> and avoid to be <see cref="IDisposable"/> to be called
        /// by the <see cref="IDisposable.Dispose"/> method of the <see cref="ServiceProviderAdapter"/>.
        /// </summary>
        /// <remarks>This implementation is used when the developer want to use a <see cref="IServiceProvider"/>
        /// which is not builded by the <see cref="ServiceProviderAdapter"/> and so it is not managed
        /// and responsability of this library to call the <see cref="IDisposable.Dispose"/>.</remarks>
        private class NonDisposableServiceProvider : IServiceProvider
        {
            /// <summary>
            /// <see cref="IServiceProvider"/> to wrap.
            /// </summary>
            private readonly IServiceProvider serviceProvider;

            /// <summary>
            /// Initializes a new instance of the <see cref="NonDisposableServiceProvider"/> class which wrap the
            /// <paramref name="serviceProvider"/>.
            /// </summary>
            /// <param name="serviceProvider"><see cref="IServiceProvider"/> to wrap.</param>
            public NonDisposableServiceProvider(IServiceProvider serviceProvider)
            {
                this.serviceProvider = serviceProvider;
            }

            /// <inheritdoc />
            public object GetService(Type serviceType)
            {
                return this.serviceProvider.GetService(serviceType);
            }
        }

        /// <summary>
        /// Implementation of the <see cref="IServiceFactory"/> to use the <see cref="ActivatorUtilities.CreateInstance(IServiceProvider, Type, object[])"/> method
        /// to build instance of the service.
        /// </summary>
        private class DefaultServiceFactory : IServiceFactory
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultServiceFactory"/> class.
            /// </summary>
            private DefaultServiceFactory()
            {
            }

            /// <summary>
            /// Gets the singleton instance of the <see cref="DefaultServiceFactory"/> class.
            /// </summary>
            public static DefaultServiceFactory Instance { get; } = new DefaultServiceFactory();

            /// <inheritdoc />
            public object Create(IServiceProvider serviceProvider, Type serviceType)
            {
                return ActivatorUtilities.CreateInstance(serviceProvider, serviceType);
            }
        }

        /// <summary>
        /// Implementation of the <see cref="IServiceFactory"/> to use the <see cref="Activator.CreateInstance(Type, bool)"/> method
        /// to build instance of the service.
        /// </summary>
        private class NotVisibleConstructorServiceFactory : IServiceFactory
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NotVisibleConstructorServiceFactory"/> class.
            /// </summary>
            private NotVisibleConstructorServiceFactory()
            {
            }

            /// <summary>
            /// Gets the singleton instance of the <see cref="NotVisibleConstructorServiceFactory"/> class.
            /// </summary>
            public static NotVisibleConstructorServiceFactory Instance { get; } = new NotVisibleConstructorServiceFactory();

            /// <inheritdoc />
            public object Create(IServiceProvider serviceProvider, Type serviceType)
            {
                return Activator.CreateInstance(serviceType, true);
            }
        }
    }
}
