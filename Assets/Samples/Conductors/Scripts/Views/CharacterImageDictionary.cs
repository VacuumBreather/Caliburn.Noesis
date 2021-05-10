namespace Caliburn.Noesis.Samples.Conductors.Views
{
    using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Media;
#endif

    public class CharacterImageDictionary : Dictionary<string, ImageBrush>
    {
    }
}