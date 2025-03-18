using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NZWalksAPI.Models.Domain;

public class Image
{
    [Key]
    public Guid Id { get; set; }
    [NotMapped]
    public IFormFile File { get ; set; }
    public string FileName { get; set; }
    public string? FileDescription { get; set; }
    public string FileExtension { get; set; }
    public long FileSizeInByte { get; set; }
    public string FilePath { get; set; }
}