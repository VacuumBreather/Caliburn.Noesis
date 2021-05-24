namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;

    /// <summary>
    ///     A <see cref="ITransitionWipe" /> which shrinks and fades out the old content while sliding
    ///     in the new one from the bottom.
    /// </summary>
    /// <seealso cref="TransitionWipeBase" />
    /// <seealso cref="ITransitionWipe" />
    public class SlideOutWipe : TransitionWipeBase, ITransitionWipe
    {
        #region Constants and Fields

        private readonly ITransition shrinkOutTransition = new ShrinkOutTransition();

        private readonly ITransition slideUpTransition =
            new SlideTransition { Direction = SlideDirection.Up };

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

            fromItem.TransitionEffect = this.shrinkOutTransition;
            this.shrinkOutTransition.Duration = Duration;
            this.shrinkOutTransition.Delay = Delay;
            this.shrinkOutTransition.EasingFunction = EasingFunction;

            toItem.TransitionEffect = this.slideUpTransition;
            this.slideUpTransition.Duration = TimeSpan.FromTicks(Duration.Ticks / 2);
            this.slideUpTransition.Delay = Delay + TimeSpan.FromTicks(Duration.Ticks / 2);
            this.slideUpTransition.EasingFunction = EasingFunction;
        }

        #endregion
    }
}