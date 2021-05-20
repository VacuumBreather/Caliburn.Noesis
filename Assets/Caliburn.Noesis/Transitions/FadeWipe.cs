namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    public class FadeWipe : ITransitionWipe
    {
        #region Constants and Fields

        private readonly SineEase _sineEase = new SineEase();

        #endregion

        #region Public Properties

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
            var endKeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(Duration.TotalSeconds / 2));

            // From
            var fromAnimation = new DoubleAnimationUsingKeyFrames();
            fromAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1, zeroKeyTime));
            fromAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, endKeyTime, this._sineEase));

            // To
            var toAnimation = new DoubleAnimationUsingKeyFrames();
            toAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0, zeroKeyTime));
            toAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, endKeyTime, this._sineEase));

            // Preset
            fromItem.Opacity = 1;
            toItem.Opacity = 0;

            // Set up events
            toAnimation.Completed += (sender, args) =>
                {
                    toItem.BeginAnimation(UIElement.OpacityProperty, null);
                    fromItem.Opacity = 0;
                    toItem.Opacity = 1;
                };
            fromAnimation.Completed += (sender, args) =>
                {
                    fromItem.BeginAnimation(UIElement.OpacityProperty, null);
                    fromItem.Opacity = 0;
                    toItem.BeginAnimation(UIElement.OpacityProperty, toAnimation);
                };

            // Animate
            fromItem.BeginAnimation(UIElement.OpacityProperty, fromAnimation);
            zIndexController.Stack(toItem, fromItem);
        }

        #endregion
    }
}