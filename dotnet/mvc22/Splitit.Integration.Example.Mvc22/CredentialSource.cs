using System;
using System.Collections.Generic;
using System.Linq;
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

        public CredentialSource(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this._configuration = configuration;
            this._httpContext = httpContextAccessor.HttpContext;
        }

        private bool UseProductionUrl
        {
            get
            {
                return GetCookieValue(nameof(SetCredentialsModel.Environment)) == "Production";
            }
        }

        private string GetCookieValue(string key)
        {
            if (this._httpContext.Request.Cookies.ContainsKey(key))
            {
                return this._httpContext.Request.Cookies[key];
            }

            return null;
        }

        public string SplititApiUrl
        {
            get
            {
                return UseProductionUrl ? this._configuration["SplititApiUrlProduction"] : this._configuration["SplititApiUrl"];
            }
        }

        public string SplititApiKey
        {
            get
            {
                return GetCookieValue(nameof(SetCredentialsModel.SplititApiKey)) ?? this._configuration["SplititApiKey"];
            }
        }

        public string SplititApiUsername
        {
            get
            {
                return GetCookieValue(nameof(SetCredentialsModel.SplititApiUsername)) ?? this._configuration["SplititApiUsername"];
            }
        }

        public string SplititApiPassword
        {
            get
            {
                return GetCookieValue(nameof(SetCredentialsModel.SplititApiPassword)) ?? this._configuration["SplititApiPassword"];
            }
        }

        public string FlexFieldsUrlRoot
        {
            get
            {
                return UseProductionUrl ? this._configuration["FlexFieldsUrlRootProduction"] : this._configuration["FlexFieldsUrlRoot"];
            }
        }

        public string UpstreamUrlRoot
        {
            get
            {
                return UseProductionUrl ? this._configuration["UpstreamUrlRootProduction"] : this._configuration["UpstreamUrlRoot"];
            }
        }
    }
}
