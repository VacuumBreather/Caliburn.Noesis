namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public class CircleWipe : ITransitionWipe
    {
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

            var horizontalProportion = Math.Max(1.0 - origin.X, 1.0 * origin.X);
            var verticalProportion = Math.Max(1.0 - origin.Y, 1.0 * origin.Y);
            var radius = Math.Sqrt(
                Math.Pow(toItem.ActualWidth * horizontalProportion, 2) + Math.Pow(
                    toItem.ActualHeight * verticalProportion,
                    2));

            var scaleTransform = new ScaleTransform(0, 0);
            var translateTransform = new TranslateTransform(
                toItem.ActualWidth * origin.X,
                toItem.ActualHeight * origin.Y);
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            var ellipseGeomotry = new EllipseGeometry
                                      {
                                          RadiusX = radius,
                                          RadiusY = radius,
                                          Transform = transformGroup
                                      };

            toItem.SetCurrentValue(UIElement.ClipProperty, ellipseGeomotry);
            zIndexController.Stack(toItem, fromItem);

            var zeroKeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
            var midKeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200));
            var endKeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400));

            var opacityAnimation = new DoubleAnimationUsingKeyFrames();
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, zeroKeyTime));
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, midKeyTime));
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, endKeyTime));
            opacityAnimation.Completed += (sender, args) =>
                {
                    fromItem.BeginAnimation(UIElement.OpacityProperty, null);
                    fromItem.Opacity = 0;
                };
            fromItem.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

            var scaleAnimation = new DoubleAnimationUsingKeyFrames();
            scaleAnimation.Completed += (sender, args) =>
                {
                    toItem.SetCurrentValue(UIElement.ClipProperty, null);
                    fromItem.BeginAnimation(UIElement.OpacityProperty, null);
                    fromItem.Opacity = 0;
                };
            scaleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, zeroKeyTime));
            scaleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, endKeyTime));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }

        #endregion
    }
}