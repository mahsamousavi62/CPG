using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.Redis
{
    public class RedisConfig
    {
        public string Server { get; set; }
    
        public string Port { get; set; }

        public string Password { get; set; }

    }
}
