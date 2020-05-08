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

        public IActionResult Index(decimal? amount = 500)
        {
            ViewBag.Amount = amount;
            ViewBag.UpstreamMerchantId = this._configuration["SplititApiKey"];
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
            try
            {
                var billingAddress = new AddressData();
                var consumerModel = new ConsumerData(isLocked: false, isDataRestricted: false);

                await this.TryUpdateModelAsync(billingAddress, "billingAddress");
                await this.TryUpdateModelAsync(consumerModel, "consumerModel");

                Configuration.Sandbox.AddApiKey(this._configuration["SplititApiKey"]);
                // The next 2 lines are not needed unless deploying to Splitit environment
                Configuration.Sandbox.BasePath = this._configuration["SplititApiUrl"];
                Response.Headers.Add("splitit-api-url", Configuration.Sandbox.BasePath);

                var loginApi = new LoginApi(Configuration.Sandbox);
                var request = new LoginRequest(
                    userName: this._configuration["SplititApiUsername"], 
                    password: this._configuration["SplititApiPassword"]);

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

                return new JsonResult(initResponse);
            }
            catch(Exception ex)
            {
                var result = Json(ex);
                result.StatusCode = StatusCodes.Status400BadRequest;
                return result;
            }
            
        }
    
        public IActionResult Debug(int? options = null, int amount = 1000)
        {
            var model = new DebugViewModel();
            
            options = options ?? 5;
            model.InstallmentOptions = "[";
            for(int i = 0; i < options.Value; i++){
                model.InstallmentOptions += (i+1) + ",";
            }
            model.InstallmentOptions = model.InstallmentOptions.TrimEnd(',') + "]";

            Configuration.Sandbox.AddApiKey(this._configuration["SplititApiKey"]);

            model.PublicToken = FlexFields.Authenticate(Configuration.Sandbox, this._configuration["SplititApiUsername"], this._configuration["SplititApiPassword"])
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
