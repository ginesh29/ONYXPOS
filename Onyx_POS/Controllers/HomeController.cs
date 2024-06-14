using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onyx_POS.Services;

namespace Onyx_POS.Controllers
{
    [Authorize]
    public class HomeController(CommonService commonService) : Controller
    {
        private readonly CommonService _commonService = commonService;
        public IActionResult Index()
        {
            var terminalDeatil = _commonService.GetCuurentPosCtrl();
            return View(terminalDeatil);
        }
    }
}
