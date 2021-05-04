namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Collections.Specialized;

    #endregion

    /// <summary>Represents a collection that is bindable.</summary>
    /// <typeparam name="T">The type of elements contained in the collection.</typeparam>
    public interface IBindableCollection<T> : IList<T>,
                                              INotifyPropertyChangedEx,
                                              INotifyCollectionChanged
    {
        /// <summary>Adds a range of items to this collection.</summary>
        /// <param name="items">The items to add.</param>
        void AddRange(IEnumerable<T> items);

        /// <summary>Removes a range of items from this collection.</summary>
        /// <param name="items">The items to remove.</param>
        void RemoveRange(IEnumerable<T> items);
    }
}