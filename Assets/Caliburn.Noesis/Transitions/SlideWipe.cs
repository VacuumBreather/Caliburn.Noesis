namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using JetBrains.Annotations;

    [PublicAPI]
    public class SlideWipe : ITransitionWipe
    {
        #region Constants and Fields

        private readonly SineEase sineEase = new SineEase();

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the direction of the slide wipe transition.</summary>
        /// <value>The direction of the slide wipe transition.</value>
        public SlideDirection Direction { get; set; }

        /// <summary>Gets or sets the duration of the wipe transition.</summary>
        /// <value>The duration of the wipe transition.</value>
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.5);

        #endregion

        #region ITransitionWipe Implementation

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

            // Set up time points
            var zeroKeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
            var endKeyTime = KeyTime.FromTimeSpan(Duration);

            // Set up coordinates
            const double FromStartX = 0;
            const double ToEndX = 0;
            const double FromStartY = 0;
            const double ToEndY = 0;

            double fromEndX = 0, toStartX = 0;
            double fromEndY = 0, toStartY = 0;

            switch (Direction)
            {
                case SlideDirection.Left:
                    fromEndX = -fromItem.ActualWidth;
                    toStartX = toItem.ActualWidth;

                    break;
                case SlideDirection.Right:
                    fromEndX = fromItem.ActualWidth;
                    toStartX = -toItem.ActualWidth;

                    break;
                case SlideDirection.Up:
                    fromEndY = -fromItem.ActualHeight;
                    toStartY = toItem.ActualHeight;

                    break;
                case SlideDirection.Down:
                    fromEndY = fromItem.ActualHeight;
                    toStartY = -toItem.ActualHeight;

                    break;
            }

            // From
            var fromTransform = new TranslateTransform(FromStartX, FromStartY);
            fromItem.RenderTransform = fromTransform;

            var fromXAnimation = new DoubleAnimationUsingKeyFrames();
            fromXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(FromStartX, zeroKeyTime));
            fromXAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(fromEndX, endKeyTime, this.sineEase));

            var fromYAnimation = new DoubleAnimationUsingKeyFrames();
            fromYAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(FromStartY, zeroKeyTime));
            fromYAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(fromEndY, endKeyTime, this.sineEase));

            // To
            var toTransform = new TranslateTransform(toStartX, toStartY);
            toItem.RenderTransform = toTransform;

            var toXAnimation = new DoubleAnimationUsingKeyFrames();
            toXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(toStartX, zeroKeyTime));
            toXAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(ToEndX, endKeyTime, this.sineEase));

            var toYAnimation = new DoubleAnimationUsingKeyFrames();
            toYAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(toStartY, zeroKeyTime));
            toYAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(ToEndY, endKeyTime, this.sineEase));

            // Set up events
            fromXAnimation.Completed += (_, __) =>
                {
                    fromTransform.BeginAnimation(TranslateTransform.XProperty, null);
                    fromTransform.X = fromEndX;
                    fromItem.RenderTransform = null;
                };

            fromYAnimation.Completed += (_, __) =>
                {
                    fromTransform.BeginAnimation(TranslateTransform.YProperty, null);
                    fromTransform.Y = fromEndY;
                    fromItem.RenderTransform = null;
                };

            toXAnimation.Completed += (_, __) =>
                {
                    toTransform.BeginAnimation(TranslateTransform.XProperty, null);
                    toTransform.X = ToEndX;
                    toItem.RenderTransform = null;
                };

            toYAnimation.Completed += (_, __) =>
                {
                    toTransform.BeginAnimation(TranslateTransform.YProperty, null);
                    toTransform.Y = ToEndY;
                    toItem.RenderTransform = null;
                };

            // Animate
            fromTransform.BeginAnimation(TranslateTransform.XProperty, fromXAnimation);
            fromTransform.BeginAnimation(TranslateTransform.YProperty, fromYAnimation);

            toTransform.BeginAnimation(TranslateTransform.XProperty, toXAnimation);
            toTransform.BeginAnimation(TranslateTransform.YProperty, toYAnimation);

            zIndexController.Stack(toItem, fromItem);
        }

        #endregion
    }
}