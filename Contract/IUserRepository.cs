using Entities.Models;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User> Authenticate(string username, string password);
    }
}