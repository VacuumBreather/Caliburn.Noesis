namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;

    /// <summary>A wipe transition that takes the shape of a growing circle.</summary>
    /// <seealso cref="TransitionWipeBase" />
    /// <seealso cref="ITransitionWipe" />
    public class CircleWipe : TransitionWipeBase, ITransitionWipe
    {
        #region Constants and Fields

        private readonly ITransition fromTransition = new FadeOutTransition();
        private readonly ITransition toTransition = new CircleTransition();

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void ConfigureItems(TransitionerItem fromItem,
                                               TransitionerItem toItem,
                                               Point origin)
        {
            fromItem.TransitionEffect = this.fromTransition;
            fromItem.TransitionEffect.Duration =
                TimeSpan.FromMilliseconds(Duration.TotalMilliseconds / 2);
            fromItem.TransitionEffect.Delay =
                Delay + TimeSpan.FromMilliseconds(Duration.TotalMilliseconds / 2);
            fromItem.TransitionEffect.EasingFunction = EasingFunction;

            toItem.TransitionEffect = this.toTransition;
            toItem.TransitionEffect.Duration = Duration;
            toItem.TransitionEffect.Delay = Delay;
            toItem.TransitionEffect.EasingFunction = EasingFunction;
            toItem.TransitionEffect.Origin = origin;
        }

        #endregion
    }
}