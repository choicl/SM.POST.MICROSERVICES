using Amazon.Runtime.Internal.Util;
using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Post.Query.Infrastructure.Consumers
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public ConsumerHostedService(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.InfoFormat("Event consumer service running");

            using(IServiceScope scope = _serviceProvider.CreateScope())
            {
                var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

                Task.Run(() => eventConsumer.Consume(topic), cancellationToken);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.InfoFormat("Event consumer service stopped");

            return Task.CompletedTask;
        }
    }
}
