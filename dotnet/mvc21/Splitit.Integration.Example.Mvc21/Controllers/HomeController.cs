using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Splitit.Integration.Example.Mvc21.Models;
//using Splitit.SDK.Client.Api;
//using Splitit.SDK.Client.Client;
//using Splitit.SDK.Client.Model;

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
            return View();
        }

        public IActionResult Example(decimal? amount = 500)
        {
            ViewBag.Amount = amount;
            ViewBag.UpstreamMerchantId = this._configuration["SplititMerchantId"];
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
            //LoginResponse loginResult = null;

            try
            {
                var logger = new LoggingHandler(new HttpClientHandler());

                using (var client = new HttpClient(logger))
                {
                    client.BaseAddress = new Uri("https://webapi.sandbox.splitit.com/");

                    var json = JsonConvert.SerializeObject(new
                    {
                        Username = this._configuration["SplititApiUsername"],
                        Password = this._configuration["SplititApiPassword"]
                    });

                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("/api/Login", data);

                    //var manualResponse = await result.Content.ReadAsAsync<LoginResponse>();
                    return Json(new { logger.Log });
                }


                //var billingAddress = new AddressData();
                //var consumerModel = new ConsumerData(isLocked: false, isDataRestricted: false);

                //await this.TryUpdateModelAsync(billingAddress, "billingAddress");
                //await this.TryUpdateModelAsync(consumerModel, "consumerModel");

                //Configuration.Sandbox.AddApiKey(this._configuration["SplititApiKey"]);

                ////var loginApi = new LoginApi(Configuration.Sandbox);
                ////var request = new LoginRequest(userName: this._configuration["SplititApiUsername"], password: this._configuration["SplititApiPassword"]);

                ////loginResult = await loginApi.LoginPostAsync(request);

                

                //var initRequest = new InitiateInstallmentPlanRequest()
                //{
                //    PlanData = new PlanData(
                //        amount: new MoneyWithCurrencyCode(amount, "USD"),
                //        numberOfInstallments: numInstallments,
                //        attempt3DSecure: false),
                //    BillingAddress = billingAddress,
                //    ConsumerData = consumerModel,
                //    PaymentWizardData = new PaymentWizardData(
                //        requestedNumberOfInstallments: "1,2,4,6,8",
                //        isOpenedInIframe: false),
                //    RedirectUrls = new RedirectUrls(
                //        succeeded: "https://www.success.com/",
                //        failed: "https://www.ynet.co.il/",
                //        canceled: "https://www.walla.com/")
                //};

                //using (var client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri("https://webapi.sandbox.splitit.com/");

                //    var json = JsonConvert.SerializeObject(new
                //    {
                //        PlanData = new PlanData(
                //            amount: new MoneyWithCurrencyCode(amount, "USD"),
                //            numberOfInstallments: numInstallments,
                //            attempt3DSecure: false),
                //        BillingAddress = billingAddress,
                //        ConsumerData = consumerModel,
                //        PaymentWizardData = new PaymentWizardData(
                //            requestedNumberOfInstallments: "1,2,4,6,8",
                //            isOpenedInIframe: false),
                //        RedirectUrls = new RedirectUrls(
                //            succeeded: "https://www.success.com/",
                //            failed: "https://www.ynet.co.il/",
                //            canceled: "https://www.walla.com/"),
                //        RequestHeader = new
                //        {
                //            ApiKey = this._configuration["SplititApiKey"],
                //            SessionId = loginResult.SessionId
                //        }
                //    });

                //    var data = new StringContent(json, Encoding.UTF8, "application/json");
                //    var result = await client.PostAsync("/api/InstallmentPlan/Initiate", data);

                //    var manualResponse = await result.Content.ReadAsStringAsync();
                //    return Json(new { loginResult, manual = manualResponse, result.Headers, result.StatusCode });

                //}

                //var installmentPlanApi = new InstallmentPlanApi(Configuration.Sandbox, sessionId: loginResult.SessionId);
                //var initResponse = installmentPlanApi.InstallmentPlanInitiate(new InitiateInstallmentPlanRequest()
                //{
                //    PlanData = new PlanData(
                //            amount: new MoneyWithCurrencyCode(amount, "USD"),
                //            numberOfInstallments: numInstallments,
                //            attempt3DSecure: false),
                //    BillingAddress = billingAddress,
                //    ConsumerData = consumerModel,
                //    PaymentWizardData = new PaymentWizardData(
                //            requestedNumberOfInstallments: "1,2,4,6,8",
                //            isOpenedInIframe: false),
                //    RedirectUrls = new RedirectUrls(
                //            succeeded: "https://www.success.com/",
                //            failed: "https://www.ynet.co.il/",
                //            canceled: "https://www.walla.com/")
                //});

                //return new JsonResult(initResponse, new Newtonsoft.Json.JsonSerializerSettings()
                //{
                //    ContractResolver = new CamelCasePropertyNamesContractResolver()
                //});
            }
            catch(Exception ex)
            {
                return Json(new { exception = ex, loginData = "" });
            }
            
        }
    }

    public class LoggingHandler : DelegatingHandler
    {
        public List<string> Log { get; set; }

        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            Log = new List<string>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Log.Add("Request:");
            Log.Add(request.ToString());
            if (request.Content != null)
            {
                Log.Add(await request.Content.ReadAsStringAsync());
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            Log.Add("Response:");
            Log.Add(response.ToString());
            if (response.Content != null)
            {
                Log.Add(await response.Content.ReadAsStringAsync());
            }

            return response;
        }
    }
}
