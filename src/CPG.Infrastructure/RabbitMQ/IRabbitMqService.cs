using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.RabbitMQ
{
    public interface IRabbitMqService
    {
        void PublishMessage(string exchange, string routingKey, string message);

        Task<string> ConsumeMessage(string queue);
    }
}
