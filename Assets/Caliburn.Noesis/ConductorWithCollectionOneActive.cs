namespace Caliburn.Noesis
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;

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
            [PublicAPI]
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
                    this.AssignParentOnCollectionChanged(this.items);
                }

                #endregion

                #region Public Properties

                /// <summary>Gets the items that are currently being conducted.</summary>
                public IBindableCollection<T> Items => this.items;

                #endregion

                #region Public Methods

                /// <inheritdoc />
                public override async UniTask ActivateItemAsync(
                    T item,
                    CancellationToken cancellationToken = default)
                {
                    using var _ = Logger.GetMethodTracer(item, cancellationToken);

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

                /// <inheritdoc />
                public override async UniTask<bool> CanCloseAsync(
                    CancellationToken cancellationToken = default)
                {
                    using var _ = Logger.GetMethodTracer(cancellationToken);

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

                /// <inheritdoc />
                public override async UniTask DeactivateItemAsync(
                    T item,
                    bool close,
                    CancellationToken cancellationToken = default)
                {
                    using var _ = Logger.GetMethodTracer(item, close, cancellationToken);

                    await this.DeactivateItemAsync(
                        item,
                        close,
                        CloseItemCoreAsync,
                        cancellationToken);
                }

                /// <inheritdoc />
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
                    using var _ = Logger.GetMethodTracer(list, lastIndex);

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

                /// <inheritdoc />
                protected override T EnsureItem(T newItem)
                {
                    using var _ = Logger.GetMethodTracer(newItem);

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

                /// <inheritdoc />
                protected override UniTask OnActivateAsync(CancellationToken cancellationToken)
                {
                    using var _ = Logger.GetMethodTracer(cancellationToken);

                    return ScreenExtensions.TryActivateAsync(ActiveItem, cancellationToken);
                }

                /// <inheritdoc />
                protected override async UniTask OnDeactivateAsync(
                    bool close,
                    CancellationToken cancellationToken)
                {
                    using var _ = Logger.GetMethodTracer(close, cancellationToken);

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
                    using var _ = Logger.GetMethodTracer(item, cancellationToken);

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