using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ech.Executive.Models.Schema
{
    public class QueryModel
    {
        QueryType queryType { get; set; }
        int id { get; set; }
        int userId { get; set; }
    }
}
