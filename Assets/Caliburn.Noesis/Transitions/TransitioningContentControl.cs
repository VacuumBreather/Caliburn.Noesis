namespace Caliburn.Noesis.Transitions
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
#endif

    /// <summary>Enables transition effects for content.</summary>
    public class TransitioningContentControl : TransitionSubjectBase
    {
        #region Constants and Fields

        /// <summary>The TransitionTriggers property.</summary>
        public static readonly DependencyProperty TransitionTriggersProperty =
            DependencyProperty.Register(
                nameof(TransitionTriggers),
                typeof(TransitionTriggers),
                typeof(TransitioningContentControl),
                new PropertyMetadata(TransitionTriggers.Default));

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes the <see cref="TransitioningContentControl" /> class.</summary>
        static TransitioningContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitioningContentControl),
                new FrameworkPropertyMetadata(typeof(TransitioningContentControl)));
        }

        /// <summary>Initializes a new instance of the <see cref="TransitioningContentControl" /> class.</summary>
        public TransitioningContentControl()
        {
            Loaded += (_, __) => Run(TransitionTriggers.Loaded);
            IsVisibleChanged += OnIsVisibleChanged;
            IsEnabledChanged += OnIsEnabledChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the triggers that start the transition.</summary>
        /// <value>The triggers that start the transition.</value>
        public TransitionTriggers TransitionTriggers
        {
            get => (TransitionTriggers)GetValue(TransitionTriggersProperty);
            set => SetValue(TransitionTriggersProperty, value);
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            Run(TransitionTriggers.ContentChanged);
        }

        #endregion

        #region Event Handlers

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Run(TransitionTriggers.IsEnabled);
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Run(TransitionTriggers.IsVisible);
        }

        #endregion

        #region Private Methods

        private void Run(TransitionTriggers requiredHint)
        {
            if (((TransitionTriggers & requiredHint) != 0) && IsEnabled &&
                (Visibility == Visibility.Visible))
            {
                PerformTransition();
            }
        }

        #endregion
    }
}