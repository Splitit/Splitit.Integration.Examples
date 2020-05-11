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
using Splitit.Integration.Example.Mvc22.Models;
using Splitit.SDK.Client.Api;
using Splitit.SDK.Client.Client;
using Splitit.SDK.Client.Model;

namespace Splitit.Integration.Example.Mvc21.Controllers
{
    public class ScenarioController : Controller
    {
        private IConfiguration _configuration;

        public ScenarioController(IConfiguration configuration)
        {
            this._configuration = configuration;

            Configuration.Sandbox.AddApiKey(this._configuration["SplititApiKey"]);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Basic(int options = 5, decimal amount = 500)
        {
            return View(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(Configuration.Sandbox, this._configuration["SplititApiUsername"], this._configuration["SplititApiPassword"])
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, "USD")
            });
        }

        public IActionResult AutoCapture(int options = 5, decimal amount = 500)
        {
            return View(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(Configuration.Sandbox, this._configuration["SplititApiUsername"], this._configuration["SplititApiPassword"])
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .AddCaptureSettings(autoCapture: true)
                    .GetPublicToken(amount, "USD")
            });
        }

        public IActionResult DeferredCapture(int options = 5, decimal amount = 500, decimal firstInstallment = 100, int delayDays = 10)
        {
            return View(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(Configuration.Sandbox, this._configuration["SplititApiUsername"], this._configuration["SplititApiPassword"])
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .AddCaptureSettings(firstInstallmentAmount: firstInstallment, currencyCode: "USD", firstChargeDate: DateTime.Now.AddDays(delayDays))
                    .GetPublicToken(amount, "USD")
            });
        }

        public IActionResult Secure3D(int options = 5, decimal amount = 500)
        {
            return View(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(Configuration.Sandbox, this._configuration["SplititApiUsername"], this._configuration["SplititApiPassword"])
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .Add3DSecure(new RedirectUrls(){
                        Canceled = $"https://{this.Request.Host.Host}" + Url.Action("Secure3DResponse", new { result = "canceled"}),
                        Failed = $"https://{this.Request.Host.Host}" + Url.Action("Secure3DResponse", new { result = "failed"}),
                        Succeeded = $"https://{this.Request.Host.Host}" + Url.Action("Secure3DResponse", new { result = "succeeded"}),
                    })
                    .GetPublicToken(amount, "USD")
            });
        }

        public IActionResult Secure3DResponse(string result)
        {
            return Content(result);
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
                PublicToken = FlexFields.Authenticate(Configuration.Sandbox, this._configuration["SplititApiUsername"], this._configuration["SplititApiPassword"])
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, "USD")
            });
        }
    }
}
