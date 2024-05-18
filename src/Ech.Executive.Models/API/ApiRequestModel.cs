using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ech.Executive.Models.API
{
    public class ApiRequestModel
    {
        public QueryType queryType { get; set; }
        public int id { get; set; }
        public int userId { get; set; }
    }
}
