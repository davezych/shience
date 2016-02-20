using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Shience
{
    internal static class EnumerableExtensions
    {
        public static void ForEach<T>([NotNull]this IEnumerable<T> enumerable, [NotNull]Action<T> action)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            if (action == null) throw new ArgumentNullException(nameof(action));

            // .ToArray() for thread safety
            foreach (var element in enumerable.ToArray())
            {
                action(element);
            }
        }
    }
}
