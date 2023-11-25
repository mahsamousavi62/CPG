using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.File
{
    public class FileViewModel
    {
        public string FileName { get; set; }
        public Stream Content { get; set; }
        public string ContentType { get; set; }
    }
}
