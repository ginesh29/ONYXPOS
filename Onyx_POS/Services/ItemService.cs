using Onyx_POS.Helpers;
using Onyx_POS.Models;

namespace Onyx_POS.Services
{
    public class ItemService(CommonService commonService, SalesService salesService)
    {
        private readonly CommonService _commonService = commonService;
        private readonly SalesService _salesService = salesService;
        private readonly ValidationHelper validationHelper = new();
        public bool ValidatedWithCheckDigit(string code)
        {
            bool SCALE_CHK_DIGIT = Convert.ToBoolean(_commonService.GetParameterByType("SCALE_CHK_DIGIT").Val);
            if (SCALE_CHK_DIGIT)
                if (validationHelper.IsEAN13_Standard(long.Parse(code))) return true;
                else return false;
            else return true;
        }
        //public PluItemModel GetItemFromPluMaster(string code)
        //{
        //    int wtPlu_Len = Convert.ToInt32(_commonService.GetParameterByType("WTPLU_LEN").Val);
        //    int ScaleDivVal = 100;
        //    switch (wtPlu_Len)
        //    {
        //        case 5:
        //            ScaleDivVal = 1000;
        //            break;
        //        case 6:
        //            ScaleDivVal = 100;
        //            break;
        //        case 7:
        //            ScaleDivVal = 10;
        //            break;
        //    }
        //    PluItemModel item = new();
        //    if (code.Length >= wtPlu_Len)
        //    {
        //        item = _salesService.GetPluItems(code).FirstOrDefault();
        //        if (item != null)
        //        {
        //            int sclQty_Len = Convert.ToInt32(_commonService.GetParameterByType("QTYPLU_LEN").Val);
        //            item.PluVendCode = !string.IsNullOrEmpty(item.PluVendCode) ? item.PluVendCode.Trim() : "0";
        //            if (item.Scalleable.ToUpper().Equals("W") || item.Scalleable.ToUpper().Equals("P") || item.Scalleable.ToUpper().Equals("Q"))
        //                if (!ValidatedWithCheckDigit(code)) return null;
        //            if (item.Scalleable.Trim() == "W" && code.Length > wtPlu_Len)
        //            {
        //                if (code.Length == 8)
        //                    return null;
        //                else
        //                {
        //                    int WtQty_Len = Convert.ToInt32(_commonService.GetParameterByType("WTQTY_LEN").Val);
        //                    switch (WtQty_Len)
        //                    {
        //                        case 5:
        //                            ScaleDivVal = 100;
        //                            break;
        //                        case 6:
        //                            ScaleDivVal = 1000;
        //                            break;
        //                        case 7:
        //                            ScaleDivVal = 10000;
        //                            break;
        //                    }
        //                    if (code.Length >= (2 * WtQty_Len))
        //                    {
        //                        long num = long.Parse(code) / 10;
        //                        int wtQty_pos = Convert.ToInt32(_commonService.GetParameterByType("WTQTY_POS").Val);
        //                        string ns = num.ToString().Substring(wtQty_pos - 1, WtQty_Len);
        //                        item.Qty = Convert.ToDecimal(ns) / Convert.ToDecimal(Math.Pow(10, ns.Length - 2));
        //                    }
        //                    else
        //                    {
        //                        var itemUnit = _salesService.GetPluItemUnits(code).FirstOrDefault();
        //                        if (itemUnit != null)
        //                            item.Qty = 1;
        //                    }
        //                }
        //            }
        //            else if (item.Scalleable.Trim() == "P")
        //            {
        //                item.Qty = 1;
        //                int WtPrice_Len = Convert.ToInt32(_commonService.GetParameterByType("WtPrice_Len").Val);
        //                item.PluPrice = Convert.ToDecimal(code.Substring(wtPlu_Len, WtPrice_Len).TrimStart(['0'])) / ScaleDivVal;
        //            }
        //            else if (item.Scalleable.Trim() == "Q")
        //            {
        //                switch (sclQty_Len)
        //                {
        //                    case 5:
        //                        ScaleDivVal = 100;
        //                        break;
        //                    case 6:
        //                        ScaleDivVal = 1000;
        //                        break;
        //                    case 7:
        //                        ScaleDivVal = 10000;
        //                        break;
        //                }
        //                if (code.Length < 7)
        //                    item.Qty = 1;
        //                else
        //                    item.Qty = Convert.ToDecimal(code.Substring(wtPlu_Len, sclQty_Len).TrimStart(['0']));
        //            }
        //        }
        //        else
        //        {
        //            code = code[..wtPlu_Len];
        //            item = _salesService.GetPluItems(code).FirstOrDefault();
        //        }
        //    }
        //    return item;
        //}
        //public PluItemModel GetItemFromPluUnitMaster(string code)
        //{
        //    var unit = _salesService.GetPluItemUnits(code).FirstOrDefault();
        //    string newCode = code;
        //    int ScaleDivVal = 100;
        //    int wtPlu_Len = Convert.ToInt32(_commonService.GetParameterByType("WTPLU_LEN").Val);
        //    switch (wtPlu_Len)
        //    {
        //        case 5:
        //            ScaleDivVal = 1000;
        //            break;
        //        case 6:
        //            ScaleDivVal = 100;
        //            break;
        //        case 7:
        //            ScaleDivVal = 10;
        //            break;
        //    }
        //    if (code.Length >= wtPlu_Len)
        //    {
        //        if (unit != null)
        //        {
        //            int sclQty_Len = Convert.ToInt32(_commonService.GetParameterByType("QTYPLU_LEN").Val);
        //            var item = _salesService.GetPluItems(code).FirstOrDefault();
        //            if (item.Scalleable.Trim() == "W" && code.Length > wtPlu_Len)
        //            {
        //                if (code.Length == 8)
        //                    item.Qty = 1;
        //                else
        //                {
        //                    int WtQty_Len = Convert.ToInt32(_commonService.GetParameterByType("WTQTY_LEN").Val);
        //                    switch (WtQty_Len)
        //                    {
        //                        case 5:
        //                            ScaleDivVal = 100;
        //                            break;
        //                        case 6:
        //                            ScaleDivVal = 1000;
        //                            break;
        //                        default:
        //                            if (sclQty_Len == 7)
        //                                ScaleDivVal = 10000;
        //                            break;
        //                    }
        //                    item.Qty = Convert.ToDecimal(code.Substring(wtPlu_Len, WtQty_Len).TrimStart(['0'])) / ScaleDivVal;
        //                }
        //            }

        //            else if (item.Scalleable.Trim() == "P")
        //            {
        //                item.Qty = 1;
        //                int WtPrice_Len = Convert.ToInt32(_commonService.GetParameterByType("WtPrice_Len").Val);
        //                item.PluPrice = Convert.ToDecimal(code.Substring(wtPlu_Len, WtPrice_Len).TrimStart(['0'])) / ScaleDivVal;
        //            }
        //            else if (item.Scalleable.Trim() == "Q")
        //            {
        //                switch (sclQty_Len)
        //                {
        //                    case 5:
        //                        ScaleDivVal = 100;
        //                        break;
        //                    case 6:
        //                        ScaleDivVal = 1000;
        //                        break;
        //                    case 7:
        //                        ScaleDivVal = 10000;
        //                        break;
        //                }
        //                item.Qty = Convert.ToDecimal(code.Substring(wtPlu_Len, sclQty_Len).TrimStart(['0']));
        //            }
        //            return item;
        //        }
        //        else
        //        {
        //            code = code[..wtPlu_Len];
        //            unit = _salesService.GetPluItemUnits(code).FirstOrDefault();
        //        }                
        //    }
        //    return null;

        //}
    }
}
