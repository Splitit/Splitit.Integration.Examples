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
                    .Add3DSecure(new RedirectUrls(){
                        Canceled = $"https://{this.Request.Host.Host}" + Url.Action("Secure3DResponse", new { result = "canceled"}),
                        Failed = $"https://{this.Request.Host.Host}" + Url.Action("Secure3DResponse", new { result = "failed"}),
                        Succeeded = $"https://{this.Request.Host.Host}" + Url.Action("Secure3DResponse", new { result = "succeeded"}),
                    })
                    .GetPublicToken(amount, "USD")
            });
        }

        public async Task<IActionResult> Secure3DIframe(int options = 5, decimal amount = 500)
        {
            Configuration.Sandbox.SetTouchPoint(new TouchPoint(code: "PaymentWizard"));

            var loginApi = new LoginApi(this.FlexFieldsEnv);
            var request = new LoginRequest(userName: SplititApiUsername, password: SplititApiPassword);

            var loginResult = await loginApi.LoginPostAsync(request);

            var installmentPlanApi = new InstallmentPlanApi(this.FlexFieldsEnv, sessionId: loginResult.SessionId);
            var initResponse = installmentPlanApi.InstallmentPlanInitiate(new InitiateInstallmentPlanRequest()
            {
                PlanData = new PlanData(
                    amount: new MoneyWithCurrencyCode(amount, "USD"),
                    numberOfInstallments: 3,
                    attempt3DSecure: true,
                    autoCapture: true),
                PaymentWizardData = new PaymentWizardData(
                    requestedNumberOfInstallments: string.Join(",", Enumerable.Range(1, options)),
                    isOpenedInIframe: true)
            });

            //PaymentWizard
            return View(new CommonTestModel()
            {
                PublicToken = initResponse.PublicToken /*FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .Add3DSecure(new RedirectUrls()
                    {
                        Succeeded = $"https://flex-fields.splitit.com/success",
                        Canceled = "https://flex-fields.splitit.com/cancel",
                        Failed = "https://flex-fields.splitit.com/failed"
                    })
                    .GetPublicToken(amount, "USD")*/
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
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, "USD")
            });
        }

        [HttpGet]
        public IActionResult EmbeddedPaymentForm(int options = 5, decimal amount = 500)
        {
            return View(new CommonTestModel(){
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .AddBillingInformation(addressData: new AddressData()
					{
                        AddressLine = "J. Street 23",
                        City = "Birmingham",
                        Country = "GB",
                        Zip = "48993"
					}, consumerData: new ConsumerData() {
                        Email = "john+" + DateTime.Now.Millisecond + "@gmail.com" // since john grabbed the @gmail, let him get some spam now and then :D
                    })
                    .GetPublicToken(amount, "USD")
            });
        }
    }
}
