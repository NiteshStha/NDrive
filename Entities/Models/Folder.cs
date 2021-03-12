using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models
{
    public class Folder
    {
        [Key] public int FolderId { get; set; }

        [Required] public string FolderName { get; set; }

        [ForeignKey("Folder")] public int? ParentFolderId { get; set; }

        [ForeignKey("ParentFolderId")] public ICollection<Folder> Folders { get; set; }
    }
}