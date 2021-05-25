namespace Caliburn.Noesis.Transitions
{
    using System;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;
#endif

    /// <summary>A base class for a <see cref="ITransitionWipe" />. This is an abstract class.</summary>
    /// <seealso cref="MarkupExtension" />
    /// <seealso cref="ITransitionWipe" />
#if !UNITY_5_5_OR_NEWER
    [MarkupExtensionReturnType(typeof(ITransitionWipe))]
#endif
    public abstract class TransitionWipeBase : MarkupExtension, ITransitionWipe
    {
        #region ITransitionWipe Implementation

        /// <inheritdoc />
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <inheritdoc />
        public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(500);

#if UNITY_5_5_OR_NEWER
        /// <inheritdoc />
        public EasingFunctionBase EasingFunction { get; set; } = new SineEase();
#else
        /// <inheritdoc />
        public IEasingFunction EasingFunction { get; set; } = new SineEase();
#endif

        /// <inheritdoc />
        public void Wipe(TransitionerItem fromItem,
                         TransitionerItem toItem,
                         Point origin,
                         IZIndexController zIndexController)
        {
            if (fromItem == null)
            {
                throw new ArgumentNullException(nameof(fromItem));
            }

            if (toItem == null)
            {
                throw new ArgumentNullException(nameof(toItem));
            }

            if (zIndexController == null)
            {
                throw new ArgumentNullException(nameof(zIndexController));
            }

            ConfigureItems(fromItem, toItem, origin);

            fromItem.PerformTransition(false);
            toItem.PerformTransition();

            zIndexController.Stack(toItem, fromItem);
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Called every time the wipe is performed. Override this to configure both items and their
        ///     <see cref="ITransition" /> effects.
        /// </summary>
        /// <param name="fromItem">The content to transition from.</param>
        /// <param name="toItem">To content to transition to.</param>
        /// <param name="origin">The origin point for the wipe transition.</param>
        protected virtual void ConfigureItems(TransitionerItem fromItem,
                                              TransitionerItem toItem,
                                              Point origin)
        {
        }

        #endregion
    }
}