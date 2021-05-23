namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using JetBrains.Annotations;

    /// <summary>A directional slide wipe transition.</summary>
    /// <seealso cref="TransitionWipeBase" />
    /// <seealso cref="ITransitionWipe" />
    [PublicAPI]
    public class SlideWipe : TransitionWipeBase
    {
        #region Constants and Fields

        private readonly SlideTransition fromTransition = new SlideTransition { Reverse = true };
        private readonly SlideTransition toTransition = new SlideTransition();

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the direction of the slide wipe transition.</summary>
        /// <value>The direction of the slide wipe transition.</value>
        public SlideDirection Direction { get; set; }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void ConfigureItems(TransitionerItem fromItem,
                                               TransitionerItem toItem,
                                               Point origin)
        {
            fromItem.TransitionEffect = this.fromTransition;
            this.fromTransition.Direction = GetOppositeDirection(Direction);
            this.fromTransition.Duration = Duration;
            this.fromTransition.Delay = Delay;
            this.fromTransition.EasingFunction = EasingFunction;

            toItem.TransitionEffect = this.toTransition;
            this.toTransition.Direction = Direction;
            this.toTransition.Duration = Duration;
            this.toTransition.Delay = Delay;
            this.toTransition.EasingFunction = EasingFunction;
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