using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryWrapper
    {
        IUserRepository User { get; }

        Task Commit();
        void Save();
    }
}