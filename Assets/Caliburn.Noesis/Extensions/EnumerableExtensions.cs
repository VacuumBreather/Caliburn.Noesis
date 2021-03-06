namespace Caliburn.Noesis.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Provides extension methods for the <see cref="IEnumerable{T}" /> type.</summary>
    public static class EnumerableExtensions
    {
        #region Public Methods

        /// <summary>Performs the specified operation on each item in the source sequence.</summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="action">The action to perform on each element.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>Returns every element of the sequence that is not null.</summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source)
            where T : class
        {
            return source.Where(item => item != null);
        }

        #endregion
    }
}