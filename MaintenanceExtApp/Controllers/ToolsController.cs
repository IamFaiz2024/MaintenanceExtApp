using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaintenanceExtApp.Data;
using MaintenanceExtApp.Models;

namespace MaintenanceExtApp.Controllers
{
    public class ToolsController : Controller
    {
        private readonly MaintenanceDbContext _context;

        public ToolsController(MaintenanceDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        // GET: Hosts to List
        public IActionResult Transfer2USB()
        {
            var hosts = _context.Hosts.ToList(); // Get all hosts
            return View(hosts);
        }
    }
}
