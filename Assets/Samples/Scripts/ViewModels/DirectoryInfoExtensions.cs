namespace Samples.ViewModels
{
    #region Using Directives

    using System;
    using System.IO;

    #endregion

    /// <summary>
    ///     Provides extension methods for the <see cref="DirectoryInfo" /> type.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        #region Public Methods

        public static bool IsAncestorOf(this DirectoryInfo directory, DirectoryInfo descendant)
        {
            for (var current = descendant; current != null; current = current.Parent)
            {
                if (current.IsSameAs(directory))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsSameAs(this DirectoryInfo directory, DirectoryInfo otherDirectory)
        {
            return 0 ==
                   string.Compare(
                       Path.GetFullPath(directory.FullName).TrimEnd('\\'),
                       Path.GetFullPath(otherDirectory.FullName).TrimEnd('\\'),
                       StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}