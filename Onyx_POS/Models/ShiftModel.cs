namespace Onyx_POS.Models
{
    public class ShiftModel
    {
        public string LocId { get; set; }
        public int PosId { get; set; }
        public int ShiftNo { get; set; }
        public string UserId { get; set; }
        public DateTime? StDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StTime { get; set; }
        public string EndTime { get; set; }
        public string Flag { get; set; }
        public string StSrno { get; set; }
        public string EndSrno { get; set; }
        public decimal? Gross { get; set; }
        public decimal? Refund { get; set; }
        public decimal? Void { get; set; }
        public decimal? FOC { get; set; }
        public decimal? FOCRefund { get; set; }
        public decimal? ReturnBal { get; set; }
        public decimal? LineDisc { get; set; }
        public decimal? TotalDisc { get; set; }
        public decimal? CreditCard { get; set; }
        public decimal? GiftVoucher { get; set; }
        public decimal? PaidOut { get; set; }
        public decimal? CnIssue { get; set; }
        public decimal? OnAcRect { get; set; }
        public decimal? NoVoids { get; set; }
        public decimal? NoRefunds { get; set; }
        public decimal? NoFOC { get; set; }
        public decimal? NoFOCRefund { get; set; }
        public decimal? NoPaidOut { get; set; }
        public decimal? NoReceipts { get; set; }
        public decimal? OpenBal { get; set; }
        public decimal? ClosBal { get; set; }
        public decimal? CashDecl { get; set; }
        public string DeclareCID { get; set; }
    }
}
