using System.Collections.Generic;
using System.Collections.Specialized;

namespace Caliburn.Noesis
{
    /// <summary>
    /// Represents a collection that is observable.
    /// </summary>
    /// <typeparam name = "T">The type of elements contained in the collection.</typeparam>
    public interface IObservableCollection<T> : IList<T>, INotifyPropertyChangedEx, INotifyCollectionChanged
    {
        /// <summary>
        ///   Adds a range of items.
        /// </summary>
        /// <param name = "items">The items.</param>
        void AddRange(IEnumerable<T> items);

        /// <summary>
        ///   Removes a range of items.
        /// </summary>
        /// <param name = "items">The items.</param>
        void RemoveRange(IEnumerable<T> items);
    }
}
