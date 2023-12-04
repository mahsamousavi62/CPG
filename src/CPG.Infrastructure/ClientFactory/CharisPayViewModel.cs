using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.ClientFactory
{
     public class Result
    {
        public string name { get; set; }
        public string family { get; set; }
        public string depositNumber { get; set; }
        public string iban { get; set; }
        public string fullName { get; set; }
        public string bankName { get; set; }
    }

    public class inqueryIbanViewModel
    {
        public List<Result> result { get; set; }
        public object targetUrl { get; set; }
        public bool success { get; set; }
        public object error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
    }

}
