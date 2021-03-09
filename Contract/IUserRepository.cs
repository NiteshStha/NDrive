using Entities.Models;
using System.Threading.Tasks;

namespace Contract
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User> Authenticate(string username, string password);
    }
}