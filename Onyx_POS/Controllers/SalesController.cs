using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onyx_POS.Models;
using Onyx_POS.Services;

namespace Onyx_POS.Controllers
{
    [Authorize]
    public class SalesController(CommonService commonService, SalesService salesService, AuthService authService) : Controller
    {
        private readonly CommonService _commonService = commonService;
        private readonly SalesService _salesService = salesService;
        private readonly LoggedInUserModel _loggedInUser = authService.GetLoggedInUser();
        private readonly PosCtrlModel _posDetail = commonService.GetCuurentPosCtrl();
        private readonly ShiftModel _shiftDetail = commonService.GetActiveShiftDetail();
        public IActionResult Order()
        {
            var terminalDetail = _commonService.GetCuurentPosCtrl();
            var transNo = _commonService.GetTransactionNo();
            ViewBag.TransactionNo = transNo > 0 ? transNo + 1 : 1;
            ViewBag.TerminalDetail = terminalDetail;
            return View();
        }
        public IActionResult FetchSaleItems()
        {
            var PosTempItems = _salesService.GetPosTempItems();
            return PartialView("_SaleItems", PosTempItems);
        }
        [HttpPost]
        public IActionResult AddItem(string barcode, int qty = 1)
        {
            var item = _salesService.PluFind(barcode);
            if (item != null)
            {
                var shift = _commonService.GetActiveShiftDetail();
                var counter = _commonService.GetCuurentPosCtrl();
                var posTempItems = _salesService.GetPosTempItems();
                var totalItems = posTempItems.Count();
                var transNo = _commonService.GetTransactionNo();
                var saleItem = new SaleItemModel
                {
                    Barcode = barcode,
                    TrnNo = transNo > 0 ? transNo + 1 : 1,
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
                    TaxAmt = item.Tax * qty
                };
                _salesService.InsertItem(saleItem);
                var posHead = new PosHead
                {
                    PosId = _posDetail.P_PosId,
                    User = _loggedInUser.U_Code,
                    Shift = _shiftDetail.ShiftNo,
                    Status = BillStatus.Hold.GetDisplayName(),
                    Amt = posTempItems.Sum(m => m.TrnPrice * m.TrnQty),
                    TotalQty = posTempItems.Sum(m => m.TrnQty),
                    TotalItems = posTempItems.Count()
                };
                //_salesService.UpdatePosHead(posHead);
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
        public IActionResult HoldBill(bool holdCentralBill)
        {
            int holdTransNo = holdCentralBill ? _commonService.GetTransactionNoRemote() + 1 : _commonService.GetTransactionNo() + 1;
            int transNo = _commonService.GetTransactionNo();
            transNo = transNo > 0 ? transNo : 1;
            var PosTempItems = _salesService.GetPosTempItems().Where(m => m.TrnNo == transNo).Select(m => { m.TrnNo = holdTransNo; return m; });
            var posHead = new PosHead
            {
                PosId = _posDetail.P_PosId,
                User = _loggedInUser.U_Code,
                Shift = _shiftDetail.ShiftNo,
                Status = BillStatus.Hold.GetDisplayName(),
                Amt = PosTempItems.Sum(m => m.TrnPrice * m.TrnQty),
                TotalQty = PosTempItems.Sum(m => m.TrnQty),
                TotalItems = PosTempItems.Count()
            };
            if (holdCentralBill)
            {
                _salesService.InsertPosTransRemote(PosTempItems);
                _salesService.DeletePosHeadRemote(holdTransNo, _posDetail.P_PosId);
                posHead.TrnNo = holdTransNo;
                _salesService.UpdatePosHeadRemote(posHead);
            }
            else
            {
                _salesService.InsertPosTrans(PosTempItems);
                _salesService.DeletePosHead(transNo, _posDetail.P_PosId);
                posHead.TrnNo = transNo;
                _salesService.UpdatePosHead(posHead);
            }
            _salesService.ClearPosTempItems(transNo);

            var result = new CommonResponse
            {
                Success = true,
                Message = $"Bill # {transNo} Hold successfully",
            };
            return Json(result);
        }
    }
}
