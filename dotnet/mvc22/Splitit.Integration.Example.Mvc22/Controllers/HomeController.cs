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
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IActionResult Index(decimal amount = 500, int options = 5)
        {
            Configuration.Sandbox.AddApiKey(this._configuration["SplititApiKey"]);

            ViewBag.UpstreamMerchantId = this._configuration["SplititApiKey"];
            ViewBag.PublicToken = FlexFields
                .Authenticate(Configuration.Sandbox, this._configuration["SplititApiUsername"], this._configuration["SplititApiPassword"])
                .AddInstallments(Enumerable.Range(1, options).ToList())
                .GetPublicToken(amount, "USD");

            return View();
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

            Configuration.Sandbox.AddApiKey(this._configuration["SplititApiKey"]);

            model.PublicToken = FlexFields
                .Authenticate(Configuration.Sandbox, this._configuration["SplititApiUsername"], this._configuration["SplititApiPassword"])
                .AddInstallments(Enumerable.Range(1, options).ToList())
                .GetPublicToken(amount, "USD");

            return View(model);

            //var billingAddress = new AddressData();
            //var consumerModel = new ConsumerData(isLocked: false, isDataRestricted: false);

            //await this.TryUpdateModelAsync(billingAddress, "billingAddress");
            //await this.TryUpdateModelAsync(consumerModel, "consumerModel");

            // Configuration.Sandbox.AddApiKey(this._configuration["SplititApiKey"]);

            // var publicToken = FlexFields.Authenticate(Configuration.Sandbox, this._configuration["SplititApiUsername"], this._configuration["SplititApiPassword"])
            //     .GetPublicToken(amount, "USD");

            // return new JsonResult(new { publicToken });

            // var loginApi = new LoginApi(Configuration.Sandbox);
            // var request = new LoginRequest(userName: this._configuration["SplititApiUsername"], password: this._configuration["SplititApiPassword"]);

            // var loginResult = await loginApi.LoginPostAsync(request);

            // var installmentPlanApi = new InstallmentPlanApi(Configuration.Sandbox, sessionId: loginResult.SessionId);
            // var initResponse = installmentPlanApi.InstallmentPlanInitiate(new InitiateInstallmentPlanRequest()
            // {
            //     PlanData = new PlanData(
            //         amount: new MoneyWithCurrencyCode(amount, "USD"), 
            //         numberOfInstallments: numInstallments, 
            //         attempt3DSecure: false,
            //         strategy: null),//PlanStrategy.NonSecuredPlan),
            //     PaymentWizardData = new PaymentWizardData(
            //         isOpenedInIframe: false,
            //         requestedNumberOfInstallments: string.Join(",", Enumerable.Range(1, numOptions))),
            //     //BillingAddress = billingAddress,
            //     //ConsumerData = consumerModel,
            //     RedirectUrls = new RedirectUrls(
            //         succeeded: "https://www.success.com/", 
            //         failed: "https://www.ynet.co.il/", 
            //         canceled: "https://www.walla.com/")
            // });

            // return new JsonResult(initResponse, new System.Text.Json.JsonSerializerOptions()
            // {
            //     ContractResolver = new DefaultContractResolver()
            // });

            //return new JsonResult(initResponse);
        }
    }
}
