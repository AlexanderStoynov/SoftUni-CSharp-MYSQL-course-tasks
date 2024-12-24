using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FastFood.Web.ViewModels;

namespace FastFood.Web.Controllers
{
    public class HormeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
