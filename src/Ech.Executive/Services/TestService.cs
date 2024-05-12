using Ech.Executive.Controllers;
using Ech.Executive.Database;

namespace Ech.Executive.Services
{
    public class TestService : ITestService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TestService> _logger;
        private readonly MySQLDbContext _db;

        public TestService(IConfiguration config, ILogger<TestService> logger, MySQLDbContext db)
        {
            _config = config;
            _logger = logger;
            _db = db;
        }
    }
}
