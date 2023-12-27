using CPG.Application.UseCases.Ipg.Exceptions;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Ipg.Commands;

public class ValidateTokenCommandHandler(ReadDbContext context) : IRequestHandler<ValidateTokenCommand>
{
    private readonly ReadDbContext _context = context;

    public async Task Handle(ValidateTokenCommand command, CancellationToken cancellationToken)
    {
        var ipgTransaction = await _context.BankReadModels.FirstOrDefaultAsync(t => t.Name == command.ValidateToken.TrackId);

        if (ipgTransaction is null)
            throw new NotFoundTrackIdException();
        if(ipgTransaction.IsActive == false)
            throw new TrackIdInvalidStatusException();
    }
}