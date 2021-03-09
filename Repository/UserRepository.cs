using System.Linq;
using System.Threading.Tasks;
using Contract;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Utilities.Helpers;

namespace Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = await _repositoryContext.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            // Check if User exists
            if (user == null) return null;

            // check if password is correct and return user if the password is correct
            return !PasswordHasher.Validate(user.Password, password) ? null : user;
        }
    }
}