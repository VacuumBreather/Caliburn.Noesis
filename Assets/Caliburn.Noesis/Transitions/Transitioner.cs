namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    ///     The transitioner provides an easy way to move between content with a default in-place
    ///     circular transition.
    /// </summary>
    public class Transitioner : Selector, IZIndexController
    {
        #region Constants and Fields

        public static readonly DependencyProperty AutoApplyTransitionOriginsProperty =
            DependencyProperty.Register(
                "AutoApplyTransitionOrigins",
                typeof(bool),
                typeof(Transitioner),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty DefaultTransitionOriginProperty =
            DependencyProperty.Register(
                "DefaultTransitionOrigin",
                typeof(Point),
                typeof(Transitioner),
                new PropertyMetadata(new Point(0.5, 0.5)));

        /// <summary>Moves to the first slide.</summary>
        public static readonly RoutedUICommand MoveFirstCommand = new RoutedUICommand("First", nameof(MoveFirstCommand), typeof(Transitioner));

        /// <summary>Moves to the last slide.</summary>
        public static readonly RoutedUICommand MoveLastCommand = new RoutedUICommand("Last", nameof(MoveLastCommand), typeof(Transitioner));

        /// <summary>
        ///     Causes the the next slide to be displayed (affectively increments
        ///     <see cref="Selector.SelectedIndex" />).
        /// </summary>
        public static readonly RoutedUICommand MoveNextCommand = new RoutedUICommand("Next", nameof(MoveNextCommand), typeof(Transitioner));

        /// <summary>
        ///     Causes the the previous slide to be displayed (affectively decrements
        ///     <see cref="Selector.SelectedIndex" />).
        /// </summary>
        public static readonly RoutedUICommand MovePreviousCommand = new RoutedUICommand("Previous", nameof(MovePreviousCommand), typeof(Transitioner));

        private Point? _nextTransitionOrigin;

        #endregion

        #region Constructors and Destructors

        static Transitioner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(Transitioner),
                new FrameworkPropertyMetadata(typeof(Transitioner)));
        }

        public Transitioner()
        {
            CommandBindings.Add(new CommandBinding(MoveNextCommand, MoveNextHandler));
            CommandBindings.Add(new CommandBinding(MovePreviousCommand, MovePreviousHandler));
            CommandBindings.Add(new CommandBinding(MoveFirstCommand, MoveFirstHandler));
            CommandBindings.Add(new CommandBinding(MoveLastCommand, MoveLastHandler));
            AddHandler(
                TransitionerItem.InTransitionFinished,
                new RoutedEventHandler(IsTransitionFinishedHandler));
            Loaded += (sender, args) =>
                {
                    if (SelectedIndex != -1)
                    {
                        ActivateFrame(SelectedIndex, -1);
                    }
                };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     If enabled, transition origins will be applied to wipes, according to where a transition
        ///     was triggered from.  For example, the mouse point where a user clicks a button.
        /// </summary>
        public bool AutoApplyTransitionOrigins
        {
            get => (bool)GetValue(AutoApplyTransitionOriginsProperty);
            set => SetValue(AutoApplyTransitionOriginsProperty, value);
        }

        public Point DefaultTransitionOrigin
        {
            get => (Point)GetValue(DefaultTransitionOriginProperty);
            set => SetValue(DefaultTransitionOriginProperty, value);
        }

        #endregion

        #region IZIndexController Implementation

        void IZIndexController.Stack(params TransitionerItem?[] highestToLowest)
        {
            DoStack(highestToLowest);
        }

        #endregion

        #region Protected Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TransitionerItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TransitionerItem;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (AutoApplyTransitionOrigins)
            {
                this._nextTransitionOrigin = GetNavigationSourcePoint(e);
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }

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

            ActivateFrame(selectedIndex, unselectedIndex);

            base.OnSelectionChanged(e);
        }

        #endregion

        #region Event Handlers

        private void IsTransitionFinishedHandler(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (var slide in Items.OfType<object>()
                                       .Select(GetSlide)
                                       .Where(s => s.State == TransitionerItemState.Previous))
            {
                slide.SetCurrentValue(TransitionerItem.StateProperty, TransitionerItemState.None);
            }
        }

        private void MoveFirstHandler(object sender,
                                      ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (AutoApplyTransitionOrigins)
            {
                this._nextTransitionOrigin = GetNavigationSourcePoint(executedRoutedEventArgs);
            }

            SetCurrentValue(SelectedIndexProperty, 0);
        }

        private void MoveLastHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (AutoApplyTransitionOrigins)
            {
                this._nextTransitionOrigin = GetNavigationSourcePoint(executedRoutedEventArgs);
            }

            SetCurrentValue(SelectedIndexProperty, Items.Count - 1);
        }

        private void MoveNextHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (AutoApplyTransitionOrigins)
            {
                this._nextTransitionOrigin = GetNavigationSourcePoint(executedRoutedEventArgs);
            }

            var slides = 1;

            if (executedRoutedEventArgs.Parameter is int &&
                ((int)executedRoutedEventArgs.Parameter > 0))
            {
                slides = (int)executedRoutedEventArgs.Parameter;
            }

            SetCurrentValue(
                SelectedIndexProperty,
                Math.Min(Items.Count - 1, SelectedIndex + slides));
        }

        private void MovePreviousHandler(object sender,
                                         ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (AutoApplyTransitionOrigins)
            {
                this._nextTransitionOrigin = GetNavigationSourcePoint(executedRoutedEventArgs);
            }

            var slides = 1;

            if (executedRoutedEventArgs.Parameter is int &&
                ((int)executedRoutedEventArgs.Parameter > 0))
            {
                slides = (int)executedRoutedEventArgs.Parameter;
            }

            SetCurrentValue(SelectedIndexProperty, Math.Max(0, SelectedIndex - slides));
        }

        #endregion

        #region Private Methods

        private static void DoStack(params TransitionerItem?[] highestToLowest)
        {
            var pos = highestToLowest.Length;

            foreach (var slide in highestToLowest.Where(s => s != null))
            {
                Panel.SetZIndex(slide, pos--);
            }
        }

        private static bool IsSafePositive(double @double)
        {
            return !double.IsNaN(@double) && !double.IsInfinity(@double) && (@double > 0.0);
        }

        private void ActivateFrame(int selectedIndex, int unselectedIndex)
        {
            if (!IsLoaded)
            {
                return;
            }

            TransitionerItem? oldSlide = null, newSlide = null;

            for (var index = 0; index < Items.Count; index++)
            {
                var slide = GetSlide(Items[index]);

                if (slide == null)
                {
                    continue;
                }

                if (index == selectedIndex)
                {
                    newSlide = slide;
                    slide.SetCurrentValue(
                        TransitionerItem.StateProperty,
                        TransitionerItemState.Current);
                }
                else if (index == unselectedIndex)
                {
                    oldSlide = slide;
                    slide.SetCurrentValue(
                        TransitionerItem.StateProperty,
                        TransitionerItemState.Previous);
                }
                else
                {
                    slide.SetCurrentValue(
                        TransitionerItem.StateProperty,
                        TransitionerItemState.None);
                }

                Panel.SetZIndex(slide, 0);
            }

            if (newSlide != null)
            {
                newSlide.Opacity = 1;
            }

            if ((oldSlide != null) && (newSlide != null))
            {
                var wipe = selectedIndex > unselectedIndex
                               ? oldSlide.ForwardWipe
                               : oldSlide.BackwardWipe;

                if (wipe != null)
                {
                    wipe.Wipe(oldSlide, newSlide, GetTransitionOrigin(newSlide), this);
                }
                else
                {
                    DoStack(newSlide, oldSlide);
                }

                oldSlide.Opacity = 0;
            }
            else if ((oldSlide != null) || (newSlide != null))
            {
                DoStack(oldSlide ?? newSlide);

                if (oldSlide != null)
                {
                    oldSlide.Opacity = 0;
                }
            }

            this._nextTransitionOrigin = null;
        }

        private Point? GetNavigationSourcePoint(RoutedEventArgs executedRoutedEventArgs)
        {
            var sourceElement = executedRoutedEventArgs.OriginalSource as FrameworkElement;

            if ((sourceElement == null) || !IsAncestorOf(sourceElement) ||
                !IsSafePositive(ActualWidth) || !IsSafePositive(ActualHeight) ||
                !IsSafePositive(sourceElement.ActualWidth) ||
                !IsSafePositive(sourceElement.ActualHeight))
            {
                return null;
            }

            var transitionOrigin = sourceElement.TranslatePoint(
                new Point(sourceElement.ActualWidth / 2, sourceElement.ActualHeight),
                this);
            transitionOrigin = new Point(
                transitionOrigin.X / ActualWidth,
                transitionOrigin.Y / ActualHeight);

            return transitionOrigin;
        }

        private TransitionerItem GetSlide(object item)
        {
            if (IsItemItsOwnContainer(item))
            {
                return (TransitionerItem)item;
            }

            return (TransitionerItem)ItemContainerGenerator.ContainerFromItem(item);
        }

        private Point GetTransitionOrigin(TransitionerItem item)
        {
            if (this._nextTransitionOrigin != null)
            {
                return this._nextTransitionOrigin.Value;
            }

            if (item.ReadLocalValue(TransitionerItem.TransitionOriginProperty) !=
                DependencyProperty.UnsetValue)
            {
                return item.TransitionOrigin;
            }

            return DefaultTransitionOrigin;
        }

        #endregion
    }
}