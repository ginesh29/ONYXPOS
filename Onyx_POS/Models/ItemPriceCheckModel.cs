namespace Onyx_POS.Models
{
    public class ItemPriceCheckModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Qty { get; set; }
        public decimal Price { get; set; }
        public decimal? PackQty { get; set; }
        public string PluCode { get; set; }
    }
}
