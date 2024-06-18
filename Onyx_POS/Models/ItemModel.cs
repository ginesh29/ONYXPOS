namespace Onyx_POS.Models
{
    public class ItemModel
    {
        public string PluCode { get; set; }
        public string PluName { get; set; }
        public string PluNameAr { get; set; }
        public string PluDept { get; set; }
        public string PluUom { get; set; }
        public decimal? PluPrice { get; set; }
        public string PluBarCode { get; set; }
        public string PluVendCode { get; set; }
        public string Scalleable { get; set; }
        public int PackQty { get; set; }
    }
}
