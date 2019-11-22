using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Extensions.DependencyInjection;

namespace PosInformatique.AspNet.WebForms.DependencyInjection.IntegrationTests
{
    public partial class UserControlWithDependency : System.Web.UI.UserControl
    {
        private readonly IDogManager dogManager;

        [ActivatorUtilitiesConstructor]
        public UserControlWithDependency(IDogManager dogManager)
        {
            this.dogManager = dogManager;
        }

        public UserControlWithDependency(IDogManager dogManager, IDogRepository dogRepository)
        {
            throw new InvalidOperationException("Must not be called");
        }

        public UserControlWithDependency()
        {
            throw new InvalidOperationException("Must not be called");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.doggoList.DataSource = this.dogManager.GetDogs();
            this.doggoList.DataBind();
        }
    }
}