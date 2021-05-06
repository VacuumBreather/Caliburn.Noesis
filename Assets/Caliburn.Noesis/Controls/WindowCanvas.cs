namespace Caliburn.Noesis.Controls
{
    #region Using Directives

#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows.Controls;
#endif

    #endregion

    /// <summary>Used to host windows.</summary>
    /// <seealso cref="Canvas" />
    public class WindowCanvas : Canvas
    {
    }
}