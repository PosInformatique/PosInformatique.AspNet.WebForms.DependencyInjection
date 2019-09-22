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
WebForms project call the `AddServiceCollection` the following lines in the constructor of your `HttpApplication` class in the
`Global.asax.cs` code behind:
```csharp
public class Global : HttpApplication
{
    public Global()
    {
        ServicesConfig.RegisterServices(this.AddServiceCollection());
    }

    void Application_Start(object sender, EventArgs e)
    {
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
        public static void RegisterServices(ServiceCollection serviceCollection)
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

## Contributions
Do not hesitate to clone my code and submit some changes...
It is a open source project, so everyone is welcome to improve this library...
By the way, I am french... So maybe you will remarks that my english is not really fluent...
So do not hesitate to fix my resources strings or my documentation... Merci !

## Thanks
I want to thank the [DiliTrust](https://www.dilitrust.com/) company to test and gave me their
feedback of this library for their ASP .NET WebForms applications.