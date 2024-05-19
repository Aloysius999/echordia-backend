using Ech.Schema.Executive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ech.Schema.IntraService.ItemSaleMonitor
{
    public class ItemSaleControl : SaleControl
    {
        public new int id { get; set; }

        public int originalId
        {
            get { return base.id; }
            set { base.id = value; }
        }
    }
}
