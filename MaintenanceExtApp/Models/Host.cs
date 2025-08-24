using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaintenanceExtApp.Models
{

    public enum HostStatus
    {
        Active,
        Offline,
        Cancled        
    }

    public enum HostType
    {
        Windows,
        Linux
    }
    public class Host
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int HostId { get; set; }

        [Required, MaxLength(200)]
        public string HostName { get; set; }

        [Required, MaxLength(500)]
        public string HostAddress { get; set; }

        [MaxLength(500)]
        public string HostSharedPath { get; set; }

        [MaxLength(100)]
        public string HostUserName { get; set; }

        [MaxLength(100)]
        public string HostPassword { get; set; }

        [Required, MaxLength(50)]
        public string HostStatus { get; set; } // "Active" / "Offline" / "Cancled"

        [Required, MaxLength(50)]
        public string HostType { get; set; } // "Windows" / "Linux"
    }
}
