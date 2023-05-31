using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InforceTest.Data;
using InforceTest.Models;
using System.Data.Entity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace InforceTest.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _dbContext;

        public UsersController(AppDbContext context)
        {
            _dbContext = context;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Функція для реєстрацію нових юзерів , записує дані у базу данних , а також у Cookies
        /// </summary>
        /// <param name="model"></param>
        /// <param name="repeatpassword"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,PhoneNumber,FullName,Password")] User model , string repeatpassword)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    PhoneNumber = model.PhoneNumber,
                    FullName = model.FullName,
                    Password = model.Password
                };

                var phoneNumber = _dbContext.Users.SingleOrDefault(e => e.PhoneNumber == model.PhoneNumber); //перевірка чи такий юзер вже є у БД
                if (phoneNumber == null && repeatpassword == model.Password)
                {
                    HttpContext.Response.Cookies.Append("FullName", user.FullName.ToString()); // Використання Cookies

                    _dbContext.Users.Add(user); //запис данних у бд
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Phone number is already registered.");
                    return View(model);
                }
            }

            return View(model);
        }

        public IActionResult Login(string password, string phonenumber, bool rememberMe , bool IsAdmin)
        {
            if (IsAdmin)
            {
                var admin = _dbContext.Users.SingleOrDefault(u => u.PhoneNumber == phonenumber && u.Password == password && u.IsAdmin);
                if (admin!=null)
                {
                    HttpContext.Response.Cookies.Append("FullName", $"ADMIN"+admin.FullName);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "You don`t have permition enter like admin");
                    return View();
                }
            }
            var selectedUser = _dbContext.Users.SingleOrDefault(e => e.PhoneNumber == phonenumber && e.Password == password);
            if (selectedUser != null)
            {
                var fullName = selectedUser.FullName.ToString();

                if (rememberMe)
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddYears(1) // Cookies буде дійсним протягом 1 року
                    };

                    HttpContext.Response.Cookies.Append("FullName", fullName, cookieOptions);
                }
                else
                {
                    HttpContext.Response.Cookies.Append("FullName", fullName);
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid phone number or password.");
                return View();
            }
        }
        /// <summary>
        /// Функція ,яка виходить із поточного акаунта за рахунок очищення Cookies
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            Response.Cookies.Delete("FullName");
            return RedirectToAction("Index", "Home");
        }
    }
}