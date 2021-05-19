﻿namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;

    [Flags]
    public enum TransitioningContentRunHint
    {
        Loaded = 1,
        IsVisibleChanged = 2,
        All = Loaded | IsVisibleChanged
    }

    /// <summary>Content control to enable easier transitions.</summary>
    public class TransitioningContent : TransitioningContentBase
    {
        #region Constants and Fields

        public static readonly DependencyProperty RunHintProperty = DependencyProperty.Register(
            nameof(RunHint),
            typeof(TransitioningContentRunHint),
            typeof(TransitioningContent),
            new PropertyMetadata(TransitioningContentRunHint.All));

        #endregion

        #region Constructors and Destructors

        static TransitioningContent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TransitioningContent),
                new FrameworkPropertyMetadata(typeof(TransitioningContent)));
        }

        public TransitioningContent()
        {
            Loaded += (sender, args) => Run(TransitioningContentRunHint.Loaded);
            IsVisibleChanged += (sender, args) => Run(TransitioningContentRunHint.IsVisibleChanged);
        }

        #endregion

        #region Public Properties

        public TransitioningContentRunHint RunHint
        {
            get => (TransitioningContentRunHint)GetValue(RunHintProperty);
            set => SetValue(RunHintProperty, value);
        }

        #endregion

        #region Private Methods

        private void Run(TransitioningContentRunHint requiredHint)
        {
            if ((RunHint & requiredHint) == requiredHint)
            {
                RunOpeningEffects();
            }
        }

        #endregion
    }
}