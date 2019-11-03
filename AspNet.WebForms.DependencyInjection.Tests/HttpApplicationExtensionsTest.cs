//-----------------------------------------------------------------------
// <copyright file="HttpApplicationExtensionsTest.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace System.Web.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Web.Hosting;
    using System.Web.SessionState;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using PosInformatique.AspNet.WebForms.DependencyInjection;
    using PosInformatique.AspNet.WebForms.DependencyInjection.Tests;
    using Xunit;

    public class HttpApplicationExtensionsTest : IDisposable
    {
        private readonly IServiceProvider existingProvider;

        public HttpApplicationExtensionsTest()
        {
            this.existingProvider = Mock.Of<IServiceProvider>();
            HttpRuntime.WebObjectActivator = this.existingProvider;

            if (!HostingEnvironment.IsHosted)
            {
                new HostingEnvironment();
            }
        }

        [Fact]
        public void AddServiceCollection()
        {
            // Creates a HttpContext
            var application = new HttpApplicationMock();
            var request = new HttpRequest("The filename", "http://theurl", "");
            var response = new HttpResponse(new StringWriter());
            var session = FormatterServices.GetUninitializedObject(typeof(HttpSessionState));

            HttpContext.Current = new HttpContext(request, response)
            {
                ApplicationInstance = application,
                Items =
                {
                    { "AspSession", session }
                }
            };

            try
            {
                var collection = HttpApplicationExtensions.AddServiceCollection(application);
                collection.AddSingleton<IService, Service>();
                collection.AddSingleton<IDependentService, DependentService>();

                collection.Should().NotBeNull();

                // Checks the WebObjectActivator has been defined correctly
                HttpRuntime.WebObjectActivator.Should().BeOfType<ServiceProviderAdapter>();

                // Checks the services
                HttpRuntime.WebObjectActivator.GetService(typeof(IService)).As<Service>().Should().NotBeNull();
                HttpRuntime.WebObjectActivator.GetService(typeof(HttpApplication)).Should().BeSameAs(application);
                HttpRuntime.WebObjectActivator.GetService(typeof(HttpApplicationMock)).Should().BeSameAs(application);
                HttpRuntime.WebObjectActivator.GetService(typeof(HttpRequest)).Should().BeSameAs(request);
                HttpRuntime.WebObjectActivator.GetService(typeof(HttpResponse)).Should().BeSameAs(response);
                HttpRuntime.WebObjectActivator.GetService(typeof(HttpSessionState)).Should().BeSameAs(session);

                // Checks the next provider in the adapter has been defined correctly
                HttpRuntime.WebObjectActivator.GetFieldValue<IServiceProvider>("nextProvider").Should().BeSameAs(this.existingProvider);

                // Checks the adapter has been registered on the hosting environment infrastructure
                typeof(HostingEnvironment).GetStaticValue<object>("_theHostingEnvironment").GetFieldValue<Hashtable>("_registeredObjects").ContainsKey(HttpRuntime.WebObjectActivator);
                typeof(HostingEnvironment).GetStaticValue<object>("_theHostingEnvironment").GetFieldValue<Hashtable>("_registeredObjects").ContainsValue(HttpRuntime.WebObjectActivator);
            }
            finally
            {
                HttpContext.Current = null;
            }
        }

        [Fact]
        public void AddServiceCollection_WithExistingCollection()
        {
            // Creates a HttpContext
            var application = new HttpApplicationMock();
            var request = new HttpRequest("The filename", "http://theurl", "");
            var response = new HttpResponse(new StringWriter());
            var session = FormatterServices.GetUninitializedObject(typeof(HttpSessionState));

            HttpContext.Current = new HttpContext(request, response)
            {
                ApplicationInstance = application,
                Items =
                {
                    { "AspSession", session }
                }
            };

            try
            {
                var collection = new ServiceCollection();
                collection.AddSingleton<IService, Service>();
                collection.AddSingleton<IDependentService, DependentService>();

                var returnedCollection = HttpApplicationExtensions.AddServiceCollection(application, collection);

                returnedCollection.Should().BeSameAs(collection);

                // Checks the WebObjectActivator has been defined correctly
                HttpRuntime.WebObjectActivator.Should().BeOfType<ServiceProviderAdapter>();

                // Checks the services
                HttpRuntime.WebObjectActivator.GetService(typeof(IService)).As<Service>().Should().NotBeNull();
                HttpRuntime.WebObjectActivator.GetService(typeof(HttpApplication)).Should().BeSameAs(application);
                HttpRuntime.WebObjectActivator.GetService(typeof(HttpApplicationMock)).Should().BeSameAs(application);
                HttpRuntime.WebObjectActivator.GetService(typeof(HttpRequest)).Should().BeSameAs(request);
                HttpRuntime.WebObjectActivator.GetService(typeof(HttpResponse)).Should().BeSameAs(response);
                HttpRuntime.WebObjectActivator.GetService(typeof(HttpSessionState)).Should().BeSameAs(session);

                // Checks the next provider in the adapter has been defined correctly
                HttpRuntime.WebObjectActivator.GetFieldValue<IServiceProvider>("nextProvider").Should().BeSameAs(this.existingProvider);

                // Checks the adapter has been registered on the hosting environment infrastructure
                typeof(HostingEnvironment).GetStaticValue<object>("_theHostingEnvironment").GetFieldValue<Hashtable>("_registeredObjects").ContainsKey(HttpRuntime.WebObjectActivator);
                typeof(HostingEnvironment).GetStaticValue<object>("_theHostingEnvironment").GetFieldValue<Hashtable>("_registeredObjects").ContainsValue(HttpRuntime.WebObjectActivator);
            }
            finally
            {
                HttpContext.Current = null;
            }
        }

        [Fact]
        public void AddServiceCollection_WithNoExistingServiceCollectionAndWithNullApplication_ExceptionThrown()
        {
            Action act = () =>
            {
                HttpApplicationExtensions.AddServiceCollection<HttpApplication>(null);
            };

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("application");
        }

        [Fact]
        public void AddServiceCollection_WithExistingServiceCollectionAndWithNullApplication_ExceptionThrown()
        {
            Action act = () =>
            {
                HttpApplicationExtensions.AddServiceCollection<HttpApplication>(null, null);
            };

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("application");
        }

        [Fact]
        public void AddServiceCollection_WithExistingServiceCollectionAndWithNullServiceCollection_ExceptionThrown()
        {
            Action act = () =>
            {
                HttpApplicationExtensions.AddServiceCollection(new HttpApplication(), null);
            };

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("serviceCollection");
        }

        [Fact]
        public void UseServiceProvider()
        {
            var application = new HttpApplicationMock();

            var service = Mock.Of<IService>();

            var serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            serviceProvider.Setup(sp => sp.GetService(typeof(IService)))
                .Returns(service);

            var returnedApplication = HttpApplicationExtensions.UseServiceProvider(application, serviceProvider.Object);

            returnedApplication.Should().BeSameAs(application);

            // Checks the WebObjectActivator has been defined correctly
            HttpRuntime.WebObjectActivator.Should().BeOfType<ServiceProviderAdapter>();

            // Checks the services
            HttpRuntime.WebObjectActivator.GetService(typeof(IService)).Should().BeSameAs(service);

            // Checks the next provider in the adapter has been defined correctly
            HttpRuntime.WebObjectActivator.GetFieldValue<IServiceProvider>("nextProvider").Should().BeSameAs(this.existingProvider);

            // Checks the adapter has been registered on the hosting environment infrastructure
            typeof(HostingEnvironment).GetStaticValue<object>("_theHostingEnvironment").GetFieldValue<Hashtable>("_registeredObjects").ContainsKey(HttpRuntime.WebObjectActivator);
            typeof(HostingEnvironment).GetStaticValue<object>("_theHostingEnvironment").GetFieldValue<Hashtable>("_registeredObjects").ContainsValue(HttpRuntime.WebObjectActivator);
        }

        [Fact]
        public void UseServiceProvider_WithNullApplication_ExceptionThrown()
        {
            Action act = () =>
            {
                HttpApplicationExtensions.UseServiceProvider<HttpApplication>(null, null);
            };

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("application");
        }

        [Fact]
        public void UseServiceProvider_WithNullServiceProvider_ExceptionThrown()
        {
            Action act = () =>
            {
                HttpApplicationExtensions.UseServiceProvider(new HttpApplication(), null);
            };

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("serviceProvider");
        }

        [Fact]
        public void AddDefaultAspNetServices()
        {
            // Creates a HttpContext
            var application = new HttpApplicationMock();
            var request = new HttpRequest("The filename", "http://theurl", "");
            var response = new HttpResponse(new StringWriter());
            var session = FormatterServices.GetUninitializedObject(typeof(HttpSessionState));

            HttpContext.Current = new HttpContext(request, response)
            {
                ApplicationInstance = application,
                Items =
                {
                    { "AspSession", session }
                }
            };

            try
            {
                var collection = new ServiceCollection();

                var returnedCollection = HttpApplicationExtensions.AddDefaultAspNetServices(collection, application);

                returnedCollection.Should().BeSameAs(collection);

                // Builds the service provider and check the registered services
                var serviceProvider = collection.BuildServiceProvider();

                // Checks the services
                serviceProvider.GetService(typeof(HttpApplication)).Should().BeSameAs(application);
                serviceProvider.GetService(typeof(HttpApplicationMock)).Should().BeSameAs(application);
                serviceProvider.GetService(typeof(HttpRequest)).Should().BeSameAs(request);
                serviceProvider.GetService(typeof(HttpResponse)).Should().BeSameAs(response);
                serviceProvider.GetService(typeof(HttpSessionState)).Should().BeSameAs(session);
            }
            finally
            {
                HttpContext.Current = null;
            }
        }

        [Fact]
        public void AddDefaultAspNetServices_WithNullServiceCollection_ExceptionThrown()
        {
            Action act = () =>
            {
                HttpApplicationExtensions.AddDefaultAspNetServices<HttpApplication>(null, null);
            };

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("services");
        }

        [Fact]
        public void AddDefaultAspNetServices_WithNullHttpApplication_ExceptionThrown()
        {
            Action act = () =>
            {
                HttpApplicationExtensions.AddDefaultAspNetServices<HttpApplication>(Mock.Of<IServiceCollection>(), null);
            };

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("application");
        }

        public void Dispose()
        {
            HttpRuntime.WebObjectActivator = null;
        }

        public interface IService
        {
        }

        public class Service : IService
        {
            public Service(IDependentService dependentService)
            {
            }
        }

        public interface IDependentService
        {
        }

        public class DependentService : IDependentService
        {
        }

        public class HttpApplicationMock : HttpApplication
        {
        }
    }
}