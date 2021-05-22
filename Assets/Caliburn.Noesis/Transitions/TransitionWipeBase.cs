namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;

    /// <summary>Base class for a wipe transition.</summary>
    /// <seealso cref="MarkupExtension" />
    /// <seealso cref="ITransitionWipe" />
    public abstract class TransitionWipeBase<TWipe> : MarkupExtension, ITransitionWipe
        where TWipe : ITransitionWipe, new()
    {
        #region Protected Properties

        /// <summary>Gets or sets the ease function.</summary>
        /// <value>The ease function.</value>
        protected SineEase EasingFunction { get; set; } = new SineEase();

        #endregion

        #region ITransitionWipe Implementation

        /// <inheritdoc />
        public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(500);

        /// <inheritdoc />
        public abstract void Wipe(TransitionerItem fromItem,
                                  TransitionerItem toItem,
                                  Point origin,
                                  IZIndexController zIndexController);

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new TWipe();
        }

        #endregion
    }
}