namespace Onyx_POS.Models
{
    public class PosCtrlModel
    {
        public string P_LocId { get; set; }
        public string P_LocName { get; set; }
        public DateTime? P_TDate { get; set; }
        public decimal? P_ReceiptNo { get; set; }
        public string P_HeadInAdvance { get; set; }
        public string P_HeadStatus { get; set; }
        public string P_Tmsg1 { get; set; }
        public string P_Tmsg2 { get; set; }
        public string P_Tmsg3 { get; set; }
        public string P_Tmsg4 { get; set; }
        public string P_Tmsg5 { get; set; }
        public string P_Bmsg1 { get; set; }
        public string P_Bmsg2 { get; set; }
        public string P_Bmsg3 { get; set; }
        public string P_OBFlag { get; set; }
        public decimal? P_OBAmt { get; set; }
        public string P_FOCyn { get; set; }
        public string P_DiscTot { get; set; }
        public string P_Duplicate { get; set; }
        public string P_Cancel { get; set; }
        public string P_ReceiptOff { get; set; }
        public string P_Curr { get; set; }
        public decimal? P_PosId { get; set; }
        public string P_Tord1 { get; set; }
        public string P_Tord2 { get; set; }
        public string P_Tord3 { get; set; }
        public string P_Tord4 { get; set; }
        public string P_Tord5 { get; set; }
        public string P_Bord1 { get; set; }
        public string P_Bord2 { get; set; }
        public string P_Bord3 { get; set; }
        public string P_PrintAudit { get; set; }
        public string P_PrintOnline { get; set; }
        public decimal? P_Decimals { get; set; }
        public decimal? P_PriceFactor { get; set; }
        public string P_DeptPlu { get; set; }
        public string P_NotFoundPlu { get; set; }
        public string P_PluPriceChange { get; set; }
        public string P_KeyMapped { get; set; }
        public string P_ConfigReports { get; set; }
        public string P_DeclareCID { get; set; }
        public string P_ShiftBeforeDayend { get; set; }
        public string P_BarcodePrice { get; set; }
        public decimal? P_BarcodeLen { get; set; }
        public decimal? P_PriceLen { get; set; }
        public string P_SystemString { get; set; }
    }
}
