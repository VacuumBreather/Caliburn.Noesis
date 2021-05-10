namespace Caliburn.Noesis.Samples.Conductors.Views
{
    using System;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
#endif

    public class SolidColorBrushAnimation : AnimationTimeline
    {
        #region Constants and Fields

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(
            "From",
            typeof(Brush),
            typeof(SolidColorBrushAnimation));

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
            "To",
            typeof(Brush),
            typeof(SolidColorBrushAnimation));

        private SolidColorBrush BlendedBrush = new SolidColorBrush();

        #endregion

        #region Public Properties

        //we must define From and To, AnimationTimeline does not have this properties
        public SolidColorBrush From
        {
            get => (SolidColorBrush)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }
#if !UNITY_5_5_OR_NEWER
        public override Type TargetPropertyType => typeof(SolidColorBrush);
#endif

        public SolidColorBrush To
        {
            get => (SolidColorBrush)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        #endregion

#if !UNITY_5_5_OR_NEWER
        #region Public Methods

        public override object GetCurrentValue(object defaultOriginValue,
                                               object defaultDestinationValue,
                                               AnimationClock animationClock)
        {
            return GetCurrentValue(
                defaultOriginValue as SolidColorBrush,
                defaultDestinationValue as SolidColorBrush,
                animationClock);
        }

        #endregion

        #region Protected Methods

        protected override Freezable CreateInstanceCore()
        {
            return new SolidColorBrushAnimation();
        }

        #endregion

        #region Private Methods

        private static Color Blend(Color from, Color to, double amount)
        {
            var a = (byte)((from.A * amount) + (to.A * (1 - amount)));
            var r = (byte)((from.R * amount) + (to.R * (1 - amount)));
            var g = (byte)((from.G * amount) + (to.G * (1 - amount)));
            var b = (byte)((from.B * amount) + (to.B * (1 - amount)));

            return Color.FromArgb(a, r, g, b);
        }

        private object GetCurrentValue(SolidColorBrush defaultOriginValue,
                                       SolidColorBrush defaultDestinationValue,
                                       AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
            {
                return Brushes.Transparent;
            }

            //use the standard values if From and To are not set 
            //(it is the value of the given property)
            defaultOriginValue = From ?? defaultOriginValue;
            defaultDestinationValue = To ?? defaultDestinationValue;

            if (animationClock.CurrentProgress.Value <= 0)
            {
                return defaultOriginValue;
            }

            if (animationClock.CurrentProgress.Value >= 1)
            {
                return defaultDestinationValue;
            }

            this.BlendedBrush.Color = Blend(
                defaultOriginValue.Color,
                defaultDestinationValue.Color,
                animationClock.CurrentProgress.Value);

            return this.BlendedBrush;
        }
        #endregion
#endif
    }
}