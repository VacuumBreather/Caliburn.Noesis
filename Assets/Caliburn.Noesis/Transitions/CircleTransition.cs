namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Extensions;

    /// <summary>A circle transition effect.</summary>
    /// <seealso cref="TransitionBase" />
    public class CircleTransition : TransitionBase
    {
        #region Constants and Fields

        private const string ScaleTransformName = "__scaleTransform";

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override Timeline Build<TSubject>(TSubject effectSubject)
        {
            if (effectSubject == null)
            {
                throw new ArgumentNullException(nameof(effectSubject));
            }

            var horizontalProportion = Math.Max(1.0 - Origin.X, Origin.X);
            var verticalProportion = Math.Max(1.0 - Origin.Y, Origin.Y);
            var radius = Math.Sqrt(
                Math.Pow(effectSubject.ActualWidth * horizontalProportion, 2) + Math.Pow(
                    effectSubject.ActualHeight * verticalProportion,
                    2));

            var scaleTransform = new ScaleTransform(0, 0);
            var translateTransform = new TranslateTransform(
                effectSubject.ActualWidth * Origin.X,
                effectSubject.ActualHeight * Origin.Y);
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            var ellipseGeometry = new EllipseGeometry
                                      {
                                          RadiusX = radius,
                                          RadiusY = radius,
                                          Transform = transformGroup
                                      };

            effectSubject.GetNameScopeRoot().RegisterName(ScaleTransformName, scaleTransform);

            var zeroFrame = new DiscreteDoubleKeyFrame(0);
            var startFrame = new DiscreteDoubleKeyFrame(0, effectSubject.TransitionDelay + Delay);
            var endFrame =
                new EasingDoubleKeyFrame(1, effectSubject.TransitionDelay + Delay + Duration)
                    {
                        EasingFunction = EasingFunction
                    };

            var scaleXAnimation = new DoubleAnimationUsingKeyFrames();
            scaleXAnimation.KeyFrames.Add(zeroFrame);
            scaleXAnimation.KeyFrames.Add(startFrame);
            scaleXAnimation.KeyFrames.Add(endFrame);

            var scaleYAnimation = new DoubleAnimationUsingKeyFrames();
            scaleYAnimation.KeyFrames.Add(zeroFrame);
            scaleYAnimation.KeyFrames.Add(startFrame);
            scaleYAnimation.KeyFrames.Add(endFrame);

            var timeline = new ParallelTimeline();
            timeline.Children.Add(scaleXAnimation);
            timeline.Children.Add(scaleYAnimation);
            timeline.Completed += (_, __) => Cancel(effectSubject);

            effectSubject.SetCurrentValue(UIElement.ClipProperty, ellipseGeometry);

            Storyboard.SetTargetName(scaleXAnimation, ScaleTransformName);
            Storyboard.SetTargetProperty(
                scaleXAnimation,
                new PropertyPath(ScaleTransform.ScaleXProperty));

            Storyboard.SetTargetName(scaleYAnimation, ScaleTransformName);
            Storyboard.SetTargetProperty(
                scaleYAnimation,
                new PropertyPath(ScaleTransform.ScaleYProperty));

            return timeline;
        }

        /// <inheritdoc />
        public override void Cancel<TSubject>(TSubject effectSubject)
        {
            if (effectSubject == null)
            {
                throw new ArgumentNullException(nameof(effectSubject));
            }

            effectSubject.SetCurrentValue(UIElement.ClipProperty, null);
            effectSubject.GetNameScopeRoot().UnregisterName(ScaleTransformName);
        }

        #endregion
    }
}