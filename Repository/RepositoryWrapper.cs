using System.Threading.Tasks;
using Contract;
using Entities;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly RepositoryContext _repositoryContext;
        private IUserRepository _user;
        private IRefreshTokenRepository _refreshToken;
        private IFolderRepository _folder;
        private IFileRepository _file;

        public IUserRepository User => _user ??= new UserRepository(_repositoryContext);
        public IRefreshTokenRepository RefreshToken => _refreshToken ??= new RefreshTokenRepository(_repositoryContext);
        public IFolderRepository Folder => _folder ??= new FolderRepository(_repositoryContext);
        public IFileRepository File => _file ??= new FileRepository(_repositoryContext);

        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task Commit()
        {
            await _repositoryContext.SaveChangesAsync();
        }

        public void Save()
        {
            _repositoryContext.SaveChanges();
        }
    }
}