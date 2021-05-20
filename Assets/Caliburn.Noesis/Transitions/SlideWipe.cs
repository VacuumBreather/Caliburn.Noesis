namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public class SlideWipe : ITransitionWipe
    {
        #region Constants and Fields

        private readonly SineEase _sineEase = new SineEase();

        #endregion

        #region Public Properties

        /// <summary>Direction of the slide wipe</summary>
        public SlideDirection Direction { get; set; }

        /// <summary>Duration of the animation</summary>
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.5);

        #endregion

        #region ITransitionWipe Implementation

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
            double fromStartX = 0, fromEndX = 0, toStartX = 0, toEndX = 0;
            double fromStartY = 0, fromEndY = 0, toStartY = 0, toEndY = 0;

            if (Direction == SlideDirection.Left)
            {
                fromEndX = -fromItem.ActualWidth;
                toStartX = toItem.ActualWidth;
            }
            else if (Direction == SlideDirection.Right)
            {
                fromEndX = fromItem.ActualWidth;
                toStartX = -toItem.ActualWidth;
            }
            else if (Direction == SlideDirection.Up)
            {
                fromEndY = -fromItem.ActualHeight;
                toStartY = toItem.ActualHeight;
            }
            else if (Direction == SlideDirection.Down)
            {
                fromEndY = fromItem.ActualHeight;
                toStartY = -toItem.ActualHeight;
            }

            // From
            var fromTransform = new TranslateTransform(fromStartX, fromStartY);
            fromItem.RenderTransform = fromTransform;
            var fromXAnimation = new DoubleAnimationUsingKeyFrames();
            fromXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(fromStartX, zeroKeyTime));
            fromXAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(fromEndX, endKeyTime, this._sineEase));
            var fromYAnimation = new DoubleAnimationUsingKeyFrames();
            fromYAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(fromStartY, zeroKeyTime));
            fromYAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(fromEndY, endKeyTime, this._sineEase));

            // To
            var toTransform = new TranslateTransform(toStartX, toStartY);
            toItem.RenderTransform = toTransform;
            var toXAnimation = new DoubleAnimationUsingKeyFrames();
            toXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(toStartX, zeroKeyTime));
            toXAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(toEndX, endKeyTime, this._sineEase));
            var toYAnimation = new DoubleAnimationUsingKeyFrames();
            toYAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(toStartY, zeroKeyTime));
            toYAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(toEndY, endKeyTime, this._sineEase));

            // Set up events
            fromXAnimation.Completed += (sender, args) =>
                {
                    fromTransform.BeginAnimation(TranslateTransform.XProperty, null);
                    fromTransform.X = fromEndX;
                    fromItem.RenderTransform = null;
                };
            fromYAnimation.Completed += (sender, args) =>
                {
                    fromTransform.BeginAnimation(TranslateTransform.YProperty, null);
                    fromTransform.Y = fromEndY;
                    fromItem.RenderTransform = null;
                };
            toXAnimation.Completed += (sender, args) =>
                {
                    toTransform.BeginAnimation(TranslateTransform.XProperty, null);
                    toTransform.X = toEndX;
                    toItem.RenderTransform = null;
                };
            toYAnimation.Completed += (sender, args) =>
                {
                    toTransform.BeginAnimation(TranslateTransform.YProperty, null);
                    toTransform.Y = toEndY;
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