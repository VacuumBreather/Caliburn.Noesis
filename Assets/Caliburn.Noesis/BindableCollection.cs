namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>A base collection class that supports automatic UI thread marshalling.</summary>
    /// <typeparam name="T">The type of elements contained in the collection.</typeparam>
    public class BindableCollection<T> : ObservableCollection<T>,
                                         IBindableCollection<T>,
                                         IReadOnlyBindableCollection<T>
    {
        #region Constants and Fields

        private int suspensionCount;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BindableCollection{T}" /> class.</summary>
        public BindableCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BindableCollection{T}" /> class that contains
        ///     elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="collection" /> parameter cannot be
        ///     null.
        /// </exception>
        public BindableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        #endregion

        #region IBindableCollection<T> Implementation

        /// <inheritdoc />
        public virtual void AddRange(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            CheckReentrancy();

            void AddRangeLocal()
            {
                using (var _ = SuspendNotifications())
                {
                    var index = Count;

                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var item in items)
                    {
                        InsertItemBase(index, item);
                        index++;
                    }
                }

                OnCollectionRefreshed();
            }

            Execute.OnUIThread(AddRangeLocal);
        }

        /// <inheritdoc />
        public virtual void RemoveRange(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            CheckReentrancy();

            void RemoveRangeLocal()
            {
                using (var _ = SuspendNotifications())
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var item in items)
                    {
                        var index = IndexOf(item);

                        if (index >= 0)
                        {
                            RemoveItemBase(index);
                        }
                    }
                }

                OnCollectionRefreshed();
            }

            Execute.OnUIThread(RemoveRangeLocal);
        }

        #endregion

        #region IBindableObject Implementation

        /// <summary>
        ///     Raises a property and collection changed event that notifies that all of the properties on
        ///     this object have changed.
        /// </summary>
        public void Refresh()
        {
            CheckReentrancy();
            Execute.OnUIThread(OnCollectionRefreshed);
        }

        /// <summary>Suspends the change notifications.</summary>
        /// <returns>A guard resuming the notifications when it goes out of scope.</returns>
        /// <remarks>Use the guard in a using statement.</remarks>
        public IDisposable SuspendNotifications()
        {
            this.suspensionCount++;

            return new DisposableAction(ResumeNotifications);
        }

        #endregion

        #region Protected Methods

        /// <summary>Exposes the base implementation of the <see cref="ClearItems" /> function.</summary>
        /// <remarks>Used to avoid compiler warning regarding unverifiable code.</remarks>
        protected virtual void ClearItemsBase()
        {
            base.ClearItems();
        }

        /// <summary>Exposes the base implementation of the <see cref="InsertItem" /> function.</summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <remarks>Used to avoid compiler warning regarding unverifiable code.</remarks>
        protected virtual void InsertItemBase(int index, T item)
        {
            base.InsertItem(index, item);
        }

        /// <summary>
        ///     Raises a property and collection changed event that notifies that all of the properties on
        ///     this object have changed.
        /// </summary>
        protected virtual void OnCollectionRefreshed()
        {
            if (AreNotificationsSuspended())
            {
                return;
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>Exposes the base implementation of the <see cref="RemoveItem" /> function.</summary>
        /// <param name="index">The index.</param>
        /// <remarks>Used to avoid compiler warning regarding unverifiable code.</remarks>
        protected virtual void RemoveItemBase(int index)
        {
            base.RemoveItem(index);
        }

        /// <summary>Exposes the base implementation of the <see cref="SetItem" /> function.</summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <remarks>Used to avoid compiler warning regarding unverifiable code.</remarks>
        protected virtual void SetItemBase(int index, T item)
        {
            base.SetItem(index, item);
        }

        /// <summary>Clears the items contained by the collection.</summary>
        protected override sealed void ClearItems()
        {
            CheckReentrancy();
            Execute.OnUIThread(ClearItemsBase);
        }

        /// <summary>Inserts the item to the specified position.</summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The item to be inserted.</param>
        protected override sealed void InsertItem(int index, T item)
        {
            Execute.OnUIThread(() => InsertItemBase(index, item));
        }

        /// <summary>
        ///     Raises the
        ///     <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged" /> event
        ///     with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (AreNotificationsSuspended())
            {
                return;
            }

            base.OnCollectionChanged(e);
        }

        /// <summary>Raises the PropertyChanged event with the provided arguments.</summary>
        /// <param name="e">The event data to report in the event.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (AreNotificationsSuspended())
            {
                return;
            }

            base.OnPropertyChanged(e);
        }

        /// <summary>Removes the item at the specified position.</summary>
        /// <param name="index">The position used to identify the item to remove.</param>
        protected override sealed void RemoveItem(int index)
        {
            Execute.OnUIThread(() => RemoveItemBase(index));
        }

        /// <summary>Sets the item at the specified position.</summary>
        /// <param name="index">The index to set the item at.</param>
        /// <param name="item">The item to set.</param>
        protected override sealed void SetItem(int index, T item)
        {
            Execute.OnUIThread(() => SetItemBase(index, item));
        }

        /// <summary>Determines whether notifications are suspended.</summary>
        /// <returns></returns>
        protected bool AreNotificationsSuspended()
        {
            return this.suspensionCount > 0;
        }

        #endregion

        #region Private Methods

        private void ResumeNotifications()
        {
            this.suspensionCount--;
        }

        #endregion
    }
}