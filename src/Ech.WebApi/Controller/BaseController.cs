using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using Ech.WebApi.API;
using Ech.Abstractions.Database;

namespace Ech.WebApi.Controller
{
    public class BaseController<TController> : ControllerBase
    {
        public BaseController(IConfiguration config, ILogger<TController> logger)
        {
            Config = config;
            Logger = logger;
        }

        protected IConfiguration Config { get; private set; }

        protected readonly ILogger<TController> Logger;
        protected bool RequiresAuth { get; set; } = false;

        [NonAction]
        public virtual ObjectResult StatusCode(HttpStatusCode statusCode, object value)
        {
            var model = new ApiResponseModel()
            {
                Value = value,
            };

            return base.StatusCode((int)statusCode, model);
        }

        //protected int DefaultPageSize
        //{
        //    get
        //    {
        //        string str = this.Config.GetValue<string>("Search:PageSize");
        //        int val = int.Parse(str);
        //        return val;
        //    }
        //}
    }
}
