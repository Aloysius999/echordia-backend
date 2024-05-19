using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ech.Queries.IntraService
{
    public class QueryBase
    {
        public enum QueryType
        {
            New,
            Update,
            Delete,
        }

        protected QueryBase(QueryType query)
        {
            this.Query = query;
        }

        public QueryType Query { get; set; }
    }
}
