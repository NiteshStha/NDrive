using Contract;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class FolderRepository : RepositoryBase<Folder>, IFolderRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public FolderRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<Folder> FindWithSubFolders(int id) => await _repositoryContext.Folders
            .Include(f => f.Folders)
            .AsNoTracking()
            .SingleOrDefaultAsync(f => f.FolderId == id);
    }
}