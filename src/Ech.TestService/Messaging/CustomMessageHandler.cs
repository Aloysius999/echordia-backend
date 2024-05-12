using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ech.TestService.Messaging
{
    public class CustomMessageHandler : IMessageHandler
    {
        readonly ILogger<CustomMessageHandler> _logger;
        public CustomMessageHandler(ILogger<CustomMessageHandler> logger)
        {
            _logger = logger;
        }

        public void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute)
        {
            //Console.WriteLine("CustomMessageHandler - Handle", eventArgs.ToString());
        }

        public void Handle(MessageHandlingContext context, string matchingRoute)
        {
            BasicDeliverEventArgs eventArgs = context.Message;

            string payload = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            Console.WriteLine("CustomMessageHandler - Handle - payload", payload);

            _logger.LogDebug("Handle - payload", payload);
        }
    }
}
