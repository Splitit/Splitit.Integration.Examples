using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;
using Splitit.Integration.Example.Mvc21.Models;
using Splitit.Integration.Example.Mvc22.Controllers;
using Splitit.Integration.Example.Mvc22.Models;
using Splitit.SDK.Client.Api;
using Splitit.SDK.Client.Client;
using Splitit.SDK.Client.Model;

namespace Splitit.Integration.Example.Mvc21.Controllers
{
    public class PatrickController : ExampleControllerBase
    {
        public PatrickController(CredentialSource configuration) : base (configuration)
        {
        }

        public IActionResult Basic(int options = 5, decimal amount = 500, string currency = "USD", string culture = "en-US")
        {
            return View(new PatrickViewModel()
            {
                Amount = amount,
                CurrencyCode = currency,
                RequestedNumInstallments = string.Join(",", Enumerable.Range(1, options)),
                ApiKey = CredentialSource.SplititApiKey
            });
        }
    }
}
