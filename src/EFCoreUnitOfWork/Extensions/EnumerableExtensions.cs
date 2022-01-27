using System.Collections.Generic;
using System.Linq;

namespace EFCoreUnitOfWork.Extensions
{
	public static class EnumerableExtensions
    { 
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
    }
}
