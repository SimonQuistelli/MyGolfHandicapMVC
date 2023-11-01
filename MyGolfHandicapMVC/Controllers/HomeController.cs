using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyGolfHandicapCore.Models;
using MyGolfHandicapMVC.Data;

namespace MyGolfHandicapMVC.Controllers
{
    public class HomeController : Controller
    {
        private IClientAPI _clientAPI;

        public HomeController(IClientAPI clientAPI)
        {
            _clientAPI = clientAPI;
        }

        public IActionResult Index()
        {
            ViewBag.invalidlogon = false;
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Home()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string txtUserName, string txtPassword)
        {
            int userID = await _clientAPI.GetUserId(txtUserName, txtPassword);

            if (userID != 0)
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,txtUserName)
                    };

                var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimPrinicipal = new ClaimsPrincipal(claimIdentity);
                var props = new AuthenticationProperties();
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrinicipal, props).Wait();
                HttpContext.Session.SetInt32("userid", userID);
                return RedirectToAction("HandicapIndex", "HandicapIndex");
            }
            else
            {
                ViewBag.invalidlogon = true;
                return View("Index");
            }
        }
 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
