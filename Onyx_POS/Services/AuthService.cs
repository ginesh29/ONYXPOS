using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Onyx_POS.Models;
using System.Security.Claims;
using Onyx_POS.Data;

namespace Onyx_POS.Services
{
    public class AuthService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        private readonly AppDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public LoggedInUserModel ValidateUser(LoginModel model)
        {
            var query = $"Select U_Code ,U_Name,U_Type from UserMast where (U_Code ='{model.UserId}' or U_Name='{model.UserId}') AND U_Pw='{model.Password}'";
            using var connection = _context.CreateConnection();
            var data = connection.QueryFirstOrDefault<LoggedInUserModel>(query);
            return data;
        }
        public async Task SignInUserAsync(LoggedInUserModel model)
        {
            var claims = new List<Claim>()
            {
                new("UserCode", model.U_Code),
                new("UserName", model.U_Name),
                new("UserType",model.U_Type)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var prop = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.Now.AddYears(100),
            };
            await _httpContextAccessor.HttpContext.SignInAsync(claimsPrincipal, prop);
        }
        public async Task SignOutAsync()
        {
            //_httpContextAccessor.HttpContext.Response.Cookies.Delete(CookieRequestCultureProvider.DefaultCookieName);
            await _httpContextAccessor.HttpContext.SignOutAsync();
        }
        public LoggedInUserModel GetLoggedInUser()
        {
            var user = new LoggedInUserModel();
            var auth = _httpContextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme).Result;
            string result = string.Empty;
            if (auth.Succeeded)
            {
                var claims = auth.Principal.Claims;
                if (claims.Any())
                {
                    user.U_Code = claims.FirstOrDefault(m => m.Type == "UserCode")?.Value;
                    user.U_Name = claims.FirstOrDefault(m => m.Type == "UserName")?.Value;
                    user.U_Type = claims.FirstOrDefault(m => m.Type == "UserType")?.Value;
                }
            }
            return user;
        }
        public async Task UpdateClaim(string key, string value)
        {
            ClaimsPrincipal user = _httpContextAccessor.HttpContext.User;
            ClaimsIdentity identity = (ClaimsIdentity)user.Identity;
            Claim oldClaim = identity.FindFirst(key);
            if (oldClaim != null)
                identity.RemoveClaim(oldClaim);
            if (identity.Claims.Any())
            {
                Claim newClaim = new(key, value);
                identity.AddClaim(newClaim);
                var prop = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.Now.AddYears(100)
                };
                await _httpContextAccessor.HttpContext.SignInAsync(user, prop);
            }
        }
    }
}