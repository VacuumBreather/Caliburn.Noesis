namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;

    [Flags]
    public enum TransitioningContentRunHint
    {
        Loaded = 1,
        IsVisibleChanged = 2,
        ContentChanged = 3,
        All = Loaded | IsVisibleChanged | ContentChanged
    }

    /// <summary>A content control which enables transition animations.</summary>
    public class TransitioningContentControl : TransitioningContentControlBase
    {
        #region Constants and Fields

        /// <summary>The RunHint property.</summary>
        public static readonly DependencyProperty RunHintProperty = DependencyProperty.Register(
            nameof(RunHint),
            typeof(TransitioningContentRunHint),
            typeof(TransitioningContentControl),
            new PropertyMetadata(TransitioningContentRunHint.All));

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
            Loaded += (_, __) => Run(TransitioningContentRunHint.Loaded);
            IsVisibleChanged += (_, __) => Run(TransitioningContentRunHint.IsVisibleChanged);
        }

        #endregion

        #region Public Properties

        public TransitioningContentRunHint RunHint
        {
            get => (TransitioningContentRunHint)GetValue(RunHintProperty);
            set => SetValue(RunHintProperty, value);
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            Run(TransitioningContentRunHint.ContentChanged);
        }

        #endregion

        #region Private Methods

        private void Run(TransitioningContentRunHint requiredHint)
        {
            if ((RunHint & requiredHint) != 0)
            {
                RunTransitionEffects();
            }
        }

        #endregion
    }
}