namespace Caliburn.Noesis.Samples.ViewModels
{
    #region Using Directives

    using System;
    using System.IO;

    #endregion

    /// <summary>Provides extension methods for the <see cref="DirectoryInfo" /> type.</summary>
    public static class DirectoryInfoExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Checks whether the current directory is an ancestor directory of the specified potential
        ///     descendant directory.
        /// </summary>
        /// <param name="directory">The current directory.</param>
        /// <param name="descendant">The potential descendant directory.</param>
        /// <returns>
        ///     <c>true</c> if the current directory is an ancestor directory of the specified potential
        ///     descendant directory; otherwise, <c>false</c>.
        /// </returns>
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

        /// <summary>Checks whether the current directory is identical to the specified other directory.</summary>
        /// <param name="directory">The current directory.</param>
        /// <param name="otherDirectory">The directory to compare.</param>
        /// <returns>
        ///     <c>true</c> if the current directory is identical to the specified other directory;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameAs(this DirectoryInfo directory, DirectoryInfo otherDirectory)
        {
            return string.Compare(
                       Path.GetFullPath(directory.FullName).TrimEnd('\\'),
                       Path.GetFullPath(otherDirectory.FullName).TrimEnd('\\'),
                       StringComparison.InvariantCultureIgnoreCase) ==
                   0;
        }

        #endregion
    }
}