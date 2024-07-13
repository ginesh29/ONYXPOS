namespace Onyx_POS.Models
{
    public class HoldTransHeadViewModel
    {
        public string HBillRefNo { get; set; }
        public int TrnNo { get; set; }
        public int PosId { get; set; }
        public string TrnStatus { get; set; }
        public DateTime TrnDate { get; set; }
        public decimal TrnAmt { get; set; }
        public decimal TrnTotalQty { get; set; }
        public decimal TrnTotalItems { get; set; }
        public decimal? TrnPayNo { get; set; }
        public string TrnComment { get; set; }
        public string TrnUser { get; set; }
        public int TrnShift { get; set; }
        public decimal? TrnTDisc { get; set; }
        public string TrnLoc { get; set; }
        public decimal? RPosId { get; set; }
        public decimal? RTrnno { get; set; }
        public string RTrnUser { get; set; }
        public DateTime? RTrnDate { get; set; }
    }
}
