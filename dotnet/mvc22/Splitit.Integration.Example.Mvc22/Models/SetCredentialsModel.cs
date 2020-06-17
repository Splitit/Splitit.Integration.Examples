using System;
using Microsoft.Extensions.Configuration;
using Splitit.Integration.Example.Mvc21;

namespace Splitit.Integration.Example.Mvc22.Models
{
    public class SetCredentialsModel
    {
        public string SplititApiKey{get;set;}

        public string SplititApiUsername{get;set;}

        public string SplititApiPassword{get;set;}

        public string Environment {get;set;}

        public static SetCredentialsModel FromConfiguration(IConfiguration configuration)
        {
            return new SetCredentialsModel()
            {
                 Environment = configuration["SplititApiUrl"].Contains("production") ? "Production" : "Sandbox",
                 SplititApiKey = configuration["SplititApiKey"],
                 SplititApiUsername = configuration["SplititApiUsername"],
                 SplititApiPassword = configuration["SplititApiPassword"]
            };
        }

        public static SetCredentialsModel FromCurrent(CredentialSource configuration)
        {
            return new SetCredentialsModel()
            {
                 Environment = configuration.SplititApiUrl.Contains("production") ? "Production" : "Sandbox",
                 SplititApiKey = configuration.SplititApiKey,
                 SplititApiUsername = configuration.SplititApiUsername,
                 SplititApiPassword = configuration.SplititApiPassword,
            };
        }
    }
}