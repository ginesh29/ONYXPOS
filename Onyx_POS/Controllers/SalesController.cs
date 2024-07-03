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
        public IActionResult Order()
        {
            var terminalDetail = _commonService.GetCuurentPosCtrl();
            ViewBag.TransactionNo = _commonService.GetTransactionNo();
            ViewBag.TerminalDetail = terminalDetail;
            return View();
        }
        public IActionResult FetchSaleItems()
        {
            var PosTempItems = _salesService.GetPosTempItems();
            return PartialView("_SaleItems", PosTempItems);
        }
        [HttpPost]
        public IActionResult AddItem(string barcode)
        {
            var item = _salesService.PluFind(barcode);
            if (item != null)
            {
                var shift = _commonService.GetActiveShiftDetail();
                var counter = _commonService.GetCuurentPosCtrl();
                var totalItems = _salesService.GetPosTempItems().Count();
                var saleItem = new SaleItemModel
                {
                    Barcode = barcode,
                    TrnNo = _commonService.GetTransactionNo(),
                    SrNo = totalItems > 0 ? totalItems + 1 : 1,
                    Dept = item.Dept,
                    Plu = item.Itemcd,
                    Qty = 1,
                    Rate = item.Price,
                    Unit = item.PluUom,
                    PackQty = item.PackQty,
                    Name = item.ItemName,
                    NameAr = item.ItemNameAr,
                    ShiftNo = shift.ShiftNo,
                    User = _loggedInUser.U_Code,
                    POSId = counter.P_PosId,
                    LocId = counter.P_LocId
                };
                _salesService.InsertItem(saleItem);
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
    }
}
