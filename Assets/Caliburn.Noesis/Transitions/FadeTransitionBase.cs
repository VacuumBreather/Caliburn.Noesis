namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>A fade-in transition effect.</summary>
    /// <seealso cref="TransitionBase" />
    public abstract class FadeTransitionBase : TransitionBase
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FadeTransitionBase" /> class.</summary>
        /// <param name="fadeType">Type of the fade.</param>
        protected FadeTransitionBase(FadeTransitionType fadeType)
        {
            FadeType = fadeType;
        }

        #endregion

        #region Enums

        /// <summary>Specifies the type of a fade transition.</summary>
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
        public override Timeline Build<TSubject>(TSubject effectSubject)
        {
            if (effectSubject == null)
            {
                throw new ArgumentNullException(nameof(effectSubject));
            }

            var from = FadeType == FadeTransitionType.FadeIn ? 0.0 : 1.0;
            var to = FadeType == FadeTransitionType.FadeIn ? 1.0 : 0.0;

            var zeroFrame = new DiscreteDoubleKeyFrame(from);
            var startFrame = new DiscreteDoubleKeyFrame(
                from,
                effectSubject.TransitionDelay + Delay);
            var endFrame =
                new EasingDoubleKeyFrame(to, effectSubject.TransitionDelay + Delay + Duration)
                    {
                        EasingFunction = EasingFunction
                    };

            var timeline = new DoubleAnimationUsingKeyFrames();
            timeline.KeyFrames.Add(zeroFrame);
            timeline.KeyFrames.Add(startFrame);
            timeline.KeyFrames.Add(endFrame);
            timeline.Duration = effectSubject.TransitionDelay + Delay + Duration;
            timeline.Completed += (_, __) => Cancel(effectSubject);

            effectSubject.SetValue(UIElement.OpacityProperty, from);

            Storyboard.SetTarget(timeline, effectSubject);
            Storyboard.SetTargetProperty(timeline, new PropertyPath(UIElement.OpacityProperty));

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
        }

        #endregion
    }
}