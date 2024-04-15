using System.Collections.Generic;

namespace CPG.Infrastructure.Persistence.GraphQL.Model;

public class ReportViewModel<T>
{
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public IEnumerable<T> Models { get; set; }
}
