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

        public IActionResult Basic(int options = 5, decimal amount = 500, string currency = "USD", string culture = "en-US")
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, currency),
                Culture = culture
            });
        }

        public IActionResult ClientConfig(int options = 5, decimal amount = 500, string currency = "USD")
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, currency)
            });
        }

        public IActionResult CustomErrors(int options = 5, decimal amount = 500, string currency = "USD", string culture = "en-US")
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, currency),
                Culture = culture
            });
        }

        public IActionResult CustomEvents(int options = 5, decimal amount = 500, string currency = "USD", string culture = "en-US")
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, currency),
                Culture = culture
            });
        }

        public IActionResult PreselectedNumInstallments(int options = 12, decimal amount = 500, int preselectedOption = 10, string currency = "USD")
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList(), defaultNumInstallments: preselectedOption)
                    .GetPublicToken(amount, currency)
            });
        }

        public IActionResult Sentry(int options = 5, decimal amount = 500, string currency = "USD")
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, currency)
            });
        }

        public IActionResult CardholderName(int options = 5, decimal amount = 500, string currency = "USD")
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(Configuration.Sandbox, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, currency)
            });
        }

        public IActionResult AutoCapture(int options = 5, decimal amount = 500, string currency = "USD")
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .AddCaptureSettings(autoCapture: true)
                    .GetPublicToken(amount, currency)
            });
        }

        public IActionResult DeferredCapture(int options = 5, decimal amount = 500, decimal firstInstallment = 100, int delayDays = 10, string currency = "USD")
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .AddCaptureSettings(firstInstallmentAmount: firstInstallment, currencyCode: currency, firstChargeDate: DateTime.Now.AddDays(delayDays))
                    .GetPublicToken(amount, currency)
            });
        }

        public IActionResult Secure3D(int options = 5, decimal amount = 500, string currency = "USD")
        {
            return View(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .Add3DSecure(new RedirectUrls())
                    .GetPublicToken(amount, currency)
            });
        }

        [HttpGet]
        public IActionResult AjaxPublicToken(int options = 5, decimal amount = 500, string currency = "USD")
        {
            ViewBag.Options = options;
            ViewBag.Amount = amount;
            return View(new CommonTestModel() {  });
        }

        [HttpPost]
        [ActionName("AjaxPublicToken")]
        public IActionResult AjaxPublicTokenPost(int options = 5, decimal amount = 500, string currency = "USD")
        {
            return Json(new CommonTestModel()
            {
                PublicToken = FlexFields.Authenticate(this.FlexFieldsEnv, SplititApiUsername, SplititApiPassword)
                    .AddInstallments(Enumerable.Range(1, options).ToList())
                    .GetPublicToken(amount, currency)
            });
        }

        [HttpGet]
        public async Task<IActionResult> EmbeddedPaymentForm(int options = 5, decimal amount = 500, bool secure3d = false, string currency = "USD")
        {
            var loginApi = new LoginApi(this.FlexFieldsEnv);
            var request = new LoginRequest(userName: SplititApiUsername, password: SplititApiPassword);

            var loginResult = await loginApi.LoginPostAsync(request);

            var installmentPlanApi = new InstallmentPlanApi(this.FlexFieldsEnv, sessionId: loginResult.SessionId);
            var initResponse = installmentPlanApi.InstallmentPlanInitiate(new InitiateInstallmentPlanRequest()
            {
                PlanData = new PlanData(
                    amount: new MoneyWithCurrencyCode(amount, currency),
                    numberOfInstallments: options / 2,
                    attempt3DSecure: secure3d,
                    autoCapture: true),
                BillingAddress = new AddressData()
				{
					AddressLine = "J. Street 23",
					City = "Birmingham",
					Country = "GB",
					Zip = "48993"
				},
				ConsumerData = new ConsumerData()
				{
					Email = "john+" + DateTime.Now.Millisecond + "@gmail.com" // since john grabbed the @gmail, let him get some spam now and then :D
				},
				PaymentWizardData = new PaymentWizardData()
				{
                    RequestedNumberOfInstallments = string.Join(",", Enumerable.Range(1, options)),
                    IsOpenedInIframe = true
                }
            });

			return View(new CommonTestModel()
			{
				PublicToken = initResponse.PublicToken,
                CheckoutUrl = initResponse.CheckoutUrl
            });
		}
    }
}
