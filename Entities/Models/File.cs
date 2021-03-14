using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class File
    {
        [Key] public int FileId { get; set; }
        [Required] public string FileName { get; set; }
        [Required] public string FileLocation { get; set; }
        [Required] public string FileExtension { get; set; }
        [Required] public double FileSize { get; set; }
        [ForeignKey("Folder")] public int? ParentFolderId { get; set; }

    }
}
