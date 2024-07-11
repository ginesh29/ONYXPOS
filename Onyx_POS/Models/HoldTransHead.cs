namespace Onyx_POS.Models
{
    public class HoldTransHead
    {
        public int TrnNo { get; set; }
        public int PosId { get; set; }
        public string User { get; set; }
        public int Shift { get; set; }
        public string Status { get; set; }
        public decimal Amt { get; set; }
        public decimal? Discount { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalItems { get; set; }
        public string HBillRefNo { get; set; }
        public DateTime TrnDate { get; set; }
        public string LocId { get; set; }
        public int RPosId { get; set; }
        public int RTrnNo { get; set; }
        public string RTrnUser { get; set; }
        public DateTime? RTrnDate { get; set; }
        public string TrnPayNo { get; set; }
    }
}
