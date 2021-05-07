﻿namespace Caliburn.Noesis.Extensions
{
    using System.Reflection;

    /// <summary>Provides extension methods for the <see cref="Assembly" /> type.</summary>
    public static class AssemblyExtensions
    {
        #region Public Methods

        /// <summary>Get's the name of the assembly.</summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The assembly's name.</returns>
        public static string GetAssemblyName(this Assembly assembly)
        {
            return assembly.FullName.Remove(assembly.FullName.IndexOf(','));
        }

        #endregion
    }
}