using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.DirectDebit.Exceptions;

public class NotFoundTrackIdException() : AppException(GlobalResource.NotFoundTrackIdException)
{
    public override string Code => "1010001";
}