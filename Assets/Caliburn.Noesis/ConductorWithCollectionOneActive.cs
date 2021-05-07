namespace Caliburn.Noesis
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Extensions;

    public partial class Conductor<T>
    {
        #region Nested Types

        /// <summary>An implementation of <see cref="IConductor" /> that holds on many items.</summary>
        public partial class Collection
        {
            #region Nested Types

            /// <summary>
            ///     An implementation of <see cref="IConductor" /> that holds on many items but only activates
            ///     one at a time.
            /// </summary>
            public class OneActive : ConductorBaseWithActiveItem<T>
            {
                #region Constants and Fields

                private readonly BindableCollection<T> items = new BindableCollection<T>();

                #endregion

                #region Constructors and Destructors

                /// <summary>
                ///     Initializes a new instance of the
                ///     <see cref="Caliburn.Noesis.Conductor{T}.Collection.OneActive" /> class.
                /// </summary>
                public OneActive()
                {
                    this.items.CollectionChanged += (s, e) =>
                        {
                            switch (e.Action)
                            {
                                case NotifyCollectionChangedAction.Add:
                                    e.NewItems.OfType<IChild>().ForEach(x => x.Parent = this);

                                    break;
                                case NotifyCollectionChangedAction.Remove:
                                    e.OldItems.OfType<IChild>().ForEach(x => x.Parent = null);

                                    break;
                                case NotifyCollectionChangedAction.Replace:
                                    e.NewItems.OfType<IChild>().ForEach(x => x.Parent = this);
                                    e.OldItems.OfType<IChild>().ForEach(x => x.Parent = null);

                                    break;
                                case NotifyCollectionChangedAction.Reset:
                                    this.items.OfType<IChild>().ForEach(x => x.Parent = this);

                                    break;
                            }
                        };
                }

                #endregion

                #region Public Properties

                /// <summary>Gets the items that are currently being conducted.</summary>
                public IBindableCollection<T> Items => this.items;

                #endregion

                #region Public Methods

                /// <summary>Activates the specified item.</summary>
                /// <param name="item">The item to activate.</param>
                /// <param name="cancellationToken">
                ///     (Optional) A cancellation token that can be used by other objects
                ///     or threads to receive notice of cancellation.
                /// </param>
                /// <returns>A task that represents the asynchronous operation.</returns>
                public override async UniTask ActivateItemAsync(
                    T item,
                    CancellationToken cancellationToken = default)
                {
                    if ((item != null) && item.Equals(ActiveItem))
                    {
                        if (IsActive)
                        {
                            await ScreenExtensions.TryActivateAsync(item, cancellationToken);
                            OnActivationProcessed(item, true);
                        }

                        return;
                    }

                    await ChangeActiveItemAsync(item, false, cancellationToken);
                }

                /// <summary>Called to check whether or not this instance can close.</summary>
                /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
                /// <returns>A task that represents the asynchronous operation.</returns>
                public override async UniTask<bool> CanCloseAsync(
                    CancellationToken cancellationToken = default)
                {
                    var closeResult = await CloseStrategy.ExecuteAsync(
                                          this.items.ToList(),
                                          cancellationToken);

                    if (closeResult.CloseCanOccur || !closeResult.Children.Any())
                    {
                        return closeResult.CloseCanOccur;
                    }

                    var closable = closeResult.Children.ToList();

                    if (closable.Contains(ActiveItem))
                    {
                        var list = this.items.ToList();
                        var next = ActiveItem;

                        do
                        {
                            var previous = next;
                            next = DetermineNextItemToActivate(list, list.IndexOf(previous));
                            list.Remove(previous);
                        }
                        while (closable.Contains(next));

                        var previousActive = ActiveItem;
                        await ChangeActiveItemAsync(next, true, cancellationToken);
                        this.items.Remove(previousActive);
                        closable.Remove(previousActive);
                    }

                    foreach (var deactivate in closable.OfType<IDeactivate>())
                    {
                        await deactivate.DeactivateAsync(true, cancellationToken);
                    }

                    this.items.RemoveRange(closable);

                    return closeResult.CloseCanOccur;
                }

                /// <summary>Deactivates the specified item.</summary>
                /// <param name="item">The item to close.</param>
                /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
                /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
                /// <returns>A task that represents the asynchronous operation.</returns>
                public override async UniTask DeactivateItemAsync(
                    T item,
                    bool close,
                    CancellationToken cancellationToken = default)
                {
                    if (item == null)
                    {
                        return;
                    }

                    if (close)
                    {
                        var closeResult = await CloseStrategy.ExecuteAsync(
                                              new[]
                                                  {
                                                      item
                                                  },
                                              CancellationToken.None);

                        if (closeResult.CloseCanOccur)
                        {
                            await CloseItemCoreAsync(item, cancellationToken);
                        }
                    }
                    else
                    {
                        await ScreenExtensions.TryDeactivateAsync(item, false, cancellationToken);
                    }
                }

                /// <summary>Gets the children.</summary>
                /// <returns>The collection of children.</returns>
                public override IEnumerable<T> GetChildren()
                {
                    return this.items;
                }

                #endregion

                #region Protected Methods

                /// <summary>Determines the next item to activate based on the last active index.</summary>
                /// <param name="list">The list of possible active items.</param>
                /// <param name="lastIndex">The index of the last active item.</param>
                /// <returns>The next item to activate.</returns>
                /// <remarks>Called after an active item is closed.</remarks>
                protected virtual T DetermineNextItemToActivate(IList<T> list, int lastIndex)
                {
                    var toRemoveAt = lastIndex - 1;

                    if ((toRemoveAt == -1) && (list.Count > 1))
                    {
                        return list[1];
                    }

                    if ((toRemoveAt > -1) && (toRemoveAt < list.Count - 1))
                    {
                        return list[toRemoveAt];
                    }

                    return default(T);
                }

                /// <summary>Ensures that an item is ready to be activated.</summary>
                /// <param name="newItem">The item that is about to be activated.</param>
                /// <returns>The item to be activated.</returns>
                protected override T EnsureItem(T newItem)
                {
                    if (newItem == null)
                    {
                        newItem = DetermineNextItemToActivate(
                            this.items,
                            ActiveItem != null ? this.items.IndexOf(ActiveItem) : 0);
                    }
                    else
                    {
                        var index = this.items.IndexOf(newItem);

                        if (index == -1)
                        {
                            this.items.Add(newItem);
                        }
                        else
                        {
                            newItem = this.items[index];
                        }
                    }

                    return base.EnsureItem(newItem);
                }

                /// <summary>Called when activating.</summary>
                /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
                /// <returns>A task that represents the asynchronous operation.</returns>
                protected override UniTask OnActivateAsync(CancellationToken cancellationToken)
                {
                    return ScreenExtensions.TryActivateAsync(ActiveItem, cancellationToken);
                }

                /// <summary>Called when deactivating.</summary>
                /// <param name="close">Indicates whether this instance will be closed.</param>
                /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
                /// <returns>A task that represents the asynchronous operation.</returns>
                protected override async UniTask OnDeactivateAsync(
                    bool close,
                    CancellationToken cancellationToken)
                {
                    if (close)
                    {
                        foreach (var deactivate in this.items.OfType<IDeactivate>())
                        {
                            await deactivate.DeactivateAsync(true, cancellationToken);
                        }

                        this.items.Clear();
                    }
                    else
                    {
                        await ScreenExtensions.TryDeactivateAsync(
                            ActiveItem,
                            false,
                            cancellationToken);
                    }
                }

                #endregion

                #region Private Methods

                private async UniTask CloseItemCoreAsync(T item,
                                                         CancellationToken cancellationToken =
                                                             default)
                {
                    if (item.Equals(ActiveItem))
                    {
                        var index = this.items.IndexOf(item);
                        var next = DetermineNextItemToActivate(this.items, index);

                        await ChangeActiveItemAsync(next, true, cancellationToken);
                    }
                    else
                    {
                        await ScreenExtensions.TryDeactivateAsync(item, true, cancellationToken);
                    }

                    this.items.Remove(item);
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}