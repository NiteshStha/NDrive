using System.ComponentModel.DataAnnotations;

namespace NDriveAPI.Models.FolderCreateModel
{
    public class FolderCreateModel
    {
        [Required] public string FolderName { get; set; }
        public int? ParentFolderId { get; set; }
    }
}