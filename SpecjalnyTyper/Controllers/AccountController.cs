using SpecjalnyTyper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SpecjalnyTyper.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            var validation = Membership.ValidateUser(model.Login, model.Password);
            if (validation)
            {
                HttpCookie aCookie = new HttpCookie("userInfo");
                aCookie.Values["userName"] = model.Login;
                aCookie.Values["lastVisit"] = DateTime.Now.ToString();
                aCookie.Expires = DateTime.Now.AddMinutes(1);
                Response.Cookies.Add(aCookie);
                return RedirectToAction("Index", "Bet");
            }
            else
            {
                ModelState.AddModelError("", "Nieprawidłowy login lub hasło.");
                return View();
            }
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            MembershipCreateStatus status_reg;
            Membership.CreateUser(model.Login, model.ConfirmPassword, model.Email, "admin", "admin", false, out status_reg);
            if (MembershipCreateStatus.Success == status_reg)
            {
                return RedirectToAction("Login", "Account");
            }
            else if(MembershipCreateStatus.InvalidPassword == status_reg)
            {
                ModelState.AddModelError("", "Błędny format hasła.");
            }
            else if(MembershipCreateStatus.InvalidUserName==status_reg)
            {
                ModelState.AddModelError("", "Niepoprawny Login");
            }
            else if (MembershipCreateStatus.InvalidEmail == status_reg)
            {
                ModelState.AddModelError("", "Niepoprawny Email");
            }
            else if (MembershipCreateStatus.DuplicateUserName == status_reg)
            {
                ModelState.AddModelError("", "Użytkownik o danym loginie już istnieje");
            }
            else if (MembershipCreateStatus.DuplicateEmail == status_reg)
            {
                ModelState.AddModelError("", "Użytkownik o danym email'u już istnieje");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            HttpCookie aCookie = new HttpCookie("userInfo");
            aCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(aCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}