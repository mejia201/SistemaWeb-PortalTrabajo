using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaWeb_PortalTrabajo.Context;
using SistemaWeb_PortalTrabajo.Models;
using System.Diagnostics;

namespace SistemaWeb_PortalTrabajo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly portalTrabajoDbContext _portalContext;

        public HomeController(ILogger<HomeController> logger, portalTrabajoDbContext dbContext)
        {
            _logger = logger;
            _portalContext = dbContext;
        }

        public IActionResult Index()
        {
            var empresa = _portalContext.Model.FindEntityType(typeof(Empresa)).GetTableName();

            ViewBag.TableEmpresa = empresa;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult LoginEmpresa()
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