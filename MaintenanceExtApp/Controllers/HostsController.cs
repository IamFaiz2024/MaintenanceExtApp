using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaintenanceExtApp.Data;
using MaintenanceExtApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MaintenanceExtApp.Controllers
{
    public class HostsController : Controller
    {
        private readonly MaintenanceDbContext _context;

        public HostsController(MaintenanceDbContext context)
        {
            _context = context;
        }       

        // GET: Hosts
        public async Task<IActionResult> Index()
        {
            var hosts = await _context.Hosts.ToListAsync();
            return View(hosts);
        }

        // POST: Hosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HostName,HostAddress,HostSharedPath,HostUserName,HostPassword,HostStatus,HostType")] Models.Host host)
        {
            // Debug: Check what values are coming in
            Console.WriteLine($"HostType: {host.HostType}");
            Console.WriteLine($"HostStatus: {host.HostStatus}");

            if (ModelState.IsValid)
            {
                _context.Hosts.Add(host);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Hosts/Edit (Inline Edit)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int HostId, [Bind("HostName,HostAddress,HostSharedPath,HostUserName,HostPassword,HostStatus,HostType")] Models.Host updatedHost)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            var host = await _context.Hosts.FindAsync(HostId);
            if (host == null)
                return NotFound();

            // Update ALL host properties including HostType
            host.HostName = updatedHost.HostName;
            host.HostAddress = updatedHost.HostAddress;
            host.HostSharedPath = updatedHost.HostSharedPath;
            host.HostUserName = updatedHost.HostUserName;
            host.HostPassword = updatedHost.HostPassword;
            host.HostStatus = updatedHost.HostStatus;
            host.HostType = updatedHost.HostType; 

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Hosts/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int HostId)
        {
            var host = await _context.Hosts.FindAsync(HostId);
            if (host != null)
            {
                _context.Hosts.Remove(host);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
