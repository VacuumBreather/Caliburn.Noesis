namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>
    ///     A base class for fade-in and fade-out <see cref="ITransition" /> effects. This is an
    ///     abstract class.
    /// </summary>
    /// <seealso cref="TransitionBase" />
    /// <seealso cref="ITransition" />
    public abstract class FadeTransitionBase : TransitionBase
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FadeTransitionBase" /> class.</summary>
        /// <param name="fadeType">Type of the fade (in or out).</param>
        protected FadeTransitionBase(FadeTransitionType fadeType)
        {
            FadeType = fadeType;
        }

        #endregion

        #region Enums

        /// <summary>Represents the type of a fade transition.</summary>
        protected enum FadeTransitionType
        {
            /// <summary>The transition is fading in.</summary>
            FadeIn,

            /// <summary>The transition is fading out.</summary>
            FadeOut
        }

        #endregion

        #region Private Properties

        private FadeTransitionType FadeType { get; }

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

            var startOpacity = FadeType == FadeTransitionType.FadeIn ? 0.0 : 1.0;
            var endOpacity = FadeType == FadeTransitionType.FadeIn ? 1.0 : 0.0;

            var subjectDelay = GetTotalSubjectDelay(effectSubject);

            var zeroFrame = new DiscreteDoubleKeyFrame(startOpacity, TimeSpan.Zero);
            var startFrame = new DiscreteDoubleKeyFrame(startOpacity, subjectDelay + Delay);
            var endFrame =
                new EasingDoubleKeyFrame(endOpacity, subjectDelay + Delay + Duration)
                    {
                        EasingFunction = EasingFunction
                    };

            var timeline = new DoubleAnimationUsingKeyFrames();
            timeline.KeyFrames.Add(zeroFrame);
            timeline.KeyFrames.Add(startFrame);
            timeline.KeyFrames.Add(endFrame);
            timeline.Duration = effectSubject.TransitionDelay + Delay + Duration;
            timeline.Completed += (_, __) => Cancel(effectSubject);

            effectSubject.Opacity = startOpacity;

            Storyboard.SetTarget(timeline, effectSubject);
            Storyboard.SetTargetProperty(timeline, new PropertyPath(UIElement.OpacityProperty));

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

            effectSubject.Opacity = 1.0;
        }

        #endregion
    }
}