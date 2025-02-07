﻿using Microsoft.AspNetCore.Mvc;
using Onyx_POS.Helpers;
using Onyx_POS.Models;
using Onyx_POS.Services;

namespace Onyx_POS.Controllers
{
    public class AccountController(CommonService commonService, AuthService authService, LogService logService) : Controller
    {
        private readonly CommonService _commonService = commonService;
        private readonly AuthService _authService = authService;
        private readonly LogService _logService = logService;
        public IActionResult Login()
        {
            _commonService.GenerateModifiedSp();
            var terminalDetail = _commonService.GetCuurentPosCtrl();
            ViewBag.TerminalDetail = terminalDetail;
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
                _logService.PosLog("LOGIN", "Pos Logged in");
            }
            else
                result.Message = CommonMessage.INVALIDUSER;
            return Json(result);
        }
        [HttpPost]
        [HttpPost]
        public IActionResult ReAuthSupervisorManager(LoginModel model)
        {
            var result = new CommonResponse { Success = false };
            model.Password = model.Password.Encrypt();
            var validateUser = _authService.ValidateUser(model);
            if (validateUser != null && (validateUser.U_Type == UserType.Supervisor.GetDisplayName() || validateUser.U_Type == UserType.Manager.GetDisplayName()))
            {
                result.Success = true;
                result.Message = "Privilege granted to do this Operation";
                _logService.PosLog("LOGIN", "Pos Logged in");
            }
            else
                result.Message = "No Privilege to do this Operation";
            return Json(result);
        }
        public async Task<IActionResult> LogOut()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
