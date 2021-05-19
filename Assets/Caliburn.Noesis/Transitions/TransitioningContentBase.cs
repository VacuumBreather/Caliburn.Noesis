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

    public class TransitioningContentBase : ContentControl, ITransitionEffectSubject
    {
        #region Constants and Fields

        public const string MatrixTransformPartName = "PART_MatrixTransform";
        public const string RotateTransformPartName = "PART_RotateTransform";
        public const string ScaleTransformPartName = "PART_ScaleTransform";
        public const string SkewTransformPartName = "PART_SkewTransform";
        public const string TranslateTransformPartName = "PART_TranslateTransform";

        public static readonly DependencyProperty OpeningEffectProperty =
            DependencyProperty.Register(
                "OpeningEffect",
                typeof(TransitionEffectBase),
                typeof(TransitioningContentBase),
                new PropertyMetadata(default(TransitionEffectBase)));

        public static readonly DependencyProperty OpeningEffectsOffsetProperty =
            DependencyProperty.Register(
                "OpeningEffectsOffset",
                typeof(TimeSpan),
                typeof(TransitioningContentBase),
                new PropertyMetadata(default(TimeSpan)));

        private MatrixTransform? _matrixTransform;
        private RotateTransform? _rotateTransform;
        private ScaleTransform? _scaleTransform;
        private SkewTransform? _skewTransform;
        private TranslateTransform? _translateTransform;

        #endregion

        #region Constructors and Destructors

        static TransitioningContentBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitioningContentBase),
                new FrameworkPropertyMetadata(typeof(TransitioningContentBase)));
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the transition to run when the content is loaded and made visible.</summary>
        [TypeConverter(typeof(TransitionEffectTypeConverter))]
        public TransitionEffectBase? OpeningEffect
        {
            get => (TransitionEffectBase?)GetValue(OpeningEffectProperty);
            set => SetValue(OpeningEffectProperty, value);
        }

        /// <summary>
        ///     Allows multiple transition effects to be combined and run upon the content loading or
        ///     being made visible.
        /// </summary>
        public ObservableCollection<TransitionEffectBase> OpeningEffects { get; } =
            new ObservableCollection<TransitionEffectBase>();

        /// <summary>Delay offset to be applied to all opening effect transitions.</summary>
        public TimeSpan OpeningEffectsOffset
        {
            get => (TimeSpan)GetValue(OpeningEffectsOffsetProperty);
            set => SetValue(OpeningEffectsOffsetProperty, value);
        }

        #endregion

        #region ITransitionEffectSubject Implementation

        string ITransitionEffectSubject.MatrixTransformName => MatrixTransformPartName;

        TimeSpan ITransitionEffectSubject.Offset => OpeningEffectsOffset;

        string ITransitionEffectSubject.RotateTransformName => RotateTransformPartName;

        string ITransitionEffectSubject.ScaleTransformName => ScaleTransformPartName;

        string ITransitionEffectSubject.SkewTransformName => SkewTransformPartName;

        string ITransitionEffectSubject.TranslateTransformName => TranslateTransformPartName;

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            var nameScopeRoot = GetNameScopeRoot();

            this._matrixTransform = GetTemplateChild(MatrixTransformPartName) as MatrixTransform;
            this._rotateTransform = GetTemplateChild(RotateTransformPartName) as RotateTransform;
            this._scaleTransform = GetTemplateChild(ScaleTransformPartName) as ScaleTransform;
            this._skewTransform = GetTemplateChild(SkewTransformPartName) as SkewTransform;
            this._translateTransform =
                GetTemplateChild(TranslateTransformPartName) as TranslateTransform;

            UnregisterNames(
                MatrixTransformPartName,
                RotateTransformPartName,
                ScaleTransformPartName,
                SkewTransformPartName,
                TranslateTransformPartName);

            if (this._matrixTransform != null)
            {
                nameScopeRoot.RegisterName(MatrixTransformPartName, this._matrixTransform);
            }

            if (this._rotateTransform != null)
            {
                nameScopeRoot.RegisterName(RotateTransformPartName, this._rotateTransform);
            }

            if (this._scaleTransform != null)
            {
                nameScopeRoot.RegisterName(ScaleTransformPartName, this._scaleTransform);
            }

            if (this._skewTransform != null)
            {
                nameScopeRoot.RegisterName(SkewTransformPartName, this._skewTransform);
            }

            if (this._translateTransform != null)
            {
                nameScopeRoot.RegisterName(TranslateTransformPartName, this._translateTransform);
            }

            base.OnApplyTemplate();

            RunOpeningEffects();

            void UnregisterNames(params string[] names)
            {
                foreach (var name in names.Where(n => FindName(n) != null))
                {
                    UnregisterName(name);
                }
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void RunOpeningEffects()
        {
            if (!IsLoaded || this._matrixTransform is null)
            {
                return;
            }

            var storyboard = new Storyboard();
            var openingEffect = OpeningEffect?.Build(this);

            if (openingEffect != null)
            {
                storyboard.Children.Add(openingEffect);
            }

            foreach (var effect in OpeningEffects.Select(e => e.Build(this))
                                                 .Where(tl => !(tl is null)))
            {
                storyboard.Children.Add(effect);
            }

            storyboard.Begin(GetNameScopeRoot());
        }

        #endregion

        #region Private Methods

        private FrameworkElement GetNameScopeRoot()
        {
            //https://github.com/ButchersBoy/MaterialDesignInXamlToolkit/issues/950
            //Only set the NameScope if the child does not already have a TemplateNameScope set
            if ((VisualChildrenCount > 0) && GetVisualChild(0) is FrameworkElement fe &&
                (NameScope.GetNameScope(fe) != null))
            {
                return fe;
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