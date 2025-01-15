#if UNITY_5_5_OR_NEWER
using global::Noesis;
#else
using System.Windows.Markup;
#endif

[assembly: XmlnsDefinition("http://caliburn-noesis.com", "Caliburn.Micro")]
[assembly: XmlnsPrefix("http://caliburn-noesis.com", "cal")]
