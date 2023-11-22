using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CPG.Application.UseCases.Companies.Commands.Create
{
    public class UploadFileViewModel
    {

        [Required]
        public IFormFile? File { get; set; }

        [Required]
        public string? DetailType { get; set; }

        public int? ChunkNumber { get; set; }

        public int? TotalChunks { get; set; }

        public long ChunkSize { get; set; }

        public string FilePath { get; set; }
       
    }
}
