using System;
using MaintenanceExtApp.Data;
using MaintenanceExtApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.IO.Compression;
using Renci.SshNet;

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

        [HttpPost]
        public IActionResult TransferToUSB(int windowsHostId, int linuxHostId)
        {
            // Your transfer logic here
            //Console.WriteLine($"Transfer2USB Button pressed - Windows ID: {windowsHostId}, Linux ID: {linuxHostId}");
            //return RedirectToAction("Transfer2USB");
            try
            {
                var windowsHost = _context.Hosts.Find(windowsHostId);
                var linuxHost = _context.Hosts.Find(linuxHostId);

                var zipFileName = DateTime.Now.ToString("yyyyMMddhhmm") + ".zip";
                var tempZipPath = Path.Combine(Path.GetTempPath(), zipFileName);

                ZipFilesFromSSHHost(windowsHost, tempZipPath);

                string remotePath = ""; // Declare outside

                using (var scp = new ScpClient(linuxHost.HostAddress, linuxHost.HostUserName, linuxHost.HostPassword))
                {
                    scp.Connect();
                    remotePath = $"{linuxHost.HostSharedPath}/{zipFileName}";
                    scp.Upload(new FileInfo(tempZipPath), remotePath);
                    scp.Disconnect();
                }

                System.IO.File.Delete(tempZipPath);
                TempData["Message"] = $"Transfer completed! File: {remotePath}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Transfer failed: {ex.Message}";
            }

            return RedirectToAction("Transfer2USB");
        }

        [HttpPost]
        public IActionResult TransferFromUSB(int windowsHostId, int linuxHostId)
        {
            // Your transfer logic here
            Console.WriteLine($"TransferFromUSB Button pressed - Linux ID: {linuxHostId}, Windows ID: {windowsHostId}");
            
            
            return RedirectToAction("Transfer2USB");
        }

        private void ZipFilesFromSSHHost(Models.Host host, string zipOutputPath)
        {
            using (var sftp = new SftpClient(host.HostAddress, host.HostUserName, host.HostPassword))
            {
                sftp.Connect();

                using (var zip = ZipFile.Open(zipOutputPath, ZipArchiveMode.Create))
                {
                    DownloadDirectoryToZip(sftp, host.HostSharedPath, "", zip);
                }

                sftp.Disconnect();
            }
        }

        private void DownloadDirectoryToZip(SftpClient sftp, string remotePath, string relativePath, ZipArchive zip)
        {
            foreach (var item in sftp.ListDirectory(remotePath))
            {
                if (item.Name == "." || item.Name == "..") continue;

                string newRelativePath = Path.Combine(relativePath, item.Name);

                if (item.IsDirectory)
                {
                    DownloadDirectoryToZip(sftp, item.FullName, newRelativePath, zip);
                }
                else
                {
                    var tempFile = Path.GetTempFileName();
                    using (var fileStream = System.IO.File.OpenWrite(tempFile))
                    {
                        sftp.DownloadFile(item.FullName, fileStream);
                    }

                    zip.CreateEntryFromFile(tempFile, newRelativePath);
                    System.IO.File.Delete(tempFile);
                }
            }
        }
    }
}
