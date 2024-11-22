// контроллер для управления перенаправлением после авторизации

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MyMvcApp.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            // Извлекаем информацию о пользователе из его токенов
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return View(userClaims);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}