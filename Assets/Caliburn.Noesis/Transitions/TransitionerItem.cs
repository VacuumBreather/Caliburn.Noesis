namespace Caliburn.Noesis.Transitions
{
    using System.Windows;

    /// <summary>
    ///     Content control to host the content of an individual page within a
    ///     <see cref="Transitioner" />.
    /// </summary>
    public class TransitionerItem : TransitioningContentBase
    {
        #region Constants and Fields

        public static readonly RoutedEvent InTransitionFinished = EventManager.RegisterRoutedEvent(
            "InTransitionFinished",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TransitionerItem));

        public static readonly DependencyProperty TransitionOriginProperty =
            DependencyProperty.Register(
                "TransitionOrigin",
                typeof(Point),
                typeof(Transitioner),
                new PropertyMetadata(new Point(0.5, 0.5)));

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State",
            typeof(TransitionerItemState),
            typeof(TransitionerItem),
            new PropertyMetadata(default(TransitionerItemState), StatePropertyChangedCallback));

        public static readonly DependencyProperty BackwardWipeProperty =
            DependencyProperty.Register(
                "BackwardWipe",
                typeof(ITransitionWipe),
                typeof(TransitionerItem),
                new PropertyMetadata(new SlideOutWipe()));

        public static readonly DependencyProperty ForwardWipeProperty = DependencyProperty.Register(
            "ForwardWipe",
            typeof(ITransitionWipe),
            typeof(TransitionerItem),
            new PropertyMetadata(new CircleWipe()));

        #endregion

        #region Constructors and Destructors

        static TransitionerItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitionerItem),
                new FrameworkPropertyMetadata(typeof(TransitionerItem)));
        }

        #endregion

        #region Public Properties

        public ITransitionWipe BackwardWipe
        {
            get => (ITransitionWipe)GetValue(BackwardWipeProperty);
            set => SetValue(BackwardWipeProperty, value);
        }

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

        #region Protected Methods

        protected void OnInTransitionFinished(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion

        #region Private Methods

        private static void StatePropertyChangedCallback(DependencyObject d,
                                                         DependencyPropertyChangedEventArgs e)
        {
            ((TransitionerItem)d).AnimateToState();
        }

        private void AnimateToState()
        {
            if (State != TransitionerItemState.Current)
            {
                return;
            }

            RunOpeningEffects();
        }

        #endregion
    }
}