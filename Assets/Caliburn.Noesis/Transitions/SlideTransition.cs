namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Extensions;
    using JetBrains.Annotations;

    /// <summary>
    ///     A <see cref="ITransition" /> effect which slides content from outside its containing area
    ///     in the specified direction into its resting position.
    /// </summary>
    /// <seealso cref="TransitionBase" />
    /// <seealso cref="ITransition" />
    [PublicAPI]
    public class SlideTransition : TransitionBase
    {
        #region Constants and Fields

        private double endX;
        private double endY;
        private double startX;
        private double startY;
        private TranslateTransform translateTransform;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the container element which defines the outer edges at which the
        ///     slide transition should start.
        /// </summary>
        /// <remarks>
        ///     If this is not specified, the bounding box of the <see cref="ITransitionSubject" /> itself
        ///     is used.
        /// </remarks>
        /// <value>
        ///     The name of the container element which defines the outer edges at which the slide
        ///     transition should start.
        /// </value>
        public string ContainerElementName { get; set; }

        /// <summary>Gets or sets the direction of the transition.</summary>
        /// <value>The direction of the transition.</value>
        public SlideDirection Direction { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="SlideTransition" /> should be
        ///     reversed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="SlideTransition" /> is reversed, i.e. the content is moved
        ///     out of the frame instead; otherwise, <c>false</c>.
        /// </value>
        public bool Reverse { get; set; }

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

            if (!(effectSubject.GetNameScopeRoot().FindName(effectSubject.TranslateTransformName) is
                      TranslateTransform transform))
            {
                return null;
            }

            this.translateTransform = transform;

            var container = string.IsNullOrEmpty(ContainerElementName)
                                ? effectSubject
                                : effectSubject.FindName(ContainerElementName) as
                                      FrameworkElement ?? effectSubject;

            // Set up coordinates
            this.endX = 0;
            this.endY = 0;
            this.startX = 0;
            this.startY = 0;

            switch (Direction)
            {
                case SlideDirection.Left:
                    this.startX = container.ActualWidth;

                    break;
                case SlideDirection.Right:
                    this.startX = -container.ActualWidth;

                    break;
                case SlideDirection.Up:
                    this.startY = container.ActualHeight;

                    break;
                case SlideDirection.Down:
                    this.startY = -container.ActualHeight;

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

            var subjectDelay = GetTotalSubjectDelay(effectSubject);

            var zeroKeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
            var startKeyTime = KeyTime.FromTimeSpan(subjectDelay + Delay);
            var endKeyTime = KeyTime.FromTimeSpan(subjectDelay + Delay + Duration);

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
        /// <exception cref="ArgumentNullException"><paramref name="effectSubject" /> is <c>null</c>.</exception>
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