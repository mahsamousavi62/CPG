using System.ComponentModel.DataAnnotations;


namespace CPG.Application.UseCases.Companies.Commands.Create;

public class UploadFileViewModel
{

   // [Required]
   // public IFormFile? File { get; set; }

    [Required]
    public string? DetailType { get; set; }

    public int? ChunkNumber { get; set; }

    public int? TotalChunks { get; set; }

    public long ChunkSize { get; set; }

    public string FilePath { get; set; }
   
}
