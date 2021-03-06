﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Extensions.DependencyInjection;

namespace PosInformatique.AspNet.WebForms.DependencyInjection.IntegrationTests
{
    public partial class _Default : Page
    {
        private readonly IDogManager dogManager;

        private readonly HttpRequest httpRequest;

        [ActivatorUtilitiesConstructor]
        public _Default(IDogManager dogManager, HttpRequest httpRequest)
        {
            this.dogManager = dogManager;
            this.httpRequest = httpRequest;
        }

        public _Default()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.doggoList.DataSource = this.dogManager.GetDogs();
            this.doggoList.DataBind();

            this.urlFromHttpRequest.Text = this.httpRequest.Url.ToString();
        }
    }
}