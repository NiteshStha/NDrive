using System.ComponentModel.DataAnnotations;

namespace NDriveAPI.Models.Folder
{
    public class FolderCreateModel
    {
        [Required] public string FolderName { get; set; }
        public int? ParentFolderId { get; set; }
    }
}