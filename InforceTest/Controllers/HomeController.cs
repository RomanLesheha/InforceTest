using InforceTest.Data;
using InforceTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Security.Policy;

namespace InforceTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        /// <summary>
        /// Завантажує першу і головну сторінку із формуб для скорочення URL та таблицею усіх URL ,які вже були скорочені
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var obj = _dbContext.URLs.ToList();
            ViewBag.Error = TempData["Error"];
            ViewBag.ShortenedUrl = TempData["ShortenedUrl"];

            return View(obj);
        }
        /// <summary>
        /// Показує сторінку із усіма скороченими URL , а також одним ,яким ви вибрали для детального перегляну на першій сторінці
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult List(Urls selected)
        {
            var obj =  _dbContext.URLs.ToList();
            ViewBag.Selected = selected.LongURL;
            ViewBag.Error = TempData["Error"];
            return View(obj);
        }

        public IActionResult About() 
        { 
            return View();
        }
        /// <summary>
        /// Функція ,яка скорочує URL , та записує його у БД
        /// </summary>
        /// <param name="longUrl"></param>
        /// <returns></returns>
        public async Task<IActionResult> ShortUrlAsync(string longUrl)
        {
            var fullName = HttpContext.Request.Cookies["FullName"];
            if (fullName == null)
            {
                TempData["Error"] = "You are not authorized, please register!";
                return RedirectToAction("Index");
            }
            if (longUrl == null || longUrl.Length <= 30)
            {
                TempData["Error"] = "Your Url have to be longer";
                return RedirectToAction("Index");
            }
            string shortenedUrl = await UrlShorter.MakeTinyUrlByTinyUrl(longUrl);
            var AlreadyHas = _dbContext.URLs.SingleOrDefault(e => e.LongURL == longUrl);
            if (_dbContext.URLs.Contains(AlreadyHas))
            {
                TempData["Error"] = "This URL has already shorted ";
                return RedirectToAction("Index");
            }
            else
            {
                var url = new Urls
                {
                    LongURL = longUrl,
                    ShortURL = shortenedUrl,
                    Created = DateTime.Now,
                    CreatedBy = fullName,
                };
                _dbContext.URLs.Add(url);
                await _dbContext.SaveChangesAsync();
                TempData["ShortenedUrl"] = shortenedUrl;
            }

            return RedirectToAction("Index");
        } 

        /// <summary>
        /// Функція ,яка повертає певну інформацію про один запис
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DetailUrl(int id)
        {
            if (AllowDelete(id))
            {
                var selected = _dbContext.URLs.SingleOrDefault(e => e.Id == id);
                return RedirectToAction("List", selected);
            }
            else
            {
                TempData["Error"] = "You are not authorized, please register!";
                return RedirectToAction("Index");
            }
            
        }


        /// <summary>
        /// Функція ,яка видаляє URL із БД , та робить перевірку чи поточний юзер має право це зробити
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteUrl (int id)
        {
            if (AllowDelete(id))
            {
                var selected = _dbContext.URLs.SingleOrDefault(e => e.Id == id);
                _dbContext.URLs.Remove(selected);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("List");
            }
            TempData["Error"] = "You can`t delete this URL";
            return RedirectToAction("List");
        }
        /// <summary>
        /// Перевірка чи поточний юзер має право видалити поточний запис
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AllowDelete( int id)
        {
            var IsUser = HttpContext.Request.Cookies["FullName"];
            var selected = _dbContext.URLs.SingleOrDefault(e => e.Id == id);
            var admin = _dbContext.Users.SingleOrDefault(u => u.FullName == IsUser && u.IsAdmin);
            if (IsUser != null)
            {
                if (admin != null)
                {
                    return true;
                }
                if (HttpContext.Request.Cookies["FullName"] == selected.CreatedBy)
                {
                    return true;
                }
            }
            return false;
        }
       

      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}