using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcLibrary.Utils.Collections
{
    public interface IPagedList<T> : ICollection<T>, IEnumerable<T>
    {
        int TotalItems { get; }
        int ItemsPerPage { get; }
        int PageNumber { get; }
        int TotalPages { get; }
        int StartIndex { get; }
        int EndIndex { get; }
    }
}