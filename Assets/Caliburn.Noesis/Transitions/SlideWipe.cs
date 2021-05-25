namespace Caliburn.Noesis.Transitions
{
    using System;
    using JetBrains.Annotations;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
#endif

    /// <summary>A directional slide <see cref="ITransitionWipe" />.</summary>
    /// <seealso cref="TransitionWipeBase" />
    /// <seealso cref="ITransitionWipe" />
    [PublicAPI]
    public class SlideWipe : TransitionWipeBase
    {
        #region Constants and Fields

        private readonly SlideTransition slideInTransition = new SlideTransition();

        private readonly SlideTransition
            slideOutTransition = new SlideTransition { Reverse = true };

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the direction of the slide wipe transition, i.e. the direction the new
        ///     content is sliding towards.
        /// </summary>
        /// <value>The direction of the slide wipe transition.</value>
        public SlideDirection Direction { get; set; }

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

            fromItem.TransitionEffect = this.slideOutTransition;
            this.slideOutTransition.Direction = GetOppositeDirection(Direction);
            this.slideOutTransition.Duration = Duration;
            this.slideOutTransition.Delay = Delay;
            this.slideOutTransition.EasingFunction = EasingFunction;

            toItem.TransitionEffect = this.slideInTransition;
            this.slideInTransition.Direction = Direction;
            this.slideInTransition.Duration = Duration;
            this.slideInTransition.Delay = Delay;
            this.slideInTransition.EasingFunction = EasingFunction;
        }

        #endregion

        #region Private Methods

        private static SlideDirection GetOppositeDirection(SlideDirection direction)
        {
            return direction switch
                {
                    SlideDirection.Down => SlideDirection.Up,
                    SlideDirection.Left => SlideDirection.Right,
                    SlideDirection.Up => SlideDirection.Down,
                    SlideDirection.Right => SlideDirection.Left,
                    _ => throw new ArgumentOutOfRangeException(nameof(direction))
                };
        }

        #endregion
    }
}