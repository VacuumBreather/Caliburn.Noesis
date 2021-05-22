namespace Caliburn.Noesis.Transitions
{
    using System;
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
    public class TransitionControlBase : ContentControl, ITransitionEffectSubject
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

        /// <summary>The OpeningEffectsOffset property.</summary>
        public static readonly DependencyProperty TransitionDelayProperty =
            DependencyProperty.Register(
                nameof(TransitionDelay),
                typeof(TimeSpan),
                typeof(TransitionControlBase),
                new PropertyMetadata(default(TimeSpan)));

        /// <summary>The OpeningEffect property.</summary>
        public static readonly DependencyProperty TransitionEffectProperty =
            DependencyProperty.Register(
                nameof(TransitionEffect),
                typeof(ITransitionEffect),
                typeof(TransitionControlBase),
                new PropertyMetadata(default(ITransitionEffect)));

        /// <summary>The IsTransitionFinished event.</summary>
        public static readonly RoutedEvent TransitionFinished = EventManager.RegisterRoutedEvent(
            nameof(TransitionFinished),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TransitionControlBase));

        private readonly RoutedEventArgs transitionFinishedEventArgs;

        private MatrixTransform matrixTransform;
        private RotateTransform rotateTransform;
        private ScaleTransform scaleTransform;
        private SkewTransform skewTransform;
        private Storyboard storyboard;
        private TranslateTransform translateTransform;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes the <see cref="TransitionControlBase" /> class.</summary>
        static TransitionControlBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitionControlBase),
                new FrameworkPropertyMetadata(typeof(TransitionControlBase)));
        }

        /// <summary>Initializes a new instance of the <see cref="TransitionControlBase" /> class.</summary>
        protected TransitionControlBase()
        {
            this.transitionFinishedEventArgs = new RoutedEventArgs(TransitionFinished, this);
        }

        #endregion

        #region ITransitionEffectSubject Implementation

        /// <inheritdoc />
        public IBindableCollection<ITransitionEffect> AdditionalTransitionEffects { get; } =
            new BindableCollection<ITransitionEffect>();

        /// <inheritdoc />
        string ITransitionEffectSubject.MatrixTransformName => MatrixTransformPartName;

        /// <inheritdoc />
        string ITransitionEffectSubject.RotateTransformName => RotateTransformPartName;

        /// <inheritdoc />
        string ITransitionEffectSubject.ScaleTransformName => ScaleTransformPartName;

        /// <inheritdoc />
        string ITransitionEffectSubject.SkewTransformName => SkewTransformPartName;

        /// <inheritdoc />
        public TimeSpan TransitionDelay
        {
            get => (TimeSpan)GetValue(TransitionDelayProperty);
            set => SetValue(TransitionDelayProperty, value);
        }

        /// <inheritdoc />
        public ITransitionEffect TransitionEffect
        {
            get => (ITransitionEffect)GetValue(TransitionEffectProperty);
            set => SetValue(TransitionEffectProperty, value);
        }

        /// <inheritdoc />
        string ITransitionEffectSubject.TranslateTransformName => TranslateTransformPartName;

        /// <inheritdoc />
        public void CancelTransition()
        {
            this.storyboard?.Stop(GetNameScopeRoot());
        }

        /// <inheritdoc />
        public void PerformTransition()
        {
            if (!IsLoaded || this.matrixTransform is null)
            {
                return;
            }

            CancelTransition();
            this.storyboard = new Storyboard();
            var transitionEffect = TransitionEffect?.Build(this);

            if (transitionEffect != null)
            {
                this.storyboard.Children.Add(transitionEffect);
            }

            foreach (var effect in AdditionalTransitionEffects.Select(effect => effect.Build(this))
                                                              .Where(
                                                                  timeline => !(timeline is null)))
            {
                this.storyboard.Children.Add(effect);
            }

            this.storyboard.Completed += (_, __) => OnTransitionFinished();
            this.storyboard.Begin(GetNameScopeRoot(), true);
        }

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

            PerformTransition();

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

        /// <summary>Raises the <see cref="TransitionFinished" /> event.</summary>
        [PublicAPI]
        protected virtual void OnTransitionFinished()
        {
            RaiseEvent(this.transitionFinishedEventArgs);
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