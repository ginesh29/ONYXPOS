namespace Onyx_POS.Models
{
    public class SaleItemModel
    {
        public int TrnNo { get; set; }
        public int SrNo { get; set; }
        public string Plu { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public string Unit { get; set; }
        public decimal Value { get; set; }
        public bool Pending_Insert { get; set; }
        public bool Pending_Update { get; set; }
        public bool Voided { get; set; }
        public string Dept { get; set; }
        public decimal? PackQty { get; set; }
        public string Mode { get; set; }
        public string TrnType { get; set; }
        public string TaxValue { get; set; }
        public decimal TaxAmt { get; set; }
        public string Scalleable { get; set; }
        public string TrnDiscType { get; set; }
        public string TrnMode { get; set; }
        public int POSId { get; set; }
        public string LocId { get; set; }
        public int ShiftNo { get; set; }
        public string User { get; set; }
    }
}