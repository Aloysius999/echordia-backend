using Ech.Schema.Executive;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ech.Schema.IntraService.ItemSaleMonitor
{
    [Table("itemSaleControl")]
    public class ItemSaleControl : SaleControl
    {
        [Key]
        public int saleControlId { get; set; }
    }
}
