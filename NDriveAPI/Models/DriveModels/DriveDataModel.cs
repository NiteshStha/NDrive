using Entities.Models;
using System.Collections.Generic;

namespace NDriveAPI.Models.DriveModels
{
    public class DriveDataModel
    {
        public IEnumerable<Folder> Folders { get; set; }
        public IEnumerable<File> Files { get; set; }
    }
}
