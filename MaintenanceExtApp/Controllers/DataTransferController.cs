using MaintenanceExtApp.Data;
using MaintenanceExtApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace MaintenanceExtApp.Controllers
{
    public class DataTransferController : Controller
    {
        private readonly MaintenanceDbContext _context;
        public DataTransferController(MaintenanceDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
