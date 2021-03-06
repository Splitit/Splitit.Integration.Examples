using System;

namespace Splitit.Integration.Example.Mvc22.Models
{
    public class CommonTestModel
    {
        public string InstallmentOptions { get; set; }
        public string PublicToken {get;set;}
        public string PlanNumber { get; set; }
        public string CheckoutUrl { get; set; }
        public string Culture { get; set; } = "en-US";
        public string Currency { get; set; }
        public string Design { get; internal set; }
        public string Picker { get; internal set; }
    }
}