using Ech.ItemSaleMonitor.Database;
using Ech.ItemSaleMonitor.Database.Mapping;
using Ech.Queries.IntraService;
using Ech.Schema.IntraService.ItemSaleMonitor;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Ech.ItemSaleMonitor.Messaging
{
    public class CustomMessageHandler : IMessageHandler
    {
        readonly ILogger<CustomMessageHandler> _logger;
        readonly IServiceProvider _serviceProvider;

        public CustomMessageHandler(ILogger<CustomMessageHandler> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
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

            ItemSaleMonitorQuery query = JsonConvert.DeserializeObject<ItemSaleMonitorQuery>(payload);

            switch(query.Query)
            {
                case QueryBase.QueryType.New:
                    HandleNewQuery(query);
                    break;
                case QueryBase.QueryType.Update:
                    HandleUpdateQuery(query);
                    break;
                case QueryBase.QueryType.Delete:
                    HandleDeleteQuery(query);
                    break;
            }
        }

        private void HandleNewQuery(ItemSaleMonitorQuery query)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

                    ItemSaleControl itemFound = db.ItemSaleControls.FirstOrDefault<ItemSaleControl>(item => item.itemId == query.SaleControl.itemId &&
                                                                                                                item.userId == query.SaleControl.userId);

                    if (itemFound == null)
                    {
                        // not found - new entity
                        ItemSaleControl item = Mapping.Map(query.SaleControl);

                        db.ItemSaleControls.Add(item);

                        db.SaveChanges();
                    }
                    else
                    {
                        _logger.LogDebug(string.Format("HandleNewQuery - Item already exists - userId={0} itemId={1}", query.SaleControl.userId, query.SaleControl.itemId));
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private void HandleUpdateQuery(ItemSaleMonitorQuery query)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

                    ItemSaleControl itemFound = db.ItemSaleControls.FirstOrDefault<ItemSaleControl>(item => item.itemId == query.SaleControl.itemId &&
                                                                                                                item.userId == query.SaleControl.userId);

                    if (itemFound != null)
                    {
                        // found - update entity
                        ItemSaleControl item = Mapping.Map(query.SaleControl);

                        item.saleControlId = itemFound.saleControlId;

                        db.ItemSaleControls.Update(item);

                        db.SaveChanges();
                    }
                    else
                    {
                        _logger.LogDebug(string.Format("HandleUpdateQuery - Item not found - userId={0} itemId={1}", query.SaleControl.userId, query.SaleControl.itemId));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private void HandleDeleteQuery(ItemSaleMonitorQuery query)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

                    ItemSaleControl itemFound = db.ItemSaleControls.FirstOrDefault<ItemSaleControl>(item => item.itemId == query.SaleControl.itemId &&
                                                                                                                item.userId == query.SaleControl.userId);

                    if (itemFound != null)
                    {
                        // found - delete entity
                        db.ItemSaleControls.Remove(itemFound);

                        db.SaveChanges();
                    }
                    else
                    {
                        _logger.LogDebug(string.Format("HandleDeleteQuery - Item not found - userId={0} itemId={1}", query.SaleControl.userId, query.SaleControl.itemId));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
