# PosInformatique.AspNet.WebForms.DependencyInjection
PosInformatique.AspNet.WebForms.DependencyInjection is a library to add the IoC container support of Microsoft.Extensions.DependencyInjection for ASP .NET Web Forms

## Installing from NuGet
The **PosInformatique.AspNet.WebForms.DependencyInjection** is available directly on the
[NuGet](https://www.nuget.org/packages/PosInformatique.AspNet.WebForms.DependencyInjection/) official website.
To download and install the library to your Visual Studio project using the following NuGet command line 
```
Install-Package PosInformatique.AspNet.WebForms.DependencyInjection
```

## Setting up
After adding the **PosInformatique.AspNet.WebForms.DependencyInjection** package on your ASP .NET
WebForms project call the `AddServiceCollection` the following lines in the ``Application_Start`` of your `HttpApplication` class in the
`Global.asax.cs` code behind:
```csharp
public class Global : HttpApplication
{
    protected void Application_Start(Object sender, EventArgs e)
    {
        ServicesConfig.RegisterServices(this.AddServiceCollection());

        // Code that runs on application startup
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
}
```

In the `App_Start` folder add a new static class called `ServicesConfig` which allows to register
the services using the `Microsoft.Extensions.DependencyInjection.ServiceCollection`:
```csharp
namespace PosInformatique.AspNet.WebForms.DependencyInjection.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServicesConfig
    {
        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDogRepository, DogRepository>();
            serviceCollection.AddTransient<IDogManager, DogManager>();
        }
    }
}
```
You can register services with a *Transient* or *Singleton* scope. Unlike to ASP .NET Core,
**PosInformatique.AspNet.WebForms.DependencyInjection** does not support *Scoped* scope services
which exists during the HTTP request lifetime.

## Setting up using an existing IServiceProvider/IServiceCollection
If in your ASP .NET application you host the ASP .NET Core infrastructure (using the
[PosInformatique.AspNetCore.Server.AspNet](https://github.com/PosInformatique/PosInformatique.AspNetCore.Server.AspNet)
library for example), you can reuse the ``IServiceProvider`` and ``IServiceCollection``
internally builded by the ASP .NET Core infrastructure to share the same services
(and same singleton instances !) between your ASP .NET application and ASP .NET Core hosted application.

The following example, show how to reuse the internal ``IServiceProvider`` and ``IServiceCollection``
of ASP .NET Core application hosted in ASP .NET using the
[PosInformatique.AspNetCore.Server.AspNet](https://github.com/PosInformatique/PosInformatique.AspNetCore.Server.AspNet)
library:

```csharp
public class Global : System.Web.HttpApplication
{
    protected void Application_Start(object sender, EventArgs e)
    {
        var host = WebHost.CreateDefaultBuilder()
            .UseAspNet(options =>
            {
                options.Routes.Add("api");
                options.Routes.Add("swagger");
            })
            .ConfigureServices(services =>
            {
                // Add the default ASP .NET non-core services
                // in the IServiceCollection of ASP .NET core.
                services.AddDefaultAspNetServices(this);
            })
            .UseStartup<Startup>()
            .Start();

        // Reuse the built IServiceProvider of ASP .NET Core WebHost inside
        // the ASP .NET non-core infrastructure to use IoC feature.
        this.UseServiceProvider(host);
    }
}
```

The ``AddDefaultAspNetServices()`` method allows to add ASP .NET non-core infrastructure
service like the ``HttpRequest`` or ``HttpContext`` into the ``IServiceCollection`` which
will be available in the ``IServiceProvider`` built by the ASP .NET core infrastructure.

The ``UseServiceProvider()`` method allows to setup the ASP .NET non-core infrastructure
to use the implementation of ``IServiceProvider`` which is builted by the ``IWebHost``
of the ASP .NET Core infrastructure started.

With this approach the ASP .NET non-core and Core components will share the **same** service.
For example, if you register a ``IDogManager`` service as singleton in the services of ASP .NET Core,
the ``IDogManager`` service instance will be available and **the same instance**:
- In the constructors of the ASP .NET Web Forms pages, user controls,...
- In the constructors of the ASP .NET Core controllers
- And so on...

## Contributions
Do not hesitate to clone my code and submit some changes...
It is a open source project, so everyone is welcome to improve this library...
By the way, I am french... So maybe you will remarks that my english is not really fluent...
So do not hesitate to fix my resources strings or my documentation... Merci !

## Thanks
I want to thank the [DiliTrust](https://www.dilitrust.com/) company to test and gave me their
feedback of this library for their ASP .NET WebForms applications.