using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ech.Schema.IntraService
{
    public class QueryBase
    {
        public enum QueryType
        {
            New,
            Update,
            Delete,
        }

        public QueryType Query { get; set; }
    }
}
