namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Linq;
    using Extensions;
    using JetBrains.Annotations;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
#endif

    /// <summary>
    ///     A base class for a <see cref="ContentControl" /> supporting <see cref="ITransition" />
    ///     effects. This is an abstract class.
    /// </summary>
    /// <seealso cref="ContentControl" />
    /// <seealso cref="ITransitionSubject" />
    [PublicAPI]
#if !UNITY_5_5_OR_NEWER
    [TemplatePart(Name = MatrixTransformPartName, Type = typeof(MatrixTransform))]
    [TemplatePart(Name = RotateTransformPartName, Type = typeof(RotateTransform))]
    [TemplatePart(Name = ScaleTransformPartName, Type = typeof(ScaleTransform))]
    [TemplatePart(Name = SkewTransformPartName, Type = typeof(SkewTransform))]
    [TemplatePart(Name = TranslateTransformPartName, Type = typeof(TranslateTransform))]
#endif
    public abstract class TransitionSubjectBase : ContentControl, ITransitionSubject
    {
        #region Constants and Fields

        /// <summary>The name of the matrix transform template part.</summary>
        public const string MatrixTransformPartName = "PART_MatrixTransform";

        /// <summary>The name of the rotate transform template part.</summary>
        public const string RotateTransformPartName = "PART_RotateTransform";

        /// <summary>The name of the scale transform template part.</summary>
        public const string ScaleTransformPartName = "PART_ScaleTransform";

        /// <summary>The name of the skew transform template part.</summary>
        public const string SkewTransformPartName = "PART_SkewTransform";

        /// <summary>The name of the translate transform template part.</summary>
        public const string TranslateTransformPartName = "PART_TranslateTransform";

        /// <summary>The CascadingDelay property.</summary>
        public static readonly DependencyProperty CascadingDelayProperty =
            DependencyProperty.RegisterAttached(
                PropertyNameHelper.GetName(nameof(CascadingDelayProperty)),
                typeof(TimeSpan),
                typeof(TransitioningContentControl),
                new FrameworkPropertyMetadata(
                    TimeSpan.Zero,
                    FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>The OpeningEffectsOffset property.</summary>
        public static readonly DependencyProperty TransitionDelayProperty =
            DependencyProperty.Register(
                nameof(TransitionDelay),
                typeof(TimeSpan),
                typeof(TransitionSubjectBase),
                new PropertyMetadata(default(TimeSpan)));

        /// <summary>The OpeningEffect property.</summary>
        public static readonly DependencyProperty TransitionEffectProperty =
            DependencyProperty.Register(
                nameof(TransitionEffect),
                typeof(ITransition),
                typeof(TransitionSubjectBase),
                new PropertyMetadata(default(ITransition)));

        /// <summary>The IsTransitionFinished event.</summary>
        public static readonly RoutedEvent TransitionFinished = EventManager.RegisterRoutedEvent(
            nameof(TransitionFinished),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TransitionSubjectBase));

        private readonly RoutedEventArgs transitionFinishedEventArgs;

        private MatrixTransform matrixTransform;
        private RotateTransform rotateTransform;
        private ScaleTransform scaleTransform;
        private SkewTransform skewTransform;
        private Storyboard storyboard;
        private TranslateTransform translateTransform;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes the <see cref="TransitionSubjectBase" /> class.</summary>
        static TransitionSubjectBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitionSubjectBase),
                new FrameworkPropertyMetadata(typeof(TransitionSubjectBase)));
        }

        /// <summary>Initializes a new instance of the <see cref="TransitionSubjectBase" /> class.</summary>
        protected TransitionSubjectBase()
        {
            this.transitionFinishedEventArgs = new RoutedEventArgs(TransitionFinished, this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the cascading delay between <see cref="ITransitionSubject" /> elements inside
        ///     an <see cref="ItemsControl" />.
        /// </summary>
        /// <value>
        ///     The cascading delay between <see cref="ITransitionSubject" /> elements inside an
        ///     <see cref="ItemsControl" />.
        /// </value>
        public TimeSpan CascadingDelay
        {
            get => (TimeSpan)GetValue(CascadingDelayProperty);
            set => SetValue(CascadingDelayProperty, value);
        }

        #endregion

        #region ITransitionSubject Implementation

        /// <inheritdoc />
        public IBindableCollection<ITransition> AdditionalTransitionEffects { get; } =
            new BindableCollection<ITransition>();

        /// <inheritdoc />
        string ITransitionSubject.MatrixTransformName => MatrixTransformPartName;

        /// <inheritdoc />
        string ITransitionSubject.RotateTransformName => RotateTransformPartName;

        /// <inheritdoc />
        string ITransitionSubject.ScaleTransformName => ScaleTransformPartName;

        /// <inheritdoc />
        string ITransitionSubject.SkewTransformName => SkewTransformPartName;

        /// <inheritdoc />
        public TimeSpan TransitionDelay
        {
            get => (TimeSpan)GetValue(TransitionDelayProperty);
            set => SetValue(TransitionDelayProperty, value);
        }

        /// <inheritdoc />
        public ITransition TransitionEffect
        {
            get => (ITransition)GetValue(TransitionEffectProperty);
            set => SetValue(TransitionEffectProperty, value);
        }

        /// <inheritdoc />
        string ITransitionSubject.TranslateTransformName => TranslateTransformPartName;

        /// <inheritdoc />
        public void CancelTransition()
        {
            this.storyboard?.Stop(this.GetNameScopeRoot());
            this.storyboard = null;
            TransitionEffect?.Cancel(this);

            foreach (var effect in AdditionalTransitionEffects)
            {
                effect?.Cancel(this);
            }
        }

        /// <inheritdoc />
        public void PerformTransition(bool includeAdditionalEffects = true)
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

            if (includeAdditionalEffects)
            {
                foreach (var effect in AdditionalTransitionEffects
                                       .Select(effect => effect.Build(this))
                                       .Where(timeline => !(timeline is null)))
                {
                    this.storyboard.Children.Add(effect);
                }
            }

            this.storyboard.Completed += (_, __) => OnTransitionFinished();
            this.storyboard.Begin(this.GetNameScopeRoot(), true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the cascading delay between <see cref="ITransitionSubject" /> elements inside an
        ///     <see cref="ItemsControl" />.
        /// </summary>
        /// <param name="element">The element from which to read the property value.</param>
        /// <returns>The value of the CascadingDelay attached property.</returns>
        public static TimeSpan GetCascadingDelay(DependencyObject element)
        {
            return (TimeSpan)element.GetValue(CascadingDelayProperty);
        }

        /// <summary>
        ///     Sets the cascading delay between <see cref="ITransitionSubject" /> elements inside an
        ///     <see cref="ItemsControl" />.
        /// </summary>
        /// <param name="element">The element on which to set the attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetCascadingDelay(DependencyObject element, TimeSpan value)
        {
            element.SetValue(CascadingDelayProperty, value);
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            var nameScopeRoot = this.GetNameScopeRoot();

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

        /// <summary>Called when the transition has finished.</summary>
        protected virtual void OnTransitionFinished()
        {
            RaiseEvent(this.transitionFinishedEventArgs);
        }

        #endregion
    }
}