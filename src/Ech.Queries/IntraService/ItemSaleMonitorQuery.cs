using Ech.Schema.Executive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ech.Queries.IntraService
{
    public class ItemSaleMonitorQuery : QueryBase
    {
        public ItemSaleMonitorQuery(SaleControl saleControl, QueryBase.QueryType query)
            :base(query)
        {
            this.SaleControl = saleControl;
        }

        public SaleControl SaleControl { get; set; }
    }
}
