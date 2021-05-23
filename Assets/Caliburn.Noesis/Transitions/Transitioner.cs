namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    ///     The transitioner provides an easy way to transition from one content to another using wipe
    ///     transitions.
    /// </summary>
    public class Transitioner : Selector, IZIndexController
    {
        #region Constants and Fields

        /// <summary>The AutoApplyTransitionOrigins property.</summary>
        public static readonly DependencyProperty AutoApplyTransitionOriginsProperty =
            DependencyProperty.Register(
                nameof(AutoApplyTransitionOrigins),
                typeof(bool),
                typeof(Transitioner),
                new PropertyMetadata(default(bool)));

        /// <summary>The BackwardWipe property.</summary>
        public static readonly DependencyProperty BackwardWipeProperty =
            DependencyProperty.Register(
                nameof(BackwardWipe),
                typeof(ITransitionWipe),
                typeof(Transitioner),
                new PropertyMetadata(new SlideWipe { Direction = SlideDirection.Right }));

        /// <summary>The DefaultTransitionOrigin property.</summary>
        public static readonly DependencyProperty DefaultTransitionOriginProperty =
            DependencyProperty.Register(
                nameof(DefaultTransitionOrigin),
                typeof(Point),
                typeof(Transitioner),
                new PropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>The ForwardWipe property.</summary>
        public static readonly DependencyProperty ForwardWipeProperty = DependencyProperty.Register(
            nameof(ForwardWipe),
            typeof(ITransitionWipe),
            typeof(Transitioner),
            new PropertyMetadata(new SlideWipe { Direction = SlideDirection.Left }));

        /// <summary>The IsLooping property</summary>
        public static readonly DependencyProperty IsLoopingProperty = DependencyProperty.Register(
            nameof(IsLooping),
            typeof(bool),
            typeof(Transitioner),
            new PropertyMetadata(default(bool)));

        /// <summary>Moves to the first item.</summary>
        public static readonly RoutedUICommand MoveFirstCommand = new RoutedUICommand(
            "First",
            nameof(MoveFirstCommand),
            typeof(Transitioner));

        /// <summary>Moves to the last item.</summary>
        public static readonly RoutedUICommand MoveLastCommand = new RoutedUICommand(
            "Last",
            nameof(MoveLastCommand),
            typeof(Transitioner));

        /// <summary>
        ///     Causes the the next item to be displayed (effectively increments
        ///     <see cref="Selector.SelectedIndex" />).
        /// </summary>
        public static readonly RoutedUICommand MoveNextCommand = new RoutedUICommand(
            "Next",
            nameof(MoveNextCommand),
            typeof(Transitioner));

        /// <summary>
        ///     Causes the the previous item to be displayed (effectively decrements
        ///     <see cref="Selector.SelectedIndex" />).
        /// </summary>
        public static readonly RoutedUICommand MovePreviousCommand = new RoutedUICommand(
            "Previous",
            nameof(MovePreviousCommand),
            typeof(Transitioner));

        private Point? nextTransitionOrigin;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes the <see cref="Transitioner" /> class.</summary>
        static Transitioner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(Transitioner),
                new FrameworkPropertyMetadata(typeof(Transitioner)));
        }

        /// <summary>Initializes a new instance of the <see cref="Transitioner" /> class.</summary>
        public Transitioner()
        {
            CommandBindings.Add(new CommandBinding(MoveNextCommand, OnMoveNext, OnCanMoveNext));
            CommandBindings.Add(
                new CommandBinding(MovePreviousCommand, OnMovePrevious, OnCanMovePrevious));
            CommandBindings.Add(new CommandBinding(MoveFirstCommand, OnMoveFirst, OnCanMoveFirst));
            CommandBindings.Add(new CommandBinding(MoveLastCommand, OnMoveLast, OnCanMoveLast));

            AddHandler(
                TransitionSubjectBase.TransitionFinished,
                new RoutedEventHandler(OnIsTransitionFinished));

            Loaded += (sender, args) =>
                {
                    if (SelectedIndex != -1)
                    {
                        TransitionToItemIndex(SelectedIndex, -1);
                    }
                };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     If set to <c>true</c>, transition origins will be applied to wipes, according to where a
        ///     transition was triggered from.  For example, the mouse point where a user clicks a button.
        /// </summary>
        public bool AutoApplyTransitionOrigins
        {
            get => (bool)GetValue(AutoApplyTransitionOriginsProperty);
            set => SetValue(AutoApplyTransitionOriginsProperty, value);
        }

        /// <summary>Gets or sets the wipe used when transitioning backwards.</summary>
        /// <value>The wipe used when transitioning backwards.</value>
        public ITransitionWipe BackwardWipe
        {
            get => (ITransitionWipe)GetValue(BackwardWipeProperty);
            set => SetValue(BackwardWipeProperty, value);
        }

        /// <summary>Gets or sets the default origin of the transition wipe.</summary>
        /// <value>The default origin of the transition wipe.</value>
        public Point DefaultTransitionOrigin
        {
            get => (Point)GetValue(DefaultTransitionOriginProperty);
            set => SetValue(DefaultTransitionOriginProperty, value);
        }

        /// <summary>Gets or sets the wipe used when transitioning forwards.</summary>
        /// <value>The wipe used when transitioning forwards.</value>
        public ITransitionWipe ForwardWipe
        {
            get => (ITransitionWipe)GetValue(ForwardWipeProperty);
            set => SetValue(ForwardWipeProperty, value);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the last item is followed by the first in a loop
        ///     and vice versa.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the last item is followed by the first in a loop and vice versa; otherwise,
        ///     <c>false</c>.
        /// </value>
        public bool IsLooping
        {
            get => (bool)GetValue(IsLoopingProperty);
            set => SetValue(IsLoopingProperty, value);
        }

        #endregion

        #region IZIndexController Implementation

        /// <inheritdoc />
        void IZIndexController.Stack(params TransitionerItem[] highestToLowest)
        {
            StackZIndices(highestToLowest);
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TransitionerItem();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TransitionerItem;
        }

        /// <inheritdoc />
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (AutoApplyTransitionOrigins)
            {
                this.nextTransitionOrigin = GetNavigationSourcePoint(e);
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            var unselectedIndex = -1;

            if (e.RemovedItems.Count == 1)
            {
                unselectedIndex = Items.IndexOf(e.RemovedItems[0]);
            }

            var selectedIndex = 1;

            if (e.AddedItems.Count == 1)
            {
                selectedIndex = Items.IndexOf(e.AddedItems[0]);
            }

            TransitionToItemIndex(selectedIndex, unselectedIndex);

            base.OnSelectionChanged(e);
        }

        #endregion

        #region Event Handlers

        private void OnCanMoveFirst(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (Items.Count > 0) && (SelectedIndex > 0);
        }

        private void OnCanMoveLast(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (Items.Count > 0) && (SelectedIndex < Items.Count - 1);
        }

        private void OnCanMoveNext(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (Items.Count > 0) && (IsLooping || (SelectedIndex < Items.Count - 1));
        }

        private void OnCanMovePrevious(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (Items.Count > 0) && (IsLooping || (SelectedIndex > 0));
        }

        private void OnIsTransitionFinished(object sender, RoutedEventArgs e)
        {
            foreach (var item in Items.OfType<object>()
                                      .Select(GetItem)
                                      .Where(item => item.State == TransitionerItemState.Previous))
            {
                item.SetCurrentValue(TransitionerItem.StateProperty, TransitionerItemState.None);
            }
        }

        private void OnMoveFirst(object sender, ExecutedRoutedEventArgs e)
        {
            if (AutoApplyTransitionOrigins)
            {
                this.nextTransitionOrigin = GetNavigationSourcePoint(e);
            }

            SetCurrentValue(SelectedIndexProperty, 0);
        }

        private void OnMoveLast(object sender, ExecutedRoutedEventArgs e)
        {
            if (AutoApplyTransitionOrigins)
            {
                this.nextTransitionOrigin = GetNavigationSourcePoint(e);
            }

            SetCurrentValue(SelectedIndexProperty, Items.Count - 1);
        }

        private void OnMoveNext(object sender, ExecutedRoutedEventArgs e)
        {
            if (AutoApplyTransitionOrigins)
            {
                this.nextTransitionOrigin = GetNavigationSourcePoint(e);
            }

            var stepSize = GetStepSize(e);

            IncrementSelectedIndex(stepSize);
        }

        private void OnMovePrevious(object sender, ExecutedRoutedEventArgs e)
        {
            if (AutoApplyTransitionOrigins)
            {
                this.nextTransitionOrigin = GetNavigationSourcePoint(e);
            }

            var stepSize = GetStepSize(e);

            DecrementSelectedIndex(stepSize);
        }

        #endregion

        #region Private Methods

        private static int GetStepSize(ExecutedRoutedEventArgs args)
        {
            var stepSize = args.Parameter switch
                {
                    int i when i > 0 => i,
                    string s when int.TryParse(s, out var i) && (i > 0) => i,
                    _ => 1
                };

            return stepSize;
        }

        private static bool IsSafePositive(double @double)
        {
            return !double.IsNaN(@double) && !double.IsInfinity(@double) && (@double > 0.0);
        }

        private static void StackZIndices(params TransitionerItem[] highestToLowest)
        {
            var zIndex = highestToLowest.Length;

            foreach (var item in highestToLowest.Where(item => item != null))
            {
                Panel.SetZIndex(item, zIndex--);
            }
        }

        private void DecrementSelectedIndex(int stepSize)
        {
            var newIndex = IsLooping
                               ? (((SelectedIndex - stepSize) % Items.Count) + Items.Count) %
                                 Items.Count
                               : Math.Max(0, SelectedIndex - stepSize);

            SetCurrentValue(SelectedIndexProperty, newIndex);
        }

        private TransitionerItem GetItem(object item)
        {
            if (IsItemItsOwnContainer(item))
            {
                return (TransitionerItem)item;
            }

            return (TransitionerItem)ItemContainerGenerator.ContainerFromItem(item);
        }

        private Point? GetNavigationSourcePoint(RoutedEventArgs executedRoutedEventArgs)
        {
            if (!(executedRoutedEventArgs.OriginalSource is FrameworkElement sourceElement) ||
                !IsAncestorOf(sourceElement) || !IsSafePositive(ActualWidth) ||
                !IsSafePositive(ActualHeight) || !IsSafePositive(sourceElement.ActualWidth) ||
                !IsSafePositive(sourceElement.ActualHeight))
            {
                return null;
            }

            var transitionOrigin = sourceElement.TranslatePoint(
                new Point(sourceElement.ActualWidth / 2, sourceElement.ActualHeight / 2),
                this);

            transitionOrigin = new Point(
                transitionOrigin.X / ActualWidth,
                transitionOrigin.Y / ActualHeight);

            return transitionOrigin;
        }

        private Point GetTransitionOrigin(TransitionerItem item)
        {
            if (this.nextTransitionOrigin != null)
            {
                return this.nextTransitionOrigin.Value;
            }

            return item.ReadLocalValue(TransitionerItem.TransitionOriginProperty) !=
                   DependencyProperty.UnsetValue
                       ? item.TransitionOrigin
                       : DefaultTransitionOrigin;
        }

        private void IncrementSelectedIndex(int stepSize)
        {
            var newIndex = IsLooping
                               ? (SelectedIndex + stepSize) % Items.Count
                               : Math.Min(Items.Count - 1, SelectedIndex + stepSize);

            SetCurrentValue(SelectedIndexProperty, newIndex);
        }

        private void TransitionToItemIndex(int currentIndex, int previousIndex)
        {
            if (!IsLoaded)
            {
                return;
            }

            TransitionerItem oldItem = null, newItem = null;

            for (var index = 0; index < Items.Count; index++)
            {
                var item = GetItem(Items[index]);

                if (item == null)
                {
                    continue;
                }

                if (index == currentIndex)
                {
                    newItem = item;
                    item.SetCurrentValue(
                        TransitionerItem.StateProperty,
                        TransitionerItemState.Current);
                }
                else if (index == previousIndex)
                {
                    oldItem = item;
                    item.SetCurrentValue(
                        TransitionerItem.StateProperty,
                        TransitionerItemState.Previous);
                }
                else
                {
                    item.SetCurrentValue(
                        TransitionerItem.StateProperty,
                        TransitionerItemState.None);
                }

                item.CancelTransition();

                Panel.SetZIndex(item, 0);
            }

            if (newItem != null)
            {
                newItem.Opacity = 1;
            }

            if ((oldItem != null) && (newItem != null))
            {
                var wipe = currentIndex > previousIndex
                               ? newItem.ForwardWipe ?? ForwardWipe
                               : newItem.BackwardWipe ?? BackwardWipe;

                oldItem.Opacity = 0;

                if (wipe != null)
                {
                    wipe.Wipe(oldItem, newItem, GetTransitionOrigin(newItem), this);
                }
                else
                {
                    StackZIndices(newItem, oldItem);
                }
            }
            else if ((oldItem != null) || (newItem != null))
            {
                if (oldItem != null)
                {
                    oldItem.Opacity = 0;
                }

                StackZIndices(oldItem ?? newItem);
            }

            this.nextTransitionOrigin = null;
        }

        #endregion
    }
}