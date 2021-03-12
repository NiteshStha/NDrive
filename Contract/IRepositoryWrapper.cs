using System.Threading.Tasks;

namespace Contract
{
    public interface IRepositoryWrapper
    {
        IUserRepository User { get; }
        IRefreshTokenRepository RefreshToken { get; }
        IFolderRepository Folder { get; }

        Task Commit();
        void Save();
    }
}