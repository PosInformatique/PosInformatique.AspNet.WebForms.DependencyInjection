//-----------------------------------------------------------------------
// <copyright file="HttpApplicationExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace System.Web
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Web.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using PosInformatique.AspNet.WebForms.DependencyInjection;

    /// <summary>
    /// Contains extension methods to register a <see cref="IServiceCollection"/> as <see cref="IServiceProvider"/>
    /// for the <see cref="HttpRuntime.WebObjectActivator"/> provider services.
    /// </summary>
    public static class HttpApplicationExtensions
    {
        /// <summary>
        /// Creates a new <see cref="IServiceCollection"/> which will define as a <see cref="IServiceProvider"/>
        /// for the <see cref="HttpRuntime.WebObjectActivator"/> provider services of the <typeparamref name="TApplication"/>.
        /// </summary>
        /// <typeparam name="TApplication">Type of the <see cref="HttpApplication"/> which the <see cref="IServiceProvider"/>
        /// will be create from a <see cref="IServiceCollection"/>.</typeparam>
        /// <param name="application">The <see cref="HttpApplication"/> which the <see cref="IServiceCollection"/> will be registered on.</param>
        /// <returns>A new <see cref="IServiceCollection"/> which allows to defines the services to provide.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="application"/> argument is <see langword="null"/>.</exception>
        public static IServiceCollection AddServiceCollection<TApplication>(this TApplication application)
            where TApplication : HttpApplication
        {
            var serviceCollection = new ServiceCollection();

            return AddServiceCollection(application, serviceCollection);
        }

        /// <summary>
        /// Add an existing <see cref="IServiceCollection"/> which will define as a <see cref="IServiceProvider"/>
        /// for the <see cref="HttpRuntime.WebObjectActivator"/> provider services of the <typeparamref name="TApplication"/>.
        /// </summary>
        /// <typeparam name="TApplication">Type of the <see cref="HttpApplication"/> which the <see cref="IServiceProvider"/>
        /// will be create from a <see cref="IServiceCollection"/>.</typeparam>
        /// <param name="application">The <see cref="HttpApplication"/> which the <see cref="IServiceCollection"/> will be registered on.</param>
        /// <param name="serviceCollection">Existing <see cref="IServiceCollection"/> which contains the registered services.</param>
        /// <returns>A new <see cref="IServiceCollection"/> which allows to defines the services to provide.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="application"/> argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="serviceCollection"/> argument is <see langword="null"/>.</exception>
        public static IServiceCollection AddServiceCollection<TApplication>(this TApplication application, IServiceCollection serviceCollection)
            where TApplication : HttpApplication
        {
            Check.IsNotNull(application, nameof(application));
            Check.IsNotNull(serviceCollection, nameof(serviceCollection));

            AddDefaultAspNetServices(serviceCollection, application);

            var serviceProvider = new ServiceProviderAdapter(serviceCollection, HttpRuntime.WebObjectActivator);

            HostingEnvironment.RegisterObject(serviceProvider);

            HttpRuntime.WebObjectActivator = serviceProvider;

            return serviceCollection;
        }

        /// <summary>
        /// Configures the <paramref name="application"/> to use the specified existing <paramref name="serviceProvider"/>.
        /// </summary>
        /// <remarks>Use this method to activate IoC from an existing <see cref="IServiceProvider"/> implementation.</remarks>
        /// <typeparam name="TApplication">Type of the <see cref="HttpApplication"/> which the <see cref="IServiceProvider"/>
        /// will be use to retrieve the services using IoC.</typeparam>
        /// <param name="application">The <see cref="HttpApplication"/> which the <see cref="IServiceProvider"/> will be used on.</param>
        /// <param name="serviceProvider">Existing <see cref="IServiceProvider"/> which contains the services.</param>
        /// <returns>The <paramref name="application"/> instance to continue the configuration of it.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="application"/> argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="serviceProvider"/> argument is <see langword="null"/>.</exception>
        public static TApplication UseServiceProvider<TApplication>(this TApplication application, IServiceProvider serviceProvider)
            where TApplication : HttpApplication
        {
            Check.IsNotNull(application, nameof(application));
            Check.IsNotNull(serviceProvider, nameof(serviceProvider));

            var serviceProviderAdapter = new ServiceProviderAdapter(serviceProvider, HttpRuntime.WebObjectActivator);

            HostingEnvironment.RegisterObject(serviceProviderAdapter);

            HttpRuntime.WebObjectActivator = serviceProviderAdapter;

            return application;
        }

        /// <summary>
        /// Add default services of ASP .NET on the specified <paramref name="services"/>.
        /// </summary>
        /// <typeparam name="TApplication">Type of the <see cref="HttpApplication"/> which will be registered in the <see cref="IServiceCollection"/>.</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/> which contains the services to provides to the ASP .NET application.</param>
        /// <param name="application"><see cref="HttpApplication"/> which the ASP .NET services have to be registered on the <see cref="IServiceCollection"/>.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="services"/> argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="application"/> argument is <see langword="null"/>.</exception>
        /// <returns>The <paramref name="services"/> which allows to continue to add additional services.</returns>
        public static IServiceCollection AddDefaultAspNetServices<TApplication>(this IServiceCollection services, TApplication application)
            where TApplication : HttpApplication
        {
            Check.IsNotNull(services, nameof(services));
            Check.IsNotNull(application, nameof(application));

            services.AddTransient(serviceProvider => (TApplication)HttpContext.Current.ApplicationInstance);
            services.AddTransient(serviceProvider => HttpContext.Current.ApplicationInstance);
            services.AddTransient(serviceProvider => HttpContext.Current.Request);
            services.AddTransient(serviceProvider => HttpContext.Current.Response);
            services.AddTransient(serviceProvider => HttpContext.Current.Session);

            return services;
        }
    }
}
