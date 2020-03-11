using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;
using Splitit.Integration.Example.Mvc21.Models;
using Splitit.SDK.Client.Api;
using Splitit.SDK.Client.Client;
using Splitit.SDK.Client.Model;

namespace Splitit.Integration.Example.Mvc21.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.UpstreamMerchantId = this._configuration["SplititMerchantId"];
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<JsonResult> FlexFieldsExample(decimal amount, int? numInstallments = null)
        {
            var billingAddress = new AddressData();
            var consumerModel = new ConsumerData(isLocked: false, isDataRestricted: false);

            await this.TryUpdateModelAsync(billingAddress, "billingAddress");
            await this.TryUpdateModelAsync(consumerModel, "consumerModel");

            Configuration.Sandbox.AddApiKey(this._configuration["SplititApiKey"]);

            var loginApi = new LoginApi(Configuration.Sandbox);
            var request = new LoginRequest(userName: this._configuration["SplititApiUsername"], password: this._configuration["SplititApiPassword"]);

            var loginResult = await loginApi.LoginPostAsync(request);

            var installmentPlanApi = new InstallmentPlanApi(Configuration.Sandbox, sessionId: loginResult.SessionId);
            var initResponse = installmentPlanApi.InstallmentPlanInitiate(new InitiateInstallmentPlanRequest()
            {
                PlanData = new PlanData(
                    amount: new MoneyWithCurrencyCode(amount, "USD"),
                    numberOfInstallments: numInstallments,
                    attempt3DSecure: false),
                BillingAddress = billingAddress,
                ConsumerData = consumerModel,
                PaymentWizardData = new PaymentWizardData(
                    requestedNumberOfInstallments: "1,2,4,6,8",
                    isOpenedInIframe: false),
                RedirectUrls = new RedirectUrls(
                    succeeded: "https://www.success.com/",
                    failed: "https://www.ynet.co.il/",
                    canceled: "https://www.walla.com/")
            });

            return new JsonResult(initResponse, new Newtonsoft.Json.JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}
