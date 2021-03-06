using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Entities.Models
{
    public class User
    {
        [Key] public int UserId { get; set; }

        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Username { get; set; }
        [JsonIgnore] [Required] public string Password { get; set; }
        [Required] public string Email { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        public DateTime CreatedDate { get; set; }

        [JsonIgnore] public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}