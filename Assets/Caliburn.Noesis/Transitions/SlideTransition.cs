namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Extensions;

    /// <summary>A transition effect which slides the subject in a specified direction.</summary>
    /// <seealso cref="TransitionBase" />
    /// <seealso cref="ITransition" />
    public class SlideTransition : TransitionBase, ITransition
    {
        #region Constants and Fields

        private double endX;
        private double endY;
        private double startX;
        private double startY;
        private TranslateTransform translateTransform;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the direction of the transition.</summary>
        /// <value>The direction of the transition.</value>
        public SlideDirection Direction { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="SlideTransition" /> should be
        ///     reversed.
        /// </summary>
        /// <value><c>true</c> if this <see cref="SlideTransition" /> is reversed; otherwise, <c>false</c>.</value>
        public bool Reverse { get; set; }

        #endregion

        #region ITransition Implementation

        /// <inheritdoc />
        public override Timeline Build<TSubject>(TSubject effectSubject)
        {
            if (!(effectSubject.GetNameScopeRoot().FindName(effectSubject.TranslateTransformName) is
                      TranslateTransform transform))
            {
                return null;
            }

            this.translateTransform = transform;

            // Set up coordinates
            this.endX = 0;
            this.endY = 0;
            this.startX = 0;
            this.startY = 0;

            switch (Direction)
            {
                case SlideDirection.Left:
                    this.startX = effectSubject.ActualWidth;

                    break;
                case SlideDirection.Right:
                    this.startX = -effectSubject.ActualWidth;

                    break;
                case SlideDirection.Up:
                    this.startY = effectSubject.ActualHeight;

                    break;
                case SlideDirection.Down:
                    this.startY = -effectSubject.ActualHeight;

                    break;
            }

            if (Reverse)
            {
                var tempEndX = this.endX;
                var tempEndY = this.endY;

                this.endX = this.startX;
                this.endY = this.startY;
                this.startX = tempEndX;
                this.startY = tempEndY;
            }

            var zeroKeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
            var startKeyTime = KeyTime.FromTimeSpan(effectSubject.TransitionDelay + Delay);
            var endKeyTime = KeyTime.FromTimeSpan(effectSubject.TransitionDelay + Delay + Duration);

            var xAnimation = new DoubleAnimationUsingKeyFrames();
            xAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(this.startX, zeroKeyTime));
            xAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(this.startX, startKeyTime));
            xAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(this.endX, endKeyTime, EasingFunction));

            var yAnimation = new DoubleAnimationUsingKeyFrames();
            yAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(this.startY, zeroKeyTime));
            yAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(this.startY, startKeyTime));
            yAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(this.endY, endKeyTime, EasingFunction));

            var timeline = new ParallelTimeline();
            timeline.Children.Add(xAnimation);
            timeline.Children.Add(yAnimation);
            timeline.Completed += (_, __) => Cancel(effectSubject);

            this.translateTransform.X = this.startX;
            this.translateTransform.Y = this.startY;

            Storyboard.SetTargetName(xAnimation, effectSubject.TranslateTransformName);
            Storyboard.SetTargetProperty(
                xAnimation,
                new PropertyPath(TranslateTransform.XProperty));

            Storyboard.SetTargetName(yAnimation, effectSubject.TranslateTransformName);
            Storyboard.SetTargetProperty(
                yAnimation,
                new PropertyPath(TranslateTransform.YProperty));

            return timeline;
        }

        /// <inheritdoc />
        public override void Cancel<TSubject>(TSubject effectSubject)
        {
            if (effectSubject == null)
            {
                throw new ArgumentNullException(nameof(effectSubject));
            }

            if (this.translateTransform is null)
            {
                return;
            }

            this.translateTransform.X = this.endX;
            this.translateTransform.Y = this.endY;
            this.translateTransform = null;
        }

        #endregion
    }
}