using Microsoft.AspNetCore.Mvc;
using Onyx_POS.Helpers;
using Onyx_POS.Models;
using Onyx_POS.Services;

namespace Onyx_POS.Controllers
{
    public class AccountController(CommonService commonService, AuthService authService) : Controller
    {
        private readonly CommonService _commonService = commonService;
        private readonly AuthService _authService = authService;

        public IActionResult Login()
        {
            _commonService.GenerateModifiedSp();
            var terminalDeatil = _commonService.GetCuurentPosCtrl();
            ViewBag.TerminalDeatil = terminalDeatil;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var result = new CommonResponse { Success = false };
            model.Password = model.Password.Encrypt();
            var validateUser = _authService.ValidateUser(model);
            if (validateUser != null)
            {
                await _authService.SignOutAsync();
                await _authService.SignInUserAsync(validateUser);
                result.Success = true;
                result.Message = "Login Successfully";
            }
            else
                result.Message = CommonMessage.INVALIDUSER;
            return Json(result);
        }
        public async Task<IActionResult> LogOut()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        //public IActionResult ChangePassword()
        //{
        //    return PartialView("_ChangePassword");
        //}
        //[HttpPost]
        //public IActionResult ChangePassword(ChangePassword model)
        //{
        //    var result = new CommonResponse { Success = false };
        //    if (_loggedInUser.UserType == (int)UserTypeEnum.User)
        //    {
        //        var user = _userService.ValidateUser(new LoginModel
        //        {
        //            LoginId = _loggedInUser.LoginId,
        //            Password = model.OldPassword
        //        });
        //        if (user != null)
        //        {
        //            var userFromDb = _userService.GetUsers(_loggedInUser.UserCd, _loggedInUser.CoAbbr).FirstOrDefault();
        //            _settingService.SaveUser(new UserModel
        //            {
        //                Cd = userFromDb.Code,
        //                Code = userFromDb.Code,
        //                LoginId = userFromDb.LoginId,
        //                Abbr = userFromDb.Abbr,
        //                UPwd = model.ConfirmPassword.Encrypt(),
        //                Username = userFromDb.Username,
        //                ExpiryDt = userFromDb.ExpiryDt,
        //                EntryBy = _loggedInUser.UserCd,
        //            });
        //            result.Success = true;
        //            result.Message = "Password changed Successfully";
        //        }
        //        else
        //            result.Message = "Old Password is not valid";
        //    }
        //    else
        //    {
        //        var employee = _userService.ValidateEmployee(new LoginModel
        //        {

        //            LoginId = _loggedInUser.LoginId,
        //            Password = model.OldPassword,
        //        });
        //        if (employee != null)
        //        {
        //            _employeeService.UpdateEmployeePassword(_loggedInUser.CompanyCd, _loggedInUser.UserCd, model.ConfirmPassword);
        //            result.Success = true;
        //            result.Message = "Password changed Successfully";
        //        }
        //        else
        //            result.Message = "Old Password is not valid";
        //    }
        //    return Json(result);
        //}
    }
}
