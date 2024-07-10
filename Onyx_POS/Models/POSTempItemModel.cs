namespace Onyx_POS.Models
{
    public class POSTempItemModel
    {
        public decimal TrnNo { get; set; }
        public decimal TrnSlNo { get; set; }
        public string TrnDept { get; set; }
        public string TrnPlu { get; set; }
        public string TrnType { get; set; }
        public string TrnMode { get; set; }
        public string TrnUser { get; set; }
        public DateTime? TrnDt { get; set; }
        public string TrnTime { get; set; }
        public string TrnErrPlu { get; set; }
        public string TrnLoc { get; set; }
        public string TrnDeptPlu { get; set; }
        public decimal TrnQty { get; set; }
        public string TrnUnit { get; set; }
        public decimal? TrnPackQty { get; set; }
        public decimal TrnPrice { get; set; }
        public string TrnPrLvl { get; set; }
        public decimal? TrnLDisc { get; set; }
        public decimal? TrnLDiscPercent { get; set; }
        public decimal? TrnTDisc { get; set; }
        public string TrnTDiscType { get; set; }
        public decimal? TrnPosId { get; set; }
        public decimal? TrnShift { get; set; }
        public decimal? TrnNetVal { get; set; }
        public string TrnName { get; set; }
        public string TrnNameAr { get; set; }
        public string TrnDesc { get; set; }
        public decimal TrnAmt { get; set; }
        public string TrnSalesman { get; set; }
        public string TrnParty { get; set; }
        public string TrnFlag { get; set; }
        public string TrnBarcode { get; set; }
    }
}
