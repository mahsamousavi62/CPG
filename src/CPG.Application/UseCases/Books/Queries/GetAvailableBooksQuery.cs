using System.Collections.Generic;
using CPG.Application.UseCases.Books.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Books.Queries
{
    public class GetAvailableBooksQuery : IRequest<IReadOnlyCollection<BookViewModel>>
    {
    }
}
