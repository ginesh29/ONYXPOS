using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onyx_POS.Services;

namespace Onyx_POS.Controllers
{
    [Authorize]
    public class SalesController(CommonService commonService, SalesService salesService) : Controller
    {
        private readonly CommonService _commonService = commonService;
        private readonly SalesService _salesService = salesService;
        public IActionResult Order()
        {
            var terminalDetail = _commonService.GetCuurentPosCtrl();
            ViewBag.TransactionNo = _commonService.GetTransactionNo();
            ViewBag.TerminalDetail = terminalDetail;
            return View();
        }
        public IActionResult GetPriceCheckItems(string code)
        {
            var items = _salesService.GetItemFromPLU(code);
            return PartialView("_PriceCheckItems", items);
        }
    }
}
