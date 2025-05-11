using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapplication.Models;

namespace webapplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Username == "admin" && model.Password == "admin123")
                    {
                        // Admin girişi başarılı - doğrudan URL'ye yönlendir
                        return Redirect("/Admin");
                    }
                    else if (model.Username == "user" && model.Password == "user123")
                    {
                        // Kullanıcı girişi başarılı
                        return Redirect("/Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda
                ModelState.AddModelError(string.Empty, "Giriş işlemi sırasında bir hata oluştu: " + ex.Message);
            }

            return View(model);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            // Burada oturum kapatma işlemleri yapılabilir
            return RedirectToAction("Index", "Home");
        }
    }
}
