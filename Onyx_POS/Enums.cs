using System.ComponentModel.DataAnnotations;

namespace Onyx_POS
{
    public enum TransMode
    {
        [Display(Name = "NR")]
        Normal,
        [Display(Name = "XX")]
        Cancelled
    }
    public enum TransType
    {
        [Display(Name = "SA")]
        Sale,
        [Display(Name = "VO")]
        Void,
        [Display(Name = "SR")]
        SaleReturn,
    }
}
