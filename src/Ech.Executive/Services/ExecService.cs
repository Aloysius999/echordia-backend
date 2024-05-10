using Ech.Executive.Controllers;
using Ech.Executive.Database;

namespace Ech.Executive.Services
{
    public class ExecService : IExecService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ExecService> _logger;
        private readonly MySQLDbContext _db;

        public ExecService(IConfiguration config, ILogger<ExecService> logger, MySQLDbContext db)
        {
            _config = config;
            _logger = logger;
            _db = db;
        }
    }
}
