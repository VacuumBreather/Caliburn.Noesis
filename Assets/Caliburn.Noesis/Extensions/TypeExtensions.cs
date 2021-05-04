namespace Caliburn.Noesis.Extensions
{
    #region Using Directives

    using System;

    #endregion

    /// <summary>Provides extension methods for the <see cref="Type" /> type.</summary>
    public static class TypeExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Determines whether an instance of this type can be assigned to a variable of the specified
        ///     type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <param name="baseType">The base type.</param>
        /// <returns>
        ///     <c>true</c> if any of the following conditions is true:
        ///     <list type="bullet">
        ///         <item>The current type and <paramref name="baseType" /> represent the same type.</item>
        ///         <item>
        ///             The current type is derived either directly or indirectly from the
        ///             <paramref name="baseType" />. The current type is derived directly from the
        ///             <paramref name="baseType" /> if it inherits from the <paramref name="baseType" /> The
        ///             current type  is derived indirectly from the <paramref name="baseType" /> if it
        ///             inherits from a succession of one or more classes that inherit from the
        ///             <paramref name="baseType" />.
        ///         </item>
        ///         <item>The <paramref name="baseType" /> is an interface that the current type implements.</item>
        ///         <item>
        ///             The current type is a generic type parameter, and the <paramref name="baseType" />
        ///             represents one of the constraints of the current type.
        ///         </item>
        ///         <item>
        ///             The current type represents a value type, and the <paramref name="baseType" />
        ///             represents Nullable&lt; currentType&gt;.
        ///         </item>
        ///     </list>
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDerivedFromOrImplements(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }

        #endregion
    }
}