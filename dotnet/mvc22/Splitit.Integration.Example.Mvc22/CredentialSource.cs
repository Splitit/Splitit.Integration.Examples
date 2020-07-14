using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Splitit.Integration.Example.Mvc22.Models;

namespace Splitit.Integration.Example.Mvc21
{
    public class CredentialSource
    {
        private IConfiguration _configuration;
        private HttpContext _httpContext;

        private IConfigurationSection CurrentEnvConfig
		{
            get
			{
                return this._configuration.GetSection(this.Environment);
			}
		}

        public CredentialSource(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            this._httpContext = httpContextAccessor.HttpContext;
        }

        private string GetCookieValue(string key)
        {
            if (this._httpContext.Request.Cookies.ContainsKey(key))
            {
                return this._httpContext.Request.Cookies[key];
            }

            return null;
        }

        public string Environment
        {
            get
            {
                return GetCookieValue(nameof(SetCredentialsModel.Environment)) ?? ParseEnvironment(this._httpContext);
            }
        }

        public static string ParseEnvironment(HttpContext context)
		{
            if (context.Request.Host.Host.Contains(".dev."))
            {
                return "dev";
            }

            if (context.Request.Host.Host.Contains(".stg."))
            {
                return "Staging";
            }

            if (context.Request.Host.Host.Contains(".sandbox."))
            {
                return "Sandbox";
            }

            if (context.Request.Host.Host.Contains(".production."))
            {
                return "Production";
            }

            return "local";
        }

        public string SplititApiUrl
            => GetCookieValue(nameof(SetCredentialsModel.SplititApiUrl)) ?? CurrentEnvConfig["SplititApiUrl"];

        public string PaymentFormEmbedderUrlRoot
            => GetCookieValue(nameof(SetCredentialsModel.PaymentFormEmbedderUrlRoot)) ?? CurrentEnvConfig["PaymentFormEmbedderUrlRoot"];

        public string SplititApiKey
            => GetCookieValue(nameof(SetCredentialsModel.SplititApiKey)) ?? this.CurrentEnvConfig["SplititApiKey"];

        public string SplititApiUsername
            => GetCookieValue(nameof(SetCredentialsModel.SplititApiUsername)) ?? this.CurrentEnvConfig["SplititApiUsername"];
        public string SplititApiPassword
            => GetCookieValue(nameof(SetCredentialsModel.SplititApiPassword)) ?? this.CurrentEnvConfig["SplititApiPassword"];

        public string FlexFieldsUrlRoot
            => GetCookieValue(nameof(SetCredentialsModel.FlexFieldsUrlRoot)) ?? this.CurrentEnvConfig["FlexFieldsUrlRoot"];

        public string UpstreamUrlRoot
           => GetCookieValue(nameof(SetCredentialsModel.UpstreamUrlRoot)) ?? this.CurrentEnvConfig["UpstreamUrlRoot"];
    }
}
