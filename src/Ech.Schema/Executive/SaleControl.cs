using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ech.Schema.Executive.User;

namespace Ech.Schema.Executive
{
    public class SaleControl
    {
        public enum SaleType
        {
            FixedPrice,
            Auction,
        }

        public enum RunningStatus
        {
            Created,
            Queued,
            Running,
            Completed,
            Stopped,
            Cancelled,
        }

        public enum SellingStatus
        {
            Listed,
            Sold,
            Unsold,
            Withdrawn,
        }

        public int id { get; set; }
        public int userId { get; set; }
        public int itemId { get; set; }

        //public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        public DateTime startAt { get; set; }
        public DateTime endAt { get; set; }
        public DateTime? finishedAt { get; set; }


        public SaleType saleType { get; set; }
        public RunningStatus runningStatus { get; set; }
        public SellingStatus sellingStatus { get; set; }

        // JSON string
        //public string saleData { get; set; }
    }
}
