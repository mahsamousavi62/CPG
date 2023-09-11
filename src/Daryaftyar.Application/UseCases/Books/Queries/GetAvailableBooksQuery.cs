using System.Collections.Generic;
using Daryaftyar.Application.UseCases.Books.ViewModels;
using MediatR;

namespace Daryaftyar.Application.UseCases.Books.Queries
{
    public class GetAvailableBooksQuery : IRequest<IReadOnlyCollection<BookViewModel>>
    {
    }
}
