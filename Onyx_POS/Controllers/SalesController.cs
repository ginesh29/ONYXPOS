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
            var transNo = _commonService.GetTransactionNo();
            ViewBag.TransactionNo = transNo;
            ViewBag.TerminalDetail = terminalDetail;
            return View();
        }
        public IActionResult FetchSaleItems()
        {
            var posTempItems = _salesService.GetPosTempItems();
            foreach (var item in posTempItems)
            {
                var pluItem = _salesService.PluFind(item.TrnBarcode);
                item.TrnNameAr = pluItem.ItemNameAr;
            }
            return PartialView("_SaleItems", posTempItems);
        }
        [HttpPost]
        public IActionResult AddItem(string barcode, string type, int qty = 1)
        {
            var item = _salesService.PluFind(barcode);
            var posTempItems = _salesService.GetPosTempItems();
            var barcodeItemQty = posTempItems.Where(m => m.TrnBarcode == barcode).Sum(m => m.TrnQty);
            bool validQty = type == "Qty" || type == "General" || barcodeItemQty >= Math.Abs(qty);
            if (item != null)
            {
                var shift = _commonService.GetActiveShiftDetail();
                var counter = _commonService.GetCuurentPosCtrl();
                var transNo = _commonService.GetTransactionNo();
                if (validQty)
                {
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
                    _salesService.UpsertPosTransHead(posHead);
                }
            }
            var result = new CommonResponse
            {
                Success = item != null && validQty,
                Message = item == null ? CommonMessage.ITEMNOTFOUND : validQty ? $"{item.ItemName} {CommonMessage.INSERTED}" : "Void Qty exceeded",
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
            var transNo = _commonService.GetTransactionNo();
            var holdTransNo = holdCentralBill ? _commonService.GetHoldTransactionNoRemote() : _commonService.GetHoldTransactionNo();
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
                _salesService.InsertHoldTransDetailsRemote(posTempItems);
                _salesService.UpsertHoldTransHeadRemote(holdTransHead);
            }
            else
            {
                _salesService.InsertHoldTransDetails(posTempItems);
                _salesService.UpsertHoldTransHead(holdTransHead);
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
            _salesService.UpsertPosTransHead(posHead);
            _salesService.ClearPosTempItems(transNo);
            _logService.PosLog("Hold", $"Bill on Hold : {transNo}");
            var result = new CommonResponse
            {
                Success = true,
                Message = $"Bill # {transNo} Hold successfully",
            };
            return Json(result);
        }
        public IActionResult HoldTransactions()
        {
            bool holdCentralBill = _commonService.GetParameterByType("HOLDCENTRALBILL").Val == "Y";
            IEnumerable<HoldTransHeadViewModel> holdTransHeads = holdCentralBill ? _salesService.GetHoldTransHeadsRemote() : _salesService.GetHoldTransHeads();
            return PartialView("_HoldTransactionsModal", holdTransHeads);
        }
        [HttpPost]
        public IActionResult RecallBill(int transNo)
        {
            bool holdCentralBill = _commonService.GetParameterByType("HOLDCENTRALBILL").Val == "Y";
            var holdTransItemsHead = holdCentralBill ? _salesService.GetHoldTransHeadsRemote().FirstOrDefault(m => m.TrnNo == transNo) : _salesService.GetHoldTransHeads().FirstOrDefault(m => m.TrnNo == transNo);

            var holdTransItemsDetails = holdCentralBill ? _salesService.GetHoldTransDetailsRemote(transNo) : _salesService.GetHoldTransDetails(transNo);
            var recallTransNo = _commonService.GetTransactionNo();
            holdTransItemsDetails = holdTransItemsDetails.Select(m => { m.TrnNo = recallTransNo; return m; });
            _salesService.InsertPosTempItems(holdTransItemsDetails);
            var holdTransNo = holdCentralBill ? _commonService.GetHoldTransactionNoRemote() : _commonService.GetHoldTransactionNo();
            var posHead = new PosHead
            {
                TrnNo = transNo,
                BillRefNo = _commonService.GetBillRefNo(transNo),
                PosId = _posDetail.P_PosId,
                User = _loggedInUser.U_Code,
                Shift = _shiftDetail.ShiftNo,
                Status = TransStatus.New.GetDisplayName(),
                Amt = holdTransItemsDetails.Sum(m => m.TrnPrice * m.TrnQty),
                TotalQty = holdTransItemsDetails.Sum(m => m.TrnQty),
                TotalItems = holdTransItemsDetails.Count(),
            };
            _salesService.UpsertPosTransHead(posHead);
            var holdTransHead = new HoldTransHead
            {
                TrnNo = holdTransItemsHead.TrnNo,
                HBillRefNo = holdTransItemsHead.HBillRefNo,
                PosId = holdTransItemsHead.PosId,
                User = holdTransItemsHead.TrnUser,
                Shift = holdTransItemsHead.TrnShift,
                Amt = holdTransItemsHead.TrnAmt,
                TotalQty = holdTransItemsHead.TrnTotalQty,
                TotalItems = holdTransItemsHead.TrnTotalItems,
                Discount = holdTransItemsHead.TrnTDisc,
                Status = TransStatus.Recalled.GetDisplayName(),
                LocId = holdTransItemsHead.TrnLoc,
                TrnDate = holdTransItemsHead.TrnDate,
                RPosId = _posDetail.P_PosId,
                RTrnDate = DateTime.Now,
                RTrnNo = holdTransNo,
                RTrnUser = _loggedInUser.U_Code,
            };
            if (holdCentralBill)
            {
                _salesService.UpsertHoldTransHeadRemote(holdTransHead);
                _salesService.DeleteHoldTransDetailsRemote(transNo);
            }
            else
            {
                _salesService.UpsertHoldTransHead(holdTransHead);
                _salesService.DeleteHoldTransDetails(transNo);
            }
            _logService.PosLog("Recall", $"Recall  Bill {transNo}");
            var result = new CommonResponse
            {
                Success = true,
                Message = $"Bill # {transNo} Recall successfully",
            };
            return Json(result);
        }
        [HttpPost]
        public IActionResult CancelBill(int transNo)
        {
            var posTempItems = _salesService.GetPosTempItems().Where(m => m.TrnNo == transNo).Select(m => { m.TrnMode = TransMode.Cancelled.GetDisplayName(); return m; });
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
            _salesService.UpsertPosTransHead(posHead);
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
                Success = item?.Val == "Y" || key == "allowed" || _loggedInUser.U_Type == UserType.Supervisor.GetDisplayName() || _loggedInUser.U_Type == UserType.Manager.GetDisplayName(),
            };
            return Json(result);
        }
        public IActionResult FetchRecallItemBill(int transNo)
        {
            bool holdCentralBill = _commonService.GetParameterByType("HOLDCENTRALBILL").Val == "Y";
            var holdTransItemsHead = holdCentralBill ? _salesService.GetHoldTransHeadsRemote().FirstOrDefault(m => m.TrnNo == transNo) : _salesService.GetHoldTransHeads().FirstOrDefault(m => m.TrnNo == transNo);

            var holdTransItemsDetails = holdCentralBill ? _salesService.GetHoldTransDetailsRemote(transNo) : _salesService.GetHoldTransDetails(transNo);

            foreach (var item in holdTransItemsDetails)
            {
                var pluItem = _salesService.PluFind(item.TrnBarcode);
                item.TrnNameAr = pluItem.ItemNameAr;
            }
            return PartialView("_SaleItems", holdTransItemsDetails);
        }
    }
}
