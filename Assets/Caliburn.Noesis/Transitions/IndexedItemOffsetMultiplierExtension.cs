namespace Caliburn.Noesis.Transitions
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;

    /// <summary>Multiplies a time span unit by the index of an item in a list.</summary>
    /// <remarks>
    ///     Example usage is for a <see cref="TransitioningContentControl" /> to have a
    ///     <see cref="TransitionBase.Delay" /> time delayed according to position in a list, so cascading
    ///     animations can occur.
    /// </remarks>
    [MarkupExtensionReturnType(typeof(TimeSpan))]
    public class IndexedItemOffsetMultiplierExtension : MarkupExtension
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IndexedItemOffsetMultiplierExtension" />
        ///     class.
        /// </summary>
        /// <param name="interval">The unit.</param>
        public IndexedItemOffsetMultiplierExtension(TimeSpan interval)
        {
            Interval = interval;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the unit.</summary>
        /// <value>The unit.</value>
        [ConstructorArgument("unit")]
        public TimeSpan Interval { get; set; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget =
                serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (provideValueTarget == null)
            {
                return TimeSpan.Zero;
            }

            if ((provideValueTarget.TargetObject != null) &&
                (provideValueTarget.TargetObject.GetType().FullName == "System.Windows.SharedDp"))

                // We are inside a template, return this, so we can re-evaluate later...
            {
                return this;
            }

            var element = provideValueTarget?.TargetObject as DependencyObject;

            if (element == null)
            {
                return TimeSpan.Zero;
            }

            var itemsControl = ItemsControl.ItemsControlFromItemContainer(element);

            if (itemsControl == null)
            {
                var ancestor = element;

                while ((ancestor != null) && (itemsControl == null))
                {
                    ancestor = VisualTreeHelper.GetParent(ancestor);
                    itemsControl = ItemsControl.ItemsControlFromItemContainer(ancestor);
                }
            }

            if (itemsControl == null)
            {
                return TimeSpan.Zero;
            }

            var isOwnContainer = itemsControl.IsItemItsOwnContainer(element);
            var container = isOwnContainer
                                ? element
                                : itemsControl.ItemContainerGenerator.ContainerFromItem(element);

            if (container == null)
            {
                var dataContext = (element as FrameworkElement)?.DataContext;

                if (dataContext != null)
                {
                    container = itemsControl.ItemContainerGenerator.ContainerFromItem(dataContext);
                }
            }

            if (container == null)
            {
                return TimeSpan.Zero;
            }

            var multiplier = itemsControl.ItemContainerGenerator.IndexFromContainer(container);

            // Container generation may not have completed.
            if (multiplier == -1)
            {
                multiplier = itemsControl.Items.IndexOf(element);
            }

            // Still not found, repeat now using DataContext.
            if (multiplier == -1)
            {
                if (element is FrameworkElement frameworkElement)
                {
                    multiplier = itemsControl.Items.IndexOf(frameworkElement.DataContext);
                }
            }

            return multiplier > -1 ? new TimeSpan(Interval.Ticks * multiplier) : TimeSpan.Zero;
        }

        #endregion
    }
}