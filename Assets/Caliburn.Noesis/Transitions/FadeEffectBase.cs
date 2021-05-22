namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>A fade-in transition effect.</summary>
    /// <seealso cref="TransitionEffectBase{FadeInEffect}" />
    public abstract class FadeEffectBase<TFadeEffect> : TransitionEffectBase<TFadeEffect>
        where TFadeEffect : FadeEffectBase<TFadeEffect>, ITransitionEffect, new()
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FadeEffectBase{TFadeEffect}" /> class.</summary>
        /// <param name="fadeType">Type of the fade.</param>
        protected FadeEffectBase(FadeEffectType fadeType)
        {
            FadeType = fadeType;
        }

        #endregion

        #region Enums

        /// <summary>Defines the different types of fade effect.</summary>
        protected enum FadeEffectType
        {
            /// <summary>A fade-in effect.</summary>
            FadeIn,

            /// <summary>A fade-out effect.</summary>
            FadeOut
        }

        #endregion

        #region Private Properties

        private FadeEffectType FadeType { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override Timeline Build<TSubject>(TSubject effectSubject)
        {
            if (effectSubject == null)
            {
                throw new ArgumentNullException(nameof(effectSubject));
            }

            var from = FadeType == FadeEffectType.FadeIn ? 0.0 : 1.0;
            var to = FadeType == FadeEffectType.FadeIn ? 1.0 : 0.0;

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
            timeline.Completed += (_, __) => effectSubject.SetValue(UIElement.OpacityProperty, to);

            effectSubject.SetValue(UIElement.OpacityProperty, from);

            Storyboard.SetTarget(timeline, effectSubject);
            Storyboard.SetTargetProperty(timeline, new PropertyPath(UIElement.OpacityProperty));

            return timeline;
        }

        #endregion
    }
}