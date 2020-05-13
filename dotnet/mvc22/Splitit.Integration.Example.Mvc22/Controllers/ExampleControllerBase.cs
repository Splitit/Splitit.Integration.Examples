using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Splitit.SDK.Client.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Splitit.Integration.Example.Mvc22.Controllers
{
    public class ExampleControllerBase : Controller
    {
        protected IConfiguration _configuration;

        protected Configuration FlexFieldsEnv { get; }

        public ExampleControllerBase(IConfiguration configuration)
        {
            this._configuration = configuration;

            if (this._configuration["SplititApiUrl"].Contains("production"))
            {
                this.FlexFieldsEnv = Configuration.Default;
            } 
            else
            {
                this.FlexFieldsEnv = Configuration.Sandbox;
            }

            this.FlexFieldsEnv.AddApiKey(this._configuration["SplititApiKey"]);
            this.FlexFieldsEnv.BasePath = this._configuration["SplititApiUrl"];
        }
    }
}
