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
        /// <summary>Adds the range.</summary>
        /// <param name="items">The items.</param>
        void AddRange(IEnumerable<T> items);

        /// <summary>Removes the range.</summary>
        /// <param name="items">The items.</param>
        void RemoveRange(IEnumerable<T> items);
    }
}