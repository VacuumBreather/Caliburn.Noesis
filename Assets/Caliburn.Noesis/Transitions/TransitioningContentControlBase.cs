namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using JetBrains.Annotations;

    /// <summary>Base class for a content control supporting transition animations.</summary>
    /// <seealso cref="ContentControl" />
    /// <seealso cref="ITransitionEffectSubject" />
    [TemplatePart(Name = MatrixTransformPartName, Type = typeof(MatrixTransform))]
    [TemplatePart(Name = RotateTransformPartName, Type = typeof(RotateTransform))]
    [TemplatePart(Name = ScaleTransformPartName, Type = typeof(ScaleTransform))]
    [TemplatePart(Name = SkewTransformPartName, Type = typeof(SkewTransform))]
    [TemplatePart(Name = TranslateTransformPartName, Type = typeof(TranslateTransform))]
    public class TransitioningContentControlBase : ContentControl, ITransitionEffectSubject
    {
        #region Constants and Fields

        /// <summary>The name of the matrix transform template part.</summary>
        [UsedImplicitly]
        public const string MatrixTransformPartName = "PART_MatrixTransform";

        /// <summary>The name of the rotate transform template part.</summary>
        [UsedImplicitly]
        public const string RotateTransformPartName = "PART_RotateTransform";

        /// <summary>The name of the scale transform template part.</summary>
        [UsedImplicitly]
        public const string ScaleTransformPartName = "PART_ScaleTransform";

        /// <summary>The name of the skew transform template part.</summary>
        [UsedImplicitly]
        public const string SkewTransformPartName = "PART_SkewTransform";

        /// <summary>The name of the translate transform template part.</summary>
        [UsedImplicitly]
        public const string TranslateTransformPartName = "PART_TranslateTransform";

        /// <summary>The IsTransitionFinished event.</summary>
        public static readonly RoutedEvent IsTransitionFinished = EventManager.RegisterRoutedEvent(
            nameof(IsTransitionFinished),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TransitioningContentControlBase));

        /// <summary>The OpeningEffectsOffset property.</summary>
        public static readonly DependencyProperty OpeningEffectsOffsetProperty =
            DependencyProperty.Register(
                nameof(OpeningEffectsOffset),
                typeof(TimeSpan),
                typeof(TransitioningContentControlBase),
                new PropertyMetadata(default(TimeSpan)));

        /// <summary>The OpeningEffect property.</summary>
        public static readonly DependencyProperty TransitionEffectProperty =
            DependencyProperty.Register(
                nameof(TransitionEffect),
                typeof(TransitionEffectBase),
                typeof(TransitioningContentControlBase),
                new PropertyMetadata(default(TransitionEffectBase)));

        private readonly RoutedEventArgs isTransitionFinishedArgs;

        private MatrixTransform matrixTransform;
        private RotateTransform rotateTransform;
        private ScaleTransform scaleTransform;
        private SkewTransform skewTransform;
        private TranslateTransform translateTransform;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes the <see cref="TransitioningContentControlBase" /> class.</summary>
        static TransitioningContentControlBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitioningContentControlBase),
                new FrameworkPropertyMetadata(typeof(TransitioningContentControlBase)));
        }

        /// <summary>Initializes a new instance of the <see cref="TransitioningContentControlBase" /> class.</summary>
        protected TransitioningContentControlBase()
        {
            this.isTransitionFinishedArgs = new RoutedEventArgs(IsTransitionFinished, this);
        }

        #endregion

        #region Public Properties

        /// <summary>Delay offset to be applied to all opening effect transitions.</summary>
        public TimeSpan OpeningEffectsOffset
        {
            get => (TimeSpan)GetValue(OpeningEffectsOffsetProperty);
            set => SetValue(OpeningEffectsOffsetProperty, value);
        }

        /// <summary>Gets or sets the transition to run when the content is loaded and made visible.</summary>
        [TypeConverter(typeof(TransitionEffectTypeConverter))]
        public TransitionEffectBase TransitionEffect
        {
            get => (TransitionEffectBase)GetValue(TransitionEffectProperty);
            set => SetValue(TransitionEffectProperty, value);
        }

        /// <summary>
        ///     Allows multiple transition effects to be combined and run upon the content loading or
        ///     being made visible.
        /// </summary>
        [UsedImplicitly]
        public ObservableCollection<TransitionEffectBase> TransitionEffects { get; } =
            new ObservableCollection<TransitionEffectBase>();

        #endregion

        #region ITransitionEffectSubject Implementation

        /// <inheritdoc />
        string ITransitionEffectSubject.MatrixTransformName => MatrixTransformPartName;

        /// <inheritdoc />
        TimeSpan ITransitionEffectSubject.Offset => OpeningEffectsOffset;

        /// <inheritdoc />
        string ITransitionEffectSubject.RotateTransformName => RotateTransformPartName;

        /// <inheritdoc />
        string ITransitionEffectSubject.ScaleTransformName => ScaleTransformPartName;

        /// <inheritdoc />
        string ITransitionEffectSubject.SkewTransformName => SkewTransformPartName;

        /// <inheritdoc />
        string ITransitionEffectSubject.TranslateTransformName => TranslateTransformPartName;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            var nameScopeRoot = GetNameScopeRoot();

            this.matrixTransform = GetTemplateChild(MatrixTransformPartName) as MatrixTransform;
            this.rotateTransform = GetTemplateChild(RotateTransformPartName) as RotateTransform;
            this.scaleTransform = GetTemplateChild(ScaleTransformPartName) as ScaleTransform;
            this.skewTransform = GetTemplateChild(SkewTransformPartName) as SkewTransform;
            this.translateTransform =
                GetTemplateChild(TranslateTransformPartName) as TranslateTransform;

            UnregisterNames(
                MatrixTransformPartName,
                RotateTransformPartName,
                ScaleTransformPartName,
                SkewTransformPartName,
                TranslateTransformPartName);

            if (this.matrixTransform != null)
            {
                nameScopeRoot.RegisterName(MatrixTransformPartName, this.matrixTransform);
            }

            if (this.rotateTransform != null)
            {
                nameScopeRoot.RegisterName(RotateTransformPartName, this.rotateTransform);
            }

            if (this.scaleTransform != null)
            {
                nameScopeRoot.RegisterName(ScaleTransformPartName, this.scaleTransform);
            }

            if (this.skewTransform != null)
            {
                nameScopeRoot.RegisterName(SkewTransformPartName, this.skewTransform);
            }

            if (this.translateTransform != null)
            {
                nameScopeRoot.RegisterName(TranslateTransformPartName, this.translateTransform);
            }

            base.OnApplyTemplate();

            RunTransitionEffects();

            void UnregisterNames(params string[] names)
            {
                foreach (var name in names.Where(name => FindName(name) != null))
                {
                    UnregisterName(name);
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>Raises the <see cref="IsTransitionFinished" /> event.</summary>
        [PublicAPI]
        protected virtual void OnIsTransitionFinished()
        {
            RaiseEvent(this.isTransitionFinishedArgs);
        }

        /// <summary>Runs the transition effects.</summary>
        protected void RunTransitionEffects()
        {
            if (!IsLoaded || this.matrixTransform is null)
            {
                return;
            }

            var storyboard = new Storyboard();
            var transitionEffect = TransitionEffect?.Build(this);

            if (transitionEffect != null)
            {
                storyboard.Children.Add(transitionEffect);
            }

            foreach (var effect in TransitionEffects.Select(effect => effect.Build(this))
                                                    .Where(timeline => !(timeline is null)))
            {
                storyboard.Children.Add(effect);
            }

            storyboard.Completed += (_, __) => OnIsTransitionFinished();
            storyboard.Begin(GetNameScopeRoot());
        }

        #endregion

        #region Private Methods

        private FrameworkElement GetNameScopeRoot()
        {
            // Only set the namescope if the child does not already have a template XAML namescope set.
            if ((VisualChildrenCount > 0) &&
                GetVisualChild(0) is FrameworkElement frameworkElement &&
                (NameScope.GetNameScope(frameworkElement) != null))
            {
                return frameworkElement;
            }

            if (NameScope.GetNameScope(this) is null)
            {
                NameScope.SetNameScope(this, new NameScope());
            }

            return this;
        }

        #endregion
    }
}