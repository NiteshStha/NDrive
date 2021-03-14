using NDriveAPI.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Responses;

namespace NDriveAPI.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> Authenticate(SignInModel model, string ipAddress);
        Task<AuthenticationResponse> RefreshToken(string token, string ipAddress);
        Task<bool> RevokeToken(string token, string ipAddress);
    }
}
