namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;

    /// <seealso cref="TransitionWipeBase" />
    /// <seealso cref="ITransitionWipe" />
    public class SlideOutWipe : TransitionWipeBase, ITransitionWipe
    {
        #region Constants and Fields

        private readonly ITransition fromTransition = new ShrinkOutTransition();

        private readonly ITransition toTransition =
            new SlideTransition { Direction = SlideDirection.Up };

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void ConfigureItems(TransitionerItem fromItem,
                                               TransitionerItem toItem,
                                               Point origin)
        {
            fromItem.TransitionEffect = this.fromTransition;
            this.fromTransition.Duration = Duration;
            this.fromTransition.Delay = Delay;
            this.fromTransition.EasingFunction = EasingFunction;

            toItem.TransitionEffect = this.toTransition;
            this.toTransition.Duration = TimeSpan.FromTicks(Duration.Ticks / 2);
            this.toTransition.Delay = Delay + TimeSpan.FromTicks(Duration.Ticks / 2);
            this.toTransition.EasingFunction = EasingFunction;
        }

        #endregion
    }
}