using Ech.Schema.Executive;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ech.Schema.IntraService.ItemSaleMonitor
{
    [Table("seller")]
    public class Seller : User
    {
        [Key] 
        public int sellerId { get; set; }
    }
}
