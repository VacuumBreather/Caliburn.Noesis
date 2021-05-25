namespace Caliburn.Noesis.Transitions
{
    using System;
    using JetBrains.Annotations;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
#endif

    /// <summary>A <see cref="ITransitionWipe" /> which takes the shape of a growing circle.</summary>
    /// <seealso cref="TransitionWipeBase" />
    /// <seealso cref="ITransitionWipe" />
    [PublicAPI]
    public class CircleWipe : TransitionWipeBase
    {
        #region Constants and Fields

        private readonly ITransition circleTransition = new CircleTransition();
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
            fromItem.TransitionEffect.Duration = TimeSpan.FromTicks(Duration.Ticks / 2);
            fromItem.TransitionEffect.Delay = Delay + TimeSpan.FromTicks(Duration.Ticks / 2);
            fromItem.TransitionEffect.EasingFunction = EasingFunction;

            toItem.TransitionEffect = this.circleTransition;
            toItem.TransitionEffect.Duration = Duration;
            toItem.TransitionEffect.Delay = Delay;
            toItem.TransitionEffect.EasingFunction = EasingFunction;
            toItem.TransitionEffect.Origin = origin;
        }

        #endregion
    }
}