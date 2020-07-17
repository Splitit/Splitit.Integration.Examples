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
    public class ScenarioController : ExampleControllerBase
    {
        public ScenarioController(CredentialSource configuration) : base (configuration)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Basic(int options = 5, decimal amount = 500)
        {
            return View(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, "USD")
            });
        }

        public IActionResult Sentry(int options = 5, decimal amount = 500)
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, "USD")
            });
        }

        public IActionResult CardholderName(int options = 5, decimal amount = 500)
        {
            return View(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(Configuration.Sandbox, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, "USD")
            });
        }

        public IActionResult AutoCapture(int options = 5, decimal amount = 500)
        {
            return View(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .AddCaptureSettings(autoCapture: true)
                    .GetPublicToken(amount, "USD")
            });
        }

        public IActionResult DeferredCapture(int options = 5, decimal amount = 500, decimal firstInstallment = 100, int delayDays = 10)
        {
            return View(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .AddCaptureSettings(firstInstallmentAmount: firstInstallment, currencyCode: "USD", firstChargeDate: DateTime.Now.AddDays(delayDays))
                    .GetPublicToken(amount, "USD")
            });
        }

        public IActionResult Secure3D(int options = 5, decimal amount = 500)
        {
            return View(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .Add3DSecure(new RedirectUrls())
                    .GetPublicToken(amount, "USD")
            });
        }

        [HttpGet]
        public IActionResult AjaxPublicToken(int options = 5, decimal amount = 500)
        {
            ViewBag.Options = options;
            ViewBag.Amount = amount;
            return View(new CommonTestModel() {  });
        }

        [HttpPost]
        [ActionName("AjaxPublicToken")]
        public IActionResult AjaxPublicTokenPost(int options = 5, decimal amount = 500)
        {
            return Json(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, "USD")
            });
        }

        [HttpGet]
        public IActionResult EmbeddedPaymentForm(int options = 5, decimal amount = 500, bool secure3d = false)
        {
            var ff = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                .AddInstallments(Enumerable.Range(1, options).ToList())
                .AddBillingInformation(addressData: new AddressData()
                {
                    AddressLine = "J. Street 23",
                    City = "Birmingham",
                    Country = "GB",
                    Zip = "48993"
                }, consumerData: new ConsumerData()
                {
                    Email = "john+" + DateTime.Now.Millisecond + "@gmail.com" // since john grabbed the @gmail, let him get some spam now and then :D
                });

            if (secure3d)
			{
                ff.Add3DSecure(null);
			}

            return View(new CommonTestModel(){
                PublicToken = ff.GetPublicToken(amount, "USD")
            });
        }
    }
}
