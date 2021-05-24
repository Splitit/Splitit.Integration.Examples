using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Splitit.Integration.Example.Mvc22.Models
{
    public class PatrickViewModel
    {
        public string ApiKey { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string RequestedNumInstallments { get; set; }
    }
}
