using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NDriveAPI.Models.UserModels
{
    public class SignUpModel
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
    }
}
