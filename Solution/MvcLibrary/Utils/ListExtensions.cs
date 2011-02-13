using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcLibrary.Utils
{
    public static class ListExtensions
    {
        public static IList<T> TakeColumn<T>(this IEnumerable<T> items, int column, int totalColumns)
        {
            if (items == null) return new List<T>();

            int totalItems = items.Count();

            totalColumns = totalColumns.BullyIntoRange(1, totalItems);
            column = column.BullyIntoRange(1, totalColumns);

            int itemsPerColumn = (int)Math.Ceiling((double)totalItems / (double)totalColumns);

            return items.Skip((column - 1) * itemsPerColumn).Take(itemsPerColumn).ToList();
        }
    }
}