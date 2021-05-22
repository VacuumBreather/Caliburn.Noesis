namespace Caliburn.Noesis.Transitions
{
    using System.Windows;
    using JetBrains.Annotations;

    /// <summary>
    ///     Content control to host the content of an individual page within a
    ///     <see cref="Transitioner" />.
    /// </summary>
    [PublicAPI]
    public class TransitionerItem : TransitioningContentControlBase
    {
        #region Constants and Fields

        /// <summary>The TransitionOrigin property.</summary>
        public static readonly DependencyProperty TransitionOriginProperty =
            DependencyProperty.Register(
                nameof(TransitionOrigin),
                typeof(Point),
                typeof(Transitioner),
                new PropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>The State property.</summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            nameof(State),
            typeof(TransitionerItemState),
            typeof(TransitionerItem),
            new PropertyMetadata(default(TransitionerItemState), StatePropertyChangedCallback));

        /// <summary>The BackwardWipe property.</summary>
        public static readonly DependencyProperty BackwardWipeProperty =
            DependencyProperty.Register(
                nameof(BackwardWipe),
                typeof(ITransitionWipe),
                typeof(TransitionerItem),
                new PropertyMetadata(default));

        /// <summary>The ForwardWipe property.</summary>
        public static readonly DependencyProperty ForwardWipeProperty = DependencyProperty.Register(
            nameof(ForwardWipe),
            typeof(ITransitionWipe),
            typeof(TransitionerItem),
            new PropertyMetadata(default));

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes the <see cref="TransitionerItem" /> class.</summary>
        static TransitionerItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitionerItem),
                new FrameworkPropertyMetadata(typeof(TransitionerItem)));
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the wipe used when transitioning backwards.</summary>
        /// <value>The wipe used when transitioning backwards.</value>
        public ITransitionWipe BackwardWipe
        {
            get => (ITransitionWipe)GetValue(BackwardWipeProperty);
            set => SetValue(BackwardWipeProperty, value);
        }

        /// <summary>Gets or sets the wipe used when transitioning forwards.</summary>
        /// <value>The wipe used when transitioning forwards.</value>
        public ITransitionWipe ForwardWipe
        {
            get => (ITransitionWipe)GetValue(ForwardWipeProperty);
            set => SetValue(ForwardWipeProperty, value);
        }

        public TransitionerItemState State
        {
            get => (TransitionerItemState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public Point TransitionOrigin
        {
            get => (Point)GetValue(TransitionOriginProperty);
            set => SetValue(TransitionOriginProperty, value);
        }

        #endregion

        #region Private Methods

        private static void StatePropertyChangedCallback(DependencyObject d,
                                                         DependencyPropertyChangedEventArgs e)
        {
            ((TransitionerItem)d).RunOpeningEffectsIfCurrent();
        }

        private void RunOpeningEffectsIfCurrent()
        {
            if (State == TransitionerItemState.Current)
            {
                RunTransitionEffects();
            }
        }

        #endregion
    }
}