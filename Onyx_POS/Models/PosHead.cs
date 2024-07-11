namespace Onyx_POS.Models
{
    public class PosHead
    {
        public int TrnNo { get; set; }
        public int PosId { get; set; }
        public string User { get; set; }
        public int Shift { get; set; }
        public string Status { get; set; }
        public decimal Amt { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalItems { get; set; }
        public string BillRefNo { get; set; }
    }
}
