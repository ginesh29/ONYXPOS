using Microsoft.AspNetCore.Mvc;

namespace Onyx_POS.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
