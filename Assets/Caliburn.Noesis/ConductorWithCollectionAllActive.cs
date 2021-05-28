namespace Caliburn.Noesis
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
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
            ///     An implementation of <see cref="IConductor" /> that holds on to many items which are all
            ///     activated.
            /// </summary>
            [PublicAPI]
            public class AllActive : ConductorBase<T>
            {
                #region Constants and Fields

                private readonly bool conductPublicItems;

                private readonly BindableCollection<T> items = new BindableCollection<T>();

                #endregion

                #region Constructors and Destructors

                /// <summary>
                ///     Initializes a new instance of the
                ///     <see cref="Caliburn.Noesis.Conductor{T}.Collection.AllActive" /> class.
                /// </summary>
                /// <param name="conductPublicItems">
                ///     If set to <c>true</c> public items that are properties of this
                ///     class will be conducted.
                /// </param>
                public AllActive(bool conductPublicItems)
                    : this()
                {
                    this.conductPublicItems = conductPublicItems;
                }

                /// <summary>
                ///     Initializes a new instance of the
                ///     <see cref="Caliburn.Noesis.Conductor{T}.Collection.AllActive" /> class.
                /// </summary>
                public AllActive()
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
                /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
                /// <returns>A task that represents the asynchronous operation.</returns>
                public override async UniTask ActivateItemAsync(
                    T item,
                    CancellationToken cancellationToken = default)
                {
                    if (item == null)
                    {
                        return;
                    }

                    item = EnsureItem(item);

                    if (IsActive)
                    {
                        await ScreenExtensions.TryActivateAsync(item, cancellationToken);
                    }

                    OnActivationProcessed(item, true);
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

                    if (!closeResult.CloseCanOccur && closeResult.Children.Any())
                    {
                        foreach (var deactivate in closeResult.Children.OfType<IDeactivate>())
                        {
                            await deactivate.DeactivateAsync(true, cancellationToken);
                        }

                        this.items.RemoveRange(closeResult.Children);
                    }

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

                /// <summary>Ensures that an item is ready to be activated.</summary>
                /// <param name="newItem">The item that is about to be activated.</param>
                /// <returns>The item to be activated.</returns>
                protected override T EnsureItem(T newItem)
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

                    return base.EnsureItem(newItem);
                }

                /// <summary>Called when activating.</summary>
                protected override async UniTask OnActivateAsync(
                    CancellationToken cancellationToken)
                {
                    foreach (var activate in this.items.OfType<IActivate>())
                    {
                        await activate.ActivateAsync(cancellationToken);
                    }
                }

                /// <summary>Called when deactivating.</summary>
                /// <param name="close">Indicates whether this instance will be closed.</param>
                /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
                /// <returns>A task that represents the asynchronous operation.</returns>
                protected override async UniTask OnDeactivateAsync(
                    bool close,
                    CancellationToken cancellationToken)
                {
                    foreach (var deactivate in this.items.OfType<IDeactivate>())
                    {
                        await deactivate.DeactivateAsync(close, cancellationToken);
                    }

                    if (close)
                    {
                        this.items.Clear();
                    }
                }

                /// <summary>Called when initializing.</summary>
                protected override async UniTask OnInitializeAsync(
                    CancellationToken cancellationToken)
                {
                    if (this.conductPublicItems)
                    {
                        var publicItems = GetType()
                                          .GetTypeInfo()
                                          .DeclaredProperties
                                          .Where(
                                              propertyInfo =>
                                                  (propertyInfo.Name != nameof(Parent)) &&
                                                  typeof(T).GetTypeInfo()
                                                           .IsAssignableFrom(
                                                               propertyInfo.PropertyType
                                                                   .GetTypeInfo()))
                                          .Select(propertyInfo => propertyInfo.GetValue(this, null))
                                          .Cast<T>()
                                          .ToList();

                        foreach (var item in publicItems)
                        {
                            Logger.Trace($"{ToString()} detected and conducts public item {item}");
                        }

                        await UniTask.WhenAll(
                            publicItems.Select(item => ActivateItemAsync(item, cancellationToken)));
                    }
                }

                #endregion

                #region Private Methods

                private async UniTask CloseItemCoreAsync(T item,
                                                         CancellationToken cancellationToken =
                                                             default)
                {
                    await ScreenExtensions.TryDeactivateAsync(item, true, cancellationToken);

                    this.items.Remove(item);
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}