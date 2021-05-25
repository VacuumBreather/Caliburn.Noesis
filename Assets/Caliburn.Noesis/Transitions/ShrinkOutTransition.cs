namespace Caliburn.Noesis.Transitions
{
    using Extensions;
    using System;
    using JetBrains.Annotations;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
#endif

    /// <summary>
    ///     A <see cref="ITransition" /> effect which shrinks the <see cref="ITransitionSubject" />
    ///     and fades it out.
    /// </summary>
    /// <seealso cref="TransitionBase" />
    /// <seealso cref="ITransition" />
    [PublicAPI]
    public class ShrinkOutTransition : TransitionBase
    {
        #region Constants and Fields

        private const float EndOpacity = 0.0f;
        private const float EndScale = 0.8f;

        private const float StartOpacity = 1.0f;
        private const float StartScale = 1.0f;

        private ScaleTransform scaleTransform;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="effectSubject" /> is <c>null</c>.</exception>
        public override Timeline Build<TSubject>(TSubject effectSubject)
        {
            if (effectSubject == null)
            {
                throw new ArgumentNullException(nameof(effectSubject));
            }

            if (!(effectSubject.GetNameScopeRoot().FindName(effectSubject.ScaleTransformName) is
                      ScaleTransform transform))
            {
                return null;
            }

            this.scaleTransform = transform;

            var subjectDelay = GetTotalSubjectDelay(effectSubject);

            var zeroKeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
            var startKeyTime = KeyTime.FromTimeSpan(subjectDelay + Delay);
            var endKeyTime = KeyTime.FromTimeSpan(subjectDelay + Delay + Duration);

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
        /// <exception cref="ArgumentNullException"><paramref name="effectSubject" /> is <c>null</c>.</exception>
        public override void Cancel<TSubject>(TSubject effectSubject)
        {
            if (effectSubject == null)
            {
                throw new ArgumentNullException(nameof(effectSubject));
            }

            effectSubject.Opacity = 1.0f;

            if (this.scaleTransform is { })
            {
                this.scaleTransform.ScaleX = 1.0f;
                this.scaleTransform.ScaleY = 1.0f;
                this.scaleTransform = null;
            }
        }

        #endregion
    }
}