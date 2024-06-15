using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Onyx_POS.Services;

namespace Onyx_POS.Controllers
{
    [Authorize]
    public class HomeController(CommonService commonService) : Controller
    {
        private readonly CommonService _commonService = commonService;
        [HttpPost]
        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTime.Now.AddYears(100) }
            );
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Index()
        {
            var terminalDeatil = _commonService.GetCuurentPosCtrl();
            return View(terminalDeatil);
        }
    }
}
