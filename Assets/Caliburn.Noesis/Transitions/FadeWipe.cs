namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using JetBrains.Annotations;

    /// <seealso cref="TransitionWipeBase" />
    /// <seealso cref="ITransitionWipe" />
    [PublicAPI]
    public class FadeWipe : TransitionWipeBase
    {
        #region Constants and Fields

        private readonly ITransition fadeInTransition = new FadeInTransition();
        private readonly ITransition fadeOutTransition = new FadeOutTransition();

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="fromItem" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="toItem" /> is <c>null</c>.</exception>
        protected override void ConfigureItems(TransitionerItem fromItem,
                                               TransitionerItem toItem,
                                               Point origin)
        {
            if (fromItem == null)
            {
                throw new ArgumentNullException(nameof(fromItem));
            }

            if (toItem == null)
            {
                throw new ArgumentNullException(nameof(toItem));
            }

            fromItem.TransitionEffect = this.fadeOutTransition;
            fromItem.TransitionEffect.Duration = Duration;
            fromItem.TransitionEffect.Delay = Delay;
            fromItem.TransitionEffect.EasingFunction = EasingFunction;

            toItem.TransitionEffect = this.fadeInTransition;
            toItem.TransitionEffect.Duration = Duration;
            toItem.TransitionEffect.Delay = Delay;
            toItem.TransitionEffect.EasingFunction = EasingFunction;
        }

        #endregion
    }
}