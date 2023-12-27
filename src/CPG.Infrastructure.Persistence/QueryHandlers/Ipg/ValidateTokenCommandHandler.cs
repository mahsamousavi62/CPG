using CPG.Application.UseCases.Ipg.Exceptions;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Ipg.Commands;

public class ValidateTokenCommandHandler(ReadDbContext context) : IRequestHandler<ValidateTokenCommand>
{
    private readonly ReadDbContext _context = context;

    public async Task Handle(ValidateTokenCommand command, CancellationToken cancellationToken)
    {
        var ipgTransaction = await _context.TransactionReadModels.FirstOrDefaultAsync(t => t.IPGTransaction.TrackId == command.ValidateToken.TrackId);

        if (ipgTransaction is null)
            throw new NotFoundTrackIdException();
        if(ipgTransaction.Status != TransactionStatus.InPrgress)
            throw new TrackIdInvalidStatusException();
    }
}