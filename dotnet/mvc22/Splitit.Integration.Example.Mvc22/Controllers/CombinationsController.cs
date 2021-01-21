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
    public class CombinationsController : ExampleControllerBase
    {
        public CombinationsController(CredentialSource configuration) : base (configuration)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ProcessCombination(
            bool? attempt3DSecure = null,
            string pageFlow = "single-page",
            int options = 5, 
            decimal amount = 500, 
            string currency = "USD", 
            string culture = "en-US")
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
                    attempt3DSecure: attempt3DSecure,
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
                    CultureName = culture,
                    Email = "john+" + DateTime.Now.Millisecond + "@gmail.com" // since john grabbed the @gmail, let him get some spam now and then :D
                },
                PaymentWizardData = new PaymentWizardData()
                {
                    RequestedNumberOfInstallments = string.Join(",", Enumerable.Range(1, options)),
                    Is3dSecureInPopup = true
                }
            });

            if (pageFlow == "single-page")
            {
                return View("SinglePage", new CommonTestModel()
                {
                    PublicToken = initResponse.PublicToken,
                    PlanNumber = initResponse.InstallmentPlan.InstallmentPlanNumber
                });
            }
            else if(pageFlow == "multi-page")
            {
                return View("MultiPage", new CommonTestModel()
                {
                    PublicToken = initResponse.PublicToken,
                    PlanNumber = initResponse.InstallmentPlan.InstallmentPlanNumber,
                    Culture = culture
                });
            }

            return RedirectToAction("Index", "Scenario");
        }

        [HttpPost]
        public async Task<IActionResult> AjaxCreate(string publicToken, string planNumber)
        {
            var installmentPlanApi = new InstallmentPlanApi(this.FlexFieldsEnv, sessionId: publicToken);

            try
            {
                var createResponse = await installmentPlanApi.InstallmentPlanCreateAsync(new CreateInstallmentPlanRequest()
                {
                    InstallmentPlanNumber = planNumber,
                    CreditCardDetails = new CardData()
                    {
                        CardHolderFullName = "3DS_V2_CHALLENGE_VALID_ERROR",
                        CardNumber = "4111111111111111",
                        CardExpMonth = "02",
                        CardExpYear = "22",
                        CardCvv = "222"
                    },
                    PlanApprovalEvidence = new PlanApprovalEvidence(areTermsAndConditionsApproved: true)
                });

                return Json(new { success = true, require3d = false });
            }
            catch (SplititApiException ex)
            {
                if (ex.Code == "641")
                {
                    return Json(new { success = true, require3d = true });
                }
                else
                {
                    return Json(new { success = false, errorCode = ex.Code, additionalInfo = ex.AdditionalInfo });
                }
            }
            catch(Exception ex)
            {
                return Json(new { success = false, additionalInfo = ex.Message });
            }
        }

        public IActionResult NextPage(string publicToken, string planNumber)
        {
            return View("MultiPageComplete", new CommonTestModel()
            {
                PlanNumber = planNumber,
                PublicToken = publicToken
            });
        }
    }
}
