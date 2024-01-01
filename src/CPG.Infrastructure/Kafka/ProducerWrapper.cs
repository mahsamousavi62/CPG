using System;
using Confluent.Kafka;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Kafka;

public class ProducerWrapper
{
    private readonly string _topicName;
    private readonly IProducer<string, string> _producer;
    private readonly ProducerConfig _config;

    public ProducerWrapper(ProducerConfig config,string topicName)
    {
        _topicName = topicName;
        _config = config;
        _producer = new ProducerBuilder<string, string>(_config).Build();
    }

    public async Task WriteMessage(string key, string message)
    {
        var dr = await _producer.ProduceAsync(_topicName, new Message<string, string>()
                    {
                        Key = key,
                        Value = message
                    });

        Console.WriteLine($"KAFKA => Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");

        return;
    }
}
