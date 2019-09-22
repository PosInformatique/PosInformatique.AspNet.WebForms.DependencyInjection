using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace PosInformatique.AspNet.WebForms.DependencyInjection.IntegrationTests
{
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
}