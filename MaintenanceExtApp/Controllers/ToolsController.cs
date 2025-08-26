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
            try
            {
                Console.WriteLine($"Transfer started - Windows ID: {windowsHostId}, Linux ID: {linuxHostId}");

                var windowsHost = _context.Hosts.Find(windowsHostId);
                var linuxHost = _context.Hosts.Find(linuxHostId);

                Console.WriteLine($"Windows Host: {windowsHost.HostName}, Linux Host: {linuxHost.HostName}");

                var zipFileName = DateTime.Now.ToString("yyMMMddHHmm", CultureInfo.InvariantCulture).ToUpper() + ".zip";
                var tempZipPath = Path.Combine(Path.GetTempPath(), zipFileName);

                Console.WriteLine($"Creating zip: {tempZipPath}");

                ZipFilesUsingPowerShell(windowsHost, tempZipPath);
                Console.WriteLine("Zip created successfully");

                string remotePath = "";
                using (var scp = new ScpClient(linuxHost.HostAddress, linuxHost.HostUserName, linuxHost.HostPassword))
                {
                    Console.WriteLine("Connecting to Linux host...");
                    scp.Connect();
                    Console.WriteLine("Connected to Linux host");

                    remotePath = $"{linuxHost.HostSharedPath}/{zipFileName}";
                    Console.WriteLine($"Uploading to: {remotePath}");

                    scp.Upload(new FileInfo(tempZipPath), remotePath);
                    Console.WriteLine("Upload completed");

                    scp.Disconnect();
                }

                System.IO.File.Delete(tempZipPath);
                Console.WriteLine("Temp file cleaned up");

                TempData["Message"] = $"Transfer completed! File: {remotePath}";
                Console.WriteLine("Transfer successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
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

        private void ZipFilesUsingPowerShell(Models.Host windowsHost, string zipOutputPath)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"Compress-Archive -Path '\\\\{windowsHost.HostAddress}\\{windowsHost.HostSharedPath}\\*' -DestinationPath '{zipOutputPath}'\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                process.WaitForExit();
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
