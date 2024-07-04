namespace Onyx_POS.Models
{
    public class PluItemModel
    {
        public string Barcode { get; set; }
        public string Itemcd { get; set; }
        public string PluUom { get; set; }
        public string ItemName { get; set; }
        public string ItemNameAr { get; set; }
        public decimal Price { get; set; }
        public decimal? PackQty { get; set; }
        public string Dept { get; set; }
        public decimal Tax { get; set; }
        public decimal PromPrice { get; set; }
        public decimal? ActualPrice { get; set; }
        public decimal? Discount { get; set; }
    }
}
