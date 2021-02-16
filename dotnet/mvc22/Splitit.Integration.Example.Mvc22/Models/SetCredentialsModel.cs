using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Splitit.Integration.Example.Mvc21;

namespace Splitit.Integration.Example.Mvc22.Models
{
    public class SetCredentialsModel
    {
        public string SplititApiKey{get;set;}
        public string SplititApiUsername{get;set;}
        public string SplititApiPassword { get; set; }

        public string SplititApiUrl { get; set; }
        public string PaymentFormEmbedderUrlRoot { get; set; }
        public string FlexFieldsUrlRoot { get; set; }
        public string FlexFieldsV2UrlRoot { get; set; }
        public string UpstreamUrlRoot { get; set; }

        public string Environment {get;set;}

        public static SetCredentialsModel FromConfiguration(HttpContext httpContext, IConfiguration configuration)
        {
            var env = CredentialSource.ParseEnvironment(httpContext);
            var config = configuration.GetSection(env);

            return new SetCredentialsModel()
            {
                Environment = env,
                SplititApiKey = config["SplititApiKey"],
                SplititApiUsername = config["SplititApiUsername"],
                SplititApiPassword = config["SplititApiPassword"],
                FlexFieldsUrlRoot = config["FlexFieldsUrlRoot"],
                FlexFieldsV2UrlRoot = config["FlexFieldsV2UrlRoot"],
                PaymentFormEmbedderUrlRoot = config["PaymentFormEmbedderUrlRoot"],
                SplititApiUrl = config["SplititApiUrl"],
                UpstreamUrlRoot = config["UpstreamUrlRoot"]
            };
        }

        public static SetCredentialsModel FromCurrent(CredentialSource configuration)
        {
            return new SetCredentialsModel()
            {
                Environment = configuration.Environment,
                SplititApiKey = configuration.SplititApiKey,
                SplititApiUsername = configuration.SplititApiUsername,
                SplititApiPassword = configuration.SplititApiPassword,
                FlexFieldsUrlRoot = configuration.FlexFieldsUrlRoot,
                FlexFieldsV2UrlRoot = configuration.FlexFieldsV2UrlRoot,
                UpstreamUrlRoot = configuration.UpstreamUrlRoot,
                SplititApiUrl = configuration.SplititApiUrl,
                PaymentFormEmbedderUrlRoot = configuration.PaymentFormEmbedderUrlRoot
            };
        }
    }
}