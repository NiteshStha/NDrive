using Contract;
using Entities.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NDriveAPI.Helpers;
using NDriveAPI.Models.User;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utilities.Responses;

namespace NDriveAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepositoryWrapper _repository;
        private readonly AppSettings _appSettings;

        public AuthenticationService(IRepositoryWrapper repo, IOptions<AppSettings> appSettings)
        {
            this._repository = repo;
            this._appSettings = appSettings.Value;
        }

        public async Task<AuthenticationResponse> Authenticate(SignInModel model, string ipAddress)
        {
            var user = await _repository.User.Authenticate(model.Username, model.Password);

            if (user == null) return null;

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(user);
            var refreshToken = generateRefreshToken(ipAddress);

            // save refresh token
            refreshToken.UserId = user.UserId;
            await _repository.RefreshToken.Create(refreshToken);
            _repository.User.Update(user);
            await _repository.Commit();

            return new AuthenticationResponse(
                user.UserId, user.Username, user.FirstName, user.LastName, jwtToken, refreshToken.Token
            );
        }

        public async Task<AuthenticationResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await _repository.User
                .FindById(u => u.RefreshTokens
                    .Any(t => t.Token == token));

            // return null if no user found with token
            if (user == null) return null;

            var refreshToken = await _repository.RefreshToken
                .FindById(t => t.UserId == user.UserId && t.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.RevokedDate = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            newRefreshToken.UserId = user.UserId;
            await _repository.RefreshToken.Create(newRefreshToken);
            _repository.User.Update(user);
            await _repository.Commit();

            // generate new jwt
            var jwtToken = generateJwtToken(user);

            return new AuthenticationResponse(
                user.UserId, user.Username, user.FirstName, user.LastName, jwtToken, refreshToken.Token
            );
        }

        public async Task<bool> RevokeToken(string token, string ipAddress)
        {
            var user = await _repository.User.FindById(u => u.RefreshTokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = await _repository.RefreshToken
                .FindById(t => t.UserId == user.UserId && t.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.RevokedDate = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _repository.User.Update(user);
            await _repository.Commit();

            return true;
        }

        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                ExpireDate = DateTime.UtcNow.AddDays(7),
                CreatedDate = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
    }
}