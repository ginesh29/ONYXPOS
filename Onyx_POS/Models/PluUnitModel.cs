namespace Onyx_POS.Models
{
    public class PluUnitModel
    {
        public char BarCode { get; set; }
        public char PluCode { get; set; }
        public char? PluUom { get; set; }
        public string BarDesc { get; set; }
        public decimal? PackQty { get; set; }
        public decimal? PluPrice { get; set; }
    }
}
