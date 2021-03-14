using Contract;
using Entities;
using Entities.Models;

namespace Repository
{
    public class FileRepository : RepositoryBase<File>, IFileRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public FileRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }
    }
}