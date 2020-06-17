using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Splitit.Integration.Example.Mvc21;
using Splitit.SDK.Client.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Splitit.Integration.Example.Mvc22.Controllers
{
    public class ExampleControllerBase : Controller
    {
        protected CredentialSource CredentialSource;

        protected Configuration FlexFieldsEnv { get; }

        public string SplititApiUrl => this.CredentialSource.SplititApiUrl;
        public string SplititApiKey => this.CredentialSource.SplititApiKey;
        public string SplititApiUsername => this.CredentialSource.SplititApiUsername;
        public string SplititApiPassword => this.CredentialSource.SplititApiPassword;

        public ExampleControllerBase(CredentialSource credentialSource)
        {
            this.CredentialSource = credentialSource;

            if (SplititApiUrl.Contains("production"))
            {
                this.FlexFieldsEnv = Configuration.Default;
            } 
            else
            {
                this.FlexFieldsEnv = Configuration.Sandbox;
            }

            this.FlexFieldsEnv.AddApiKey(SplititApiKey);
            this.FlexFieldsEnv.BasePath = SplititApiUrl;
        }
    }
}
