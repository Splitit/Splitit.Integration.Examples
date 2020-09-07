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
    public class BackwardCmpController : ExampleControllerBase
    {
        public BackwardCmpController(CredentialSource configuration) 
            : base(configuration)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AjaxInitiate(
            decimal amount, int? numInstallments = null, bool? secure3d = false, bool? autoCapture = null,
            int numOptions = 6, bool pascalCase = false)
        {
            var billingAddress = new AddressData();
            var consumerModel = new ConsumerData(isLocked: false, isDataRestricted: false);

            await this.TryUpdateModelAsync(billingAddress, "billingAddress");
            await this.TryUpdateModelAsync(consumerModel, "consumerModel");

            var loginApi = new LoginApi(this.FlexFieldsEnv);
            var request = new LoginRequest(userName: SplititApiUsername, password: SplititApiPassword);

            var loginResult = await loginApi.LoginPostAsync(request);

            var installmentPlanApi = new InstallmentPlanApi(this.FlexFieldsEnv, sessionId: loginResult.SessionId);
            var initResponse = installmentPlanApi.InstallmentPlanInitiate(new InitiateInstallmentPlanRequest()
            {
                PlanData = new PlanData(
                    amount: new MoneyWithCurrencyCode(amount, "USD"), 
                    numberOfInstallments: numInstallments, 
                    attempt3DSecure: secure3d,
                    autoCapture: autoCapture),
                BillingAddress = billingAddress,
                ConsumerData = consumerModel,
                PaymentWizardData = new PaymentWizardData(
                    requestedNumberOfInstallments: string.Join(",", Enumerable.Range(1, numOptions)))
            });

            return new JsonResult(initResponse);
        }

        public IActionResult Minimal(int options)
        {
            var model = new CommonTestModel();

            model.InstallmentOptions = "[";
            for(int i = 0; i < options; i++){
                model.InstallmentOptions += (i+1) + ",";
            }
            model.InstallmentOptions = model.InstallmentOptions.TrimEnd(',') + "]";

            return View(model);
        }

        public IActionResult SeparateYearMonth() => View();
        public IActionResult Attacker() => View();
        public IActionResult BootstrapThemed() => View();
        public IActionResult CustomInstallmentPicker() => View();
        public IActionResult CustomPaymentBtn() => View();
        public IActionResult DefaultUIStyling() => View();
        public IActionResult Demo() => View();
        public IActionResult NonGrouped() => View();
        public IActionResult PascalCase() => View();
        public IActionResult PaymentScheduleJS() => View();
        public IActionResult Secure3D() => View();
        public IActionResult Security() => View();
        public IActionResult ShowOnDemand() => View();
        public IActionResult SPA() => View();
        public IActionResult VerifyPayment() => View();
        public IActionResult WithUpstream() => View();

        public async Task<IActionResult> OrderComplete(string planNumber){
            var amount = 860; // Usually this comes from DB.

            var loginApi = new LoginApi(this.FlexFieldsEnv);
            var request = new LoginRequest(userName: SplititApiUsername, password: SplititApiPassword);

            var loginResult = await loginApi.LoginPostAsync(request);

            var installmentPlanApi = new InstallmentPlanApi(this.FlexFieldsEnv, sessionId: loginResult.SessionId);
            var verifyPaymentResponse = installmentPlanApi.InstallmentPlanVerifyPayment(new VerifyPaymentRequest(){
                InstallmentPlanNumber = planNumber
            });

            if (verifyPaymentResponse.IsPaid == true && verifyPaymentResponse.OriginalAmountPaid == amount){
                return View(new OrderCompleteModel(){ Ok = true });
            } else {
                return View(new OrderCompleteModel(){ Ok = false });
            }
        }
    }
}
