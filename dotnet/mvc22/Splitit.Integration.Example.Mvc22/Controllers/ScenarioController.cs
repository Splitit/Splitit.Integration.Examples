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
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Basic(int options, decimal amount)
        {
            var model = LoadCommonModel(options, amount);
            return View(model);
        }

        private CommonTestModel LoadCommonModel(int options, decimal amount)
        {
            var model = new CommonTestModel();

            model.InstallmentOptions = "[";
            for(int i = 0; i < options; i++){
                model.InstallmentOptions += (i+1) + ",";
            }

            model.InstallmentOptions = model.InstallmentOptions.TrimEnd(',') + "]";

            Configuration.Sandbox.AddApiKey(this._configuration["SplititApiKey"]);

            model.PublicToken = FlexFields.Authenticate(Configuration.Sandbox, this._configuration["SplititApiUsername"], this._configuration["SplititApiPassword"])
                .GetPublicToken(amount, "USD");

            return model;
        }
    }
}
