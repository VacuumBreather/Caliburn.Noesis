namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    /// <seealso cref="TransitionWipeBase{TWipe}" />
    /// <seealso cref="ITransitionWipe" />
    public class SlideOutWipe : TransitionWipeBase<SlideOutWipe>, ITransitionWipe
    {
        #region ITransitionWipe Implementation

        /// <inheritdoc />
        public override void Wipe(TransitionerItem fromItem,
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

            var zeroKeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
            var halfTimeKeyTime = KeyTime.FromTimeSpan(
                TimeSpan.FromMilliseconds(Duration.TotalMilliseconds / 2.0));
            var endKeyTime = KeyTime.FromTimeSpan(Duration);

            //back out old slide setup
            var scaleTransform = new ScaleTransform(1, 1);
            fromItem.RenderTransform = scaleTransform;
            var scaleAnimation = new DoubleAnimationUsingKeyFrames();
            scaleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, zeroKeyTime));
            scaleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(.8, endKeyTime));
            scaleAnimation.Completed += (sender, args) => { fromItem.RenderTransform = null; };
            var opacityAnimation = new DoubleAnimationUsingKeyFrames();
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, zeroKeyTime));
            opacityAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, endKeyTime));
            opacityAnimation.Completed += (sender, args) =>
                {
                    fromItem.BeginAnimation(UIElement.OpacityProperty, null);
                    fromItem.Opacity = 0;
                };

            //slide in new slide setup
            var translateTransform = new TranslateTransform(0, toItem.ActualHeight);
            toItem.RenderTransform = translateTransform;
            var slideAnimation = new DoubleAnimationUsingKeyFrames();
            slideAnimation.KeyFrames.Add(
                new LinearDoubleKeyFrame(toItem.ActualHeight, zeroKeyTime));
            slideAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(toItem.ActualHeight, halfTimeKeyTime)
                    {
                        EasingFunction = EasingFunction
                    });
            slideAnimation.KeyFrames.Add(
                new EasingDoubleKeyFrame(0, endKeyTime) { EasingFunction = EasingFunction });

            //kick off!
            translateTransform.BeginAnimation(TranslateTransform.YProperty, slideAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
            fromItem.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);

            zIndexController.Stack(toItem, fromItem);
        }

        #endregion
    }
}