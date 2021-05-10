namespace Caliburn.Noesis.Controls
{
#if UNITY_5_5_OR_NEWER
    using Noesis;
#else
    using System.Windows;
    using System.Windows.Controls;

#endif

    /// <summary>Represents the container for an item in a <see cref="FlipView" /> control.</summary>
    /// <remarks>
    ///     The <see cref="FlipViewItem" /> class provides the container for items displayed in a
    ///     <see cref="FlipView" /> control. You populate the <see cref="FlipView" /> by adding objects
    ///     directly to its Items collection or by binding its <see cref="FlipView.ItemsSource" /> property
    ///     to a data source. When items are added to the <see cref="FlipView" />, a
    ///     <see cref="FlipViewItem" /> container is created automatically for each item in the collection.
    ///     You can specify the look of the <see cref="FlipViewItem" /> by setting the
    ///     <see cref="FlipView" />'s <see cref="FlipView.ItemContainerStyle" /> property to a
    ///     <see cref="Style" /> with a <see cref="Style.TargetType" /> of <see cref="FlipViewItem" />.
    /// </remarks>
    public class FlipViewItem : ContentControl
    {
    }
}