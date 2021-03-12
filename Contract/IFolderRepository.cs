using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contract
{
    public interface IFolderRepository : IRepositoryBase<Folder>
    {
        Task<Folder> FindWithSubFolders(int id);
    }
}