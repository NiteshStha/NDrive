using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Utilities.Responses
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticationResponse(int id, string username, string firstName, string lastName, string jwtToken,
            string refreshToken)
        {
            Id = id;
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
