namespace Caliburn.Noesis.Controls
{

#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows.Controls;

#endif

    /// <summary>
    /// A <see cref="ContentControl"/> used as a container for window content.
    /// </summary>
    /// <seealso cref="ContentControl" />
    public class Window : ContentControl
    {
    }
}
