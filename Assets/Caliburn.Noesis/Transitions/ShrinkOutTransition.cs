namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Extensions;
    using JetBrains.Annotations;

    /// <summary>An exit transition which shrinks the <see cref="ITransitionSubject" />.</summary>
    /// <seealso cref="TransitionBase" />
    [PublicAPI]
    public class ShrinkOutTransition : TransitionBase
    {
        #region Constants and Fields

        private const double EndOpacity = 0.0;
        private const double EndScale = 0.8;

        private const double StartOpacity = 1.0;
        private const double StartScale = 1.0;

        private ScaleTransform scaleTransform;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override Timeline Build<TSubject>(TSubject effectSubject)
        {
            if (!(effectSubject.GetNameScopeRoot().FindName(effectSubject.ScaleTransformName) is
                      ScaleTransform transform))
            {
                return null;
            }

            this.scaleTransform = transform;

            var zeroKeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
            var startKeyTime = KeyTime.FromTimeSpan(effectSubject.TransitionDelay + Delay);
            var endKeyTime = KeyTime.FromTimeSpan(effectSubject.TransitionDelay + Delay + Duration);

            var scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            scaleXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(StartScale, zeroKeyTime));
            scaleXAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(StartScale, startKeyTime));
            scaleXAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(EndScale, endKeyTime, EasingFunction));

            var scaleYAnimation = new DoubleAnimationUsingKeyFrames();
            scaleYAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(StartScale, zeroKeyTime));
            scaleYAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(StartScale, startKeyTime));
            scaleYAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(EndScale, endKeyTime, EasingFunction));

            var opacityAnimation = new DoubleAnimationUsingKeyFrames();
            opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(StartOpacity, zeroKeyTime));
            opacityAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(StartOpacity, startKeyTime));
            opacityAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(EndOpacity, endKeyTime, EasingFunction));

            var timeline = new ParallelTimeline();
            timeline.Children.Add(scaleXAnimation);
            timeline.Children.Add(scaleYAnimation);
            timeline.Children.Add(opacityAnimation);
            timeline.Completed += (_, __) => Cancel(effectSubject);

            this.scaleTransform.ScaleX = StartScale;
            this.scaleTransform.ScaleY = StartScale;

            Storyboard.SetTargetName(scaleXAnimation, effectSubject.ScaleTransformName);
            Storyboard.SetTargetProperty(
                scaleXAnimation,
                new PropertyPath(ScaleTransform.ScaleXProperty));

            Storyboard.SetTargetName(scaleYAnimation, effectSubject.ScaleTransformName);
            Storyboard.SetTargetProperty(
                scaleYAnimation,
                new PropertyPath(ScaleTransform.ScaleYProperty));

            Storyboard.SetTarget(opacityAnimation, effectSubject);
            Storyboard.SetTargetProperty(
                opacityAnimation,
                new PropertyPath(UIElement.OpacityProperty));

            return timeline;
        }

        /// <inheritdoc />
        public override void Cancel<TSubject>(TSubject effectSubject)
        {
            if (effectSubject == null)
            {
                throw new ArgumentNullException(nameof(effectSubject));
            }

            effectSubject.Opacity = 1.0;

            if (this.scaleTransform is { })
            {
                this.scaleTransform.ScaleX = 1.0;
                this.scaleTransform.ScaleY = 1.0;
                this.scaleTransform = null;
            }
        }

        #endregion
    }
}