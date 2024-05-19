using Ech.Schema.Executive;
using Ech.Schema.IntraService.ItemSaleMonitor;
using Newtonsoft.Json;

namespace Ech.ItemSaleMonitor.Database.Mapping
{
    public class Mapping
    {
        internal static ItemSaleControl Map(SaleControl saleControl)
        {
            string json = JsonConvert.SerializeObject(saleControl);
            ItemSaleControl res = JsonConvert.DeserializeObject<ItemSaleControl>(json);
            return res;
        }

        internal static SaleControl Map(ItemSaleControl itemSaleControl)
        {
            string json = JsonConvert.SerializeObject(itemSaleControl);
            SaleControl res = JsonConvert.DeserializeObject<SaleControl>(json);
            return res;
        }
    }
}
