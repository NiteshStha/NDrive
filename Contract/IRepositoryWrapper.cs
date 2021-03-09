using System.Threading.Tasks;

namespace Contract
{
    public interface IRepositoryWrapper
    {
        IUserRepository User { get; }
        IRefreshTokenRepository RefreshToken { get; }

        Task Commit();
        void Save();
    }
}