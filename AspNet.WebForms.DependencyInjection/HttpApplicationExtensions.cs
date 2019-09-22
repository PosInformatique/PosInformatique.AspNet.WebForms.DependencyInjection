//-----------------------------------------------------------------------
// <copyright file="HttpApplicationExtensions.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace PosInformatique.AspNet.WebForms.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Web;
    using System.Web.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Contains extension methods to register a <see cref="ServiceCollection"/> as <see cref="IServiceProvider"/>
    /// for the <see cref="HttpRuntime.WebObjectActivator"/> provider services.
    /// </summary>
    public static class HttpApplicationExtensions
    {
        /// <summary>
        /// Creates a new <see cref="ServiceCollection"/> which will define as a <see cref="IServiceProvider"/>
        /// for the <see cref="HttpRuntime.WebObjectActivator"/> provider services of the <typeparamref name="TApplication"/>.
        /// </summary>
        /// <typeparam name="TApplication">Type of the <see cref="HttpApplication"/> which the <see cref="IServiceProvider"/>
        /// will be create from a <see cref="ServiceCollection"/>.</typeparam>
        /// <param name="application">The <see cref="HttpApplication"/> which the <see cref="ServiceCollection"/> will be registered on.</param>
        /// <returns>A new <see cref="ServiceCollection"/> which allows to defines the services to provide.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="application"/> argument is <see langword="null"/>.</exception>
        public static ServiceCollection AddServiceCollection<TApplication>(this TApplication application)
            where TApplication : HttpApplication
        {
            var serviceCollection = new ServiceCollection();

            return AddServiceCollection(application, serviceCollection);
        }

        /// <summary>
        /// Add an existing <see cref="ServiceCollection"/> which will define as a <see cref="IServiceProvider"/>
        /// for the <see cref="HttpRuntime.WebObjectActivator"/> provider services of the <typeparamref name="TApplication"/>.
        /// </summary>
        /// <typeparam name="TApplication">Type of the <see cref="HttpApplication"/> which the <see cref="IServiceProvider"/>
        /// will be create from a <see cref="ServiceCollection"/>.</typeparam>
        /// <param name="application">The <see cref="HttpApplication"/> which the <see cref="ServiceCollection"/> will be registered on.</param>
        /// <param name="serviceCollection">Existings <see cref="ServiceCollection"/> which contains the registered services.</param>
        /// <returns>A new <see cref="ServiceCollection"/> which allows to defines the services to provide.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="application"/> argument is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="serviceCollection"/> argument is <see langword="null"/>.</exception>
        public static ServiceCollection AddServiceCollection<TApplication>(this TApplication application, ServiceCollection serviceCollection)
            where TApplication : HttpApplication
        {
            Check.IsNotNull(application, nameof(application));
            Check.IsNotNull(serviceCollection, nameof(serviceCollection));

            RegisterDefaultAspNetServices<TApplication>(serviceCollection);

            var serviceProvider = new ServiceProviderAdapter(HttpRuntime.WebObjectActivator, serviceCollection);

            HostingEnvironment.RegisterObject(serviceProvider);

            HttpRuntime.WebObjectActivator = serviceProvider;

            return serviceCollection;
        }

        /// <summary>
        /// Registers default services of ASP .NET on the specified <paramref name="serviceCollection"/>.
        /// </summary>
        /// <typeparam name="TApplication">Type of the <see cref="HttpApplication"/> which will be registered in the <see cref="ServiceCollection"/>.</typeparam>
        /// <param name="serviceCollection"><see cref="ServiceCollection"/> which contains the services to provides to the ASP .NET application.</param>
        private static void RegisterDefaultAspNetServices<TApplication>(ServiceCollection serviceCollection)
            where TApplication : HttpApplication
        {
            serviceCollection.AddTransient(serviceProvider => (TApplication)HttpContext.Current.ApplicationInstance);
            serviceCollection.AddTransient(serviceProvider => HttpContext.Current.ApplicationInstance);
            serviceCollection.AddTransient(serviceProvider => HttpContext.Current.Request);
            serviceCollection.AddTransient(serviceProvider => HttpContext.Current.Response);
            serviceCollection.AddTransient(serviceProvider => HttpContext.Current.Session);
        }
    }
}
