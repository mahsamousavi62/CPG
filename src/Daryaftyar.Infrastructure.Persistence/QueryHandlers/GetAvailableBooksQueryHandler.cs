using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Daryaftyar.Application.UseCases.Books.Queries;
using Daryaftyar.Application.UseCases.Books.ViewModels;
using Daryaftyar.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Daryaftyar.Infrastructure.Persistence.QueryHandlers
{
    public class GetAvailableBooksQueryHandler : IRequestHandler<GetAvailableBooksQuery, IReadOnlyCollection<BookViewModel>>
    {
        private readonly ReadDbContext _context;

        public GetAvailableBooksQueryHandler(ReadDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<BookViewModel>> Handle(GetAvailableBooksQuery query, CancellationToken cancellationToken)
        {
            var books = await _context.BookReadModels
                .Where(x => x.InStock)
                .Select(x => new BookViewModel
                {
                    Id = x.Id,
                    Author = x.Author,
                    Isbn = x.Isbn,
                    Subject = x.Subject,
                    Title = x.Title,
                    InStock = x.InStock
                })
                .ToListAsync(cancellationToken: cancellationToken);

            return books;
        }
    }
}