using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenanceExtApp.Models
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }
    public class LogEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; }

        [Required]
        [MaxLength(20)]
        public string LogLevel { get; set; } = "Info"; // Default level

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
