using Ech.ItemSaleMonitor.Controllers;
using Ech.ItemSaleMonitor.Database;

namespace Ech.ItemSaleMonitor.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MessagingService> _logger;
        private readonly MySQLDbContext _db;

        public MessagingService(IConfiguration config, ILogger<MessagingService> logger, MySQLDbContext db)
        {
            _config = config;
            _logger = logger;
            _db = db;
        }
    }
}
