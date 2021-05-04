namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>
    ///     Used to gather the results from multiple child elements which may or may not prevent
    ///     closing.
    /// </summary>
    /// <typeparam name="T">The type of child element.</typeparam>
    public class DefaultCloseStrategy<T> : ICloseStrategy<T>
    {
        #region Constants and Fields

        private readonly bool closeConductedItemsWhenConductorCannotClose;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DefaultCloseStrategy{T}" /> class.</summary>
        /// <param name="closeConductedItemsWhenConductorCannotClose">
        ///     Indicates that even if all conducted
        ///     items are not closable, those that are should be closed. The default is <c>false</c>.
        /// </param>
        public DefaultCloseStrategy(bool closeConductedItemsWhenConductorCannotClose = false)
        {
            this.closeConductedItemsWhenConductorCannotClose =
                closeConductedItemsWhenConductorCannotClose;
        }

        #endregion

        #region ICloseStrategy<T> Implementation

        /// <inheritdoc />
        public async UniTask<ICloseResult<T>> ExecuteAsync(IEnumerable<T> toClose,
                                                           CancellationToken cancellationToken =
                                                               default)
        {
            var closeableChildren = new List<T>();
            var closeCanOccur = true;

            foreach (var child in toClose)
            {
                if (child is IGuardClose guarded)
                {
                    var canClose = await guarded.CanCloseAsync(cancellationToken);

                    if (canClose)
                    {
                        closeableChildren.Add(child);
                    }

                    closeCanOccur = closeCanOccur && canClose;
                }
                else
                {
                    closeableChildren.Add(child);
                }
            }

            if (!this.closeConductedItemsWhenConductorCannotClose && !closeCanOccur)
            {
                closeableChildren.Clear();
            }

            return new CloseResult<T>(closeCanOccur, closeableChildren);
        }

        #endregion
    }
}