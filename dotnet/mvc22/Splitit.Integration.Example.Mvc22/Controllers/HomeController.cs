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
    public class HomeController : ExampleControllerBase
    {
        public HomeController(CredentialSource configuration) : base(configuration)
        {
        }

        public IActionResult Index(decimal amount = 500, int options = 5)
        {
            return RedirectToAction("Index", "ScenarioV2");
        }

        public IActionResult SetCredentials()
        {
            return View(SetCredentialsModel.FromCurrent(this.CredentialSource));
        }

        [HttpPost]
        public IActionResult SetCredentials(SetCredentialsModel model)
        {
            this.Response.Cookies.Append(nameof(model.Environment), model.Environment);
            this.Response.Cookies.Append(nameof(model.SplititApiUrl), model.SplititApiUrl);
            this.Response.Cookies.Append(nameof(model.SplititApiKey), model.SplititApiKey);
            this.Response.Cookies.Append(nameof(model.SplititApiPassword), model.SplititApiPassword);
            this.Response.Cookies.Append(nameof(model.SplititApiUsername), model.SplititApiUsername);
            this.Response.Cookies.Append(nameof(model.FlexFieldsUrlRoot), model.FlexFieldsUrlRoot);
            this.Response.Cookies.Append(nameof(model.FlexFieldsV2UrlRoot), model.FlexFieldsV2UrlRoot);
            this.Response.Cookies.Append(nameof(model.PaymentFormEmbedderUrlRoot), model.PaymentFormEmbedderUrlRoot);
            this.Response.Cookies.Append(nameof(model.UpstreamUrlRoot), model.UpstreamUrlRoot);

            this.TempData["show-success"] = true;

            return View(model);
        }

        [HttpPost]
        public IActionResult ResetCredentials()
        {
            this.Response.Cookies.Delete(nameof(SetCredentialsModel.Environment));
            this.Response.Cookies.Delete(nameof(SetCredentialsModel.SplititApiKey));
            this.Response.Cookies.Delete(nameof(SetCredentialsModel.SplititApiUrl));
            this.Response.Cookies.Delete(nameof(SetCredentialsModel.SplititApiPassword));
            this.Response.Cookies.Delete(nameof(SetCredentialsModel.SplititApiUsername));
            this.Response.Cookies.Delete(nameof(SetCredentialsModel.PaymentFormEmbedderUrlRoot));
            this.Response.Cookies.Delete(nameof(SetCredentialsModel.FlexFieldsUrlRoot));
            this.Response.Cookies.Delete(nameof(SetCredentialsModel.FlexFieldsV2UrlRoot));
            this.Response.Cookies.Delete(nameof(SetCredentialsModel.UpstreamUrlRoot));

            this.TempData["show-success"] = true;

            return RedirectToAction("SetCredentials");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    
        public IActionResult Debug(int options = 5, int amount = 1000)
        {
            var model = new DebugViewModel();

            model.InstallmentOptions = $"[{string.Join(",", Enumerable.Range(1, options))}]";

            model.PublicToken = FlexFields
                .Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                .AddInstallments(Enumerable.Range(1, options).ToList())
                .GetPublicToken(amount, "USD");

            return View(model);
        }
    }
}
