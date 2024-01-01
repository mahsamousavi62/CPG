using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Providers.Charispay
{
    public class CharisPayConfig
    {
        public string BaseUrl { get; set; }
        public string InqueryIbanUrl { get; set; }
        public string Token { get; set; }
    }
}
