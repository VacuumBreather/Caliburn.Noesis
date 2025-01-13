#if UNITY_5_5_OR_NEWER
namespace Caliburn.Noesis.Extensions
{
    using System;
    using System.Runtime.CompilerServices;
    using global::Noesis;

    /// <summary>Provides extension methods for the <see cref="UserControl" /> type.</summary>
    public static class UserControlExtensions
    {
        #region Public Methods

        /// <summary>Initializes this <see cref="UserControl" />.</summary>
        /// <param name="userControl">This <see cref="UserControl" />.</param>
        /// <param name="callerFilePath">The full path of the source file that contains the caller.</param>
        public static void InitializeComponent(this UserControl userControl,
                                               [CallerFilePath] string callerFilePath = default)
        {
            GUI.LoadComponent(userControl, GetXamlPath(callerFilePath));
        }

        #endregion

        #region Private Methods

        private static string GetXamlPath(string callerFilePath)
        {
            var xamlPath = callerFilePath?.Substring(callerFilePath.IndexOf("Assets", StringComparison.Ordinal))
                                         .Replace(".cs", string.Empty)
                                         .Replace('\\', '/');

            return xamlPath;
        }

        #endregion
    }
}
#endif