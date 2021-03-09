using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class RefreshToken
    {
        [Key] [JsonIgnore] public int RefreshTokenId { get; set; }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpireDate;
        public DateTime CreatedDate { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => RevokedDate == null && !IsExpired;
        [ForeignKey("User")] public int UserId { get; set; }
    }
}
