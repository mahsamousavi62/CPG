using CPG.Domain.SharedKernel.File;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Files.Commands.DownloadFile;

public class DownloadFileCommand : IRequest<IFile>
{
    public string FileName { get; set; }

    public DownloadFileCommand(string fileName)
    {
        FileName = fileName;
    }
}
