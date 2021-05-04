namespace Caliburn.Noesis.Extensions
{
    #region Using Directives

    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>
    ///     Provides extension methods for the <see cref="IScreen" /> and <see cref="IConductor" />
    ///     types.
    /// </summary>
    public static class ScreenExtensions
    {
        #region Public Methods

        /// <summary>Activates this instance.</summary>
        /// <param name="activate">The instance to activate</param>
        /// <param name="cancellationToken">(Optional) The cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask ActivateAsync(this IActivate activate,
                                            CancellationToken cancellationToken = default)
        {
            return activate.ActivateAsync(cancellationToken);
        }

        /// <summary>Activates a child whenever the specified parent is activated.</summary>
        /// <param name="child">The child to activate.</param>
        /// <param name="parent">The parent whose activation triggers the child's activation.</param>
        public static void ActivateWith(this IActivate child, IActivate parent)
        {
            var childReference = new WeakReference(child);

            void OnParentActivated(object sender, ActivationEventArgs e)
            {
                var activate = (IActivate)childReference.Target;

                if (activate == null)
                {
                    ((IActivate)sender).Activated -= OnParentActivated;
                }
                else
                {
                    activate.ActivateAsync(CancellationToken.None);
                }
            }

            parent.Activated += OnParentActivated;
        }

        /// <summary>
        ///     Activates and Deactivates a child whenever the specified parent is Activated or
        ///     Deactivated.
        /// </summary>
        /// <param name="child">The child to activate/deactivate.</param>
        /// <param name="parent">
        ///     The parent whose activation/deactivation triggers the child's
        ///     activation/deactivation.
        /// </param>
        public static void ConductWith<TChild, TParent>(this TChild child, TParent parent)
            where TChild : IActivate, IDeactivate where TParent : IActivate, IDeactivate
        {
            child.ActivateWith(parent);
            child.DeactivateWith(parent);
        }

        /// <summary>Deactivates this instance.</summary>
        /// <param name="deactivate">The instance to deactivate</param>
        /// <param name="close">Indicates whether or not this instance is being closed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask DeactivateAsync(this IDeactivate deactivate, bool close)
        {
            return deactivate.DeactivateAsync(close);
        }

        /// <summary>Deactivates a child whenever the specified parent is deactivated.</summary>
        /// <param name="child">The child to deactivate.</param>
        /// <param name="parent">The parent whose deactivation triggers the child's deactivation.</param>
        public static void DeactivateWith(this IDeactivate child, IDeactivate parent)
        {
            var childReference = new WeakReference(child);

            async UniTask AsyncEventHandler(object s, DeactivationEventArgs e)
            {
                var deactivate = (IDeactivate)childReference.Target;

                if (deactivate == null)
                {
                    ((IDeactivate)s).Deactivated -= AsyncEventHandler;
                }
                else
                {
                    await deactivate.DeactivateAsync(e.WasClosed, CancellationToken.None);
                }
            }

            parent.Deactivated += AsyncEventHandler;
        }

        /// <summary>Activates the item if it implements <see cref="IActivate" />, otherwise does nothing.</summary>
        /// <param name="potentialActivate">The potential activate.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask TryActivateAsync(object potentialActivate,
                                               CancellationToken cancellationToken = default)
        {
            return potentialActivate is IActivate activator
                       ? activator.ActivateAsync(cancellationToken)
                       : UniTask.FromResult(true);
        }

        /// <summary>Deactivates the item if it implements <see cref="IDeactivate" />, otherwise does nothing.</summary>
        /// <param name="potentialDeactivate">The potential deactivate.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask TryDeactivateAsync(object potentialDeactivate,
                                                 bool close,
                                                 CancellationToken cancellationToken = default)
        {
            return potentialDeactivate is IDeactivate deactivate
                       ? deactivate.DeactivateAsync(close, cancellationToken)
                       : UniTask.FromResult(true);
        }

        #endregion
    }
}