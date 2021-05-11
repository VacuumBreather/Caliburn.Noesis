namespace Caliburn.Noesis.Samples.Conductors.Views
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Media;
#endif
    using System.Collections.Generic;

    /// <summary>A dictionary used to look up image sources by name in XAML.</summary>
    /// <seealso cref="Dictionary{TKey,TValue}" />
    public class ImageSourceDictionary : Dictionary<string, ImageSource>
    {
    }
}