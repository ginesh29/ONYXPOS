using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onyx_POS.Models;
using Onyx_POS.Services;

namespace Onyx_POS.Controllers
{
    [Authorize]
    public class SalesController(CommonService commonService, SalesService salesService, AuthService authService, LogService logService) : Controller
    {
        private readonly CommonService _commonService = commonService;
        private readonly SalesService _salesService = salesService;
        private readonly LogService _logService = logService;
        private readonly LoggedInUserModel _loggedInUser = authService.GetLoggedInUser();
        private readonly PosCtrlModel _posDetail = commonService.GetCuurentPosCtrl();
        private readonly ShiftModel _shiftDetail = commonService.GetActiveShiftDetail();
        public IActionResult Order()
        {
            var terminalDetail = _commonService.GetCuurentPosCtrl();
            bool hasTransaction = _commonService.HasTransaction(true);
            var transNo = hasTransaction ? _commonService.GetCurrentTransactionNo() : _commonService.GetNextTransactionNo();
            ViewBag.TransactionNo = transNo;
            ViewBag.TerminalDetail = terminalDetail;
            return View();
        }
        public IActionResult FetchSaleItems()
        {
            var PosTempItems = _salesService.GetPosTempItems();
            foreach (var item in PosTempItems)
            {
                var pluItem = _salesService.PluFind(item.TrnBarcode);
                item.TrnNameAr = pluItem.ItemNameAr;
            }
            return PartialView("_SaleItems", PosTempItems);
        }
        [HttpPost]
        public IActionResult AddItem(string barcode, string type, int qty = 1)
        {
            var item = _salesService.PluFind(barcode);
            if (item != null)
            {
                var shift = _commonService.GetActiveShiftDetail();
                var counter = _commonService.GetCuurentPosCtrl();
                bool hasTransaction = _commonService.HasTransaction(true);
                var transNo = hasTransaction ? _commonService.GetCurrentTransactionNo() : _commonService.GetNextTransactionNo();
                var posTempItems = _salesService.GetPosTempItems();
                var totalItems = posTempItems.Count();
                var saleItem = new SaleItemModel
                {
                    Barcode = barcode,
                    TrnNo = transNo,
                    SrNo = totalItems > 0 ? totalItems + 1 : 1,
                    Dept = item.Dept,
                    Plu = item.Itemcd,
                    Qty = qty,
                    Rate = item.Price,
                    Unit = item.PluUom,
                    PackQty = item.PackQty,
                    Name = item.ItemName,
                    NameAr = item.ItemNameAr,
                    ShiftNo = shift.ShiftNo,
                    User = _loggedInUser.U_Code,
                    POSId = counter.P_PosId,
                    LocId = counter.P_LocId,
                    TaxAmt = Math.Round(item.Price * qty / (100 + Convert.ToDecimal(item.Tax)) * 100 / 100 * Convert.ToDecimal(item.Tax), 2),
                    TrnMode = TransMode.Normal.GetDisplayName(),
                    TrnType = type == "Refund" ? TransType.SaleReturn.GetDisplayName() : TransType.Sale.GetDisplayName(),
                };
                _salesService.InsertItem(saleItem);
                var updatedPosTempItems = _salesService.GetPosTempItems();
                var posHead = new PosHead
                {
                    TrnNo = transNo,
                    BillRefNo = _commonService.GetBillRefNo(transNo),
                    PosId = _posDetail.P_PosId,
                    User = _loggedInUser.U_Code,
                    Shift = _shiftDetail.ShiftNo,
                    Status = TransStatus.New.GetDisplayName(),
                    Amt = updatedPosTempItems.Sum(m => m.TrnPrice * m.TrnQty),
                    TotalQty = updatedPosTempItems.Sum(m => m.TrnQty),
                    TotalItems = updatedPosTempItems.Count()
                };
                _salesService.UpdatePosTransHead(posHead);
            }
            var result = new CommonResponse
            {
                Success = item != null,
                Message = item == null ? CommonMessage.ITEMNOTFOUND : $"{item.ItemName} {CommonMessage.INSERTED}",
            };
            return Json(result);
        }
        public IActionResult GetPriceCheckItems(string code)
        {
            var items = _salesService.GetPriceCheckItems(code);
            return PartialView("_PriceCheckItems", items);
        }
        [HttpPost]
        public IActionResult HoldBill()
        {
            bool holdCentralBill = _commonService.GetParameterByType("HOLDCENTRALBILL").Val == "Y";
            int transNo = _commonService.GetCurrentTransactionNo();
            int holdTransNo = holdCentralBill ? _commonService.GetHoldTransactionNoRemote() : _commonService.GetHoldTransactionNo();
            var posTempItems = _salesService.GetPosTempItems().Select(m =>
            {
                m.TrnNo = holdTransNo;
                m.HBillRefNo = _commonService.GetHoldCancelledRefNo(transNo, TransStatus.Hold.GetDisplayName());
                return m;
            });
            var holdTransHead = new HoldTransHead
            {
                TrnNo = holdTransNo,
                HBillRefNo = _commonService.GetHoldCancelledRefNo(transNo, TransStatus.Hold.GetDisplayName()),
                PosId = _posDetail.P_PosId,
                User = _loggedInUser.U_Code,
                Shift = _shiftDetail.ShiftNo,
                Status = TransStatus.Hold.GetDisplayName(),
                Amt = posTempItems.Sum(m => m.TrnPrice * m.TrnQty),
                TotalQty = posTempItems.Sum(m => m.TrnQty),
                TotalItems = posTempItems.Count(),
                Discount = posTempItems.Sum(m => m.TrnTDisc),
                LocId = _posDetail.P_LocId,
                TrnDate = DateTime.Now,
            };
            if (holdCentralBill)
            {
                _salesService.InsertHoldTransRemote(posTempItems);
                _salesService.UpdateHoldTransHeadRemote(holdTransHead);
            }
            else
            {
                _salesService.InsertHoldTrans(posTempItems);
                _salesService.UpdateHoldTransHead(holdTransHead);
            }
            var posHead = new PosHead
            {
                TrnNo = transNo,
                BillRefNo = _commonService.GetHoldCancelledRefNo(transNo, TransStatus.Hold.GetDisplayName()),
                PosId = _posDetail.P_PosId,
                User = _loggedInUser.U_Code,
                Shift = _shiftDetail.ShiftNo,
                Status = TransStatus.Hold.GetDisplayName(),
                Amt = posTempItems.Sum(m => m.TrnPrice * m.TrnQty),
                TotalQty = posTempItems.Sum(m => m.TrnQty),
                TotalItems = posTempItems.Count()
            };
            _salesService.UpdatePosTransHead(posHead);
            _salesService.ClearPosTempItems(transNo);
            _logService.PosLog("Hold", $"Bill on Hold : {transNo}");
            var result = new CommonResponse
            {
                Success = true,
                Message = $"Bill # {transNo} Hold successfully",
            };
            return Json(result);
        }
        [HttpPost]
        public IActionResult CancelBill(int transNo)
        {
            var posTempItems = _salesService.GetPosTempItems().Where(m => m.TrnNo == transNo).Select(m => { m.TrnMode = TransMode.Cancelled.GetDisplayName(); return m; });
            _salesService.InsertPosTrans(posTempItems);
            var posHead = new PosHead
            {
                TrnNo = transNo,
                BillRefNo = _commonService.GetHoldCancelledRefNo(transNo, TransStatus.Cancelled.GetDisplayName()),
                PosId = _posDetail.P_PosId,
                User = _loggedInUser.U_Code,
                Shift = _shiftDetail.ShiftNo,
                Status = TransStatus.Cancelled.GetDisplayName(),
                Amt = posTempItems.Sum(m => m.TrnPrice * m.TrnQty),
                TotalQty = posTempItems.Sum(m => m.TrnQty),
                TotalItems = posTempItems.Count()
            };
            _salesService.UpdatePosTransHead(posHead);
            _salesService.ClearPosTempItems(transNo);
            _logService.PosLog("Cancel", $"Cancel  Bill {transNo}");
            var result = new CommonResponse
            {
                Success = true,
                Message = $"Bill # {transNo} Cancel successfully",
            };
            return Json(result);
        }
        public IActionResult CheckAuth(string key)
        {
            var item = _commonService.GetParameterByType(key);
            var result = new CommonResponse
            {
                Success = true,
                Data = new { allowed = item.Val == "Y" || _loggedInUser.U_Code == "001" }
            };
            return Json(result);
        }
    }
}
