namespace Caliburn.Noesis.Extensions
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>Provides extension methods for the <see cref="IBindableCollection{T}" /> type.</summary>
    public static class BindableCollectionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Assigns the a <see cref="IParent" /> items being added to the collection and sets it to
        ///     <c>null</c> from removed items.
        /// </summary>
        /// <param name="children">The collection of child items.</param>
        /// <param name="parent">The parent.</param>
        public static void AreChildrenOf<T>(this IBindableCollection<T> children, IParent parent)
            where T : class
        {
            children.CollectionChanged += (s, e) =>
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            SetParent(e.NewItems, parent);

                            break;
                        case NotifyCollectionChangedAction.Remove:
                            SetParent(e.OldItems, null);

                            break;
                        case NotifyCollectionChangedAction.Replace:
                            SetParent(e.OldItems, null);
                            SetParent(e.NewItems, parent);

                            break;
                        case NotifyCollectionChangedAction.Reset:
                            SetParent((IBindableCollection<T>)s, parent);

                            break;
                    }
                };
        }

        #endregion

        #region Private Methods

        private static void SetParent(IEnumerable children, object parent)
        {
            children.OfType<IChild>().ForEach(child => child.Parent = parent);
        }

        #endregion
    }
}