namespace Caliburn.Noesis
{
    /// <summary>
    ///     Helper class for encoding strings to regular expression patterns.
    /// </summary>
    public static class RegExHelper
    {
        #region Constants and Fields

        /// <summary>
        ///     The regular expression pattern for a valid name.
        /// </summary>
        public const string NameRegEx =
            @"[\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}_][\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}_]*";

        /// <summary>
        ///     The regular expression pattern for a namespace or namespace fragment.
        /// </summary>
        public const string NamespaceRegEx = "(" + SubNamespaceRegEx + ")*";

        /// <summary>
        ///     The regular expression pattern for a sub-namespace (including the dot).
        /// </summary>
        public const string SubNamespaceRegEx = NameRegEx + @"\.";

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a named capture group with the specified regular expression.
        /// </summary>
        /// <param name="groupName">The name of the capture group to create.</param>
        /// <param name="regEx">The regular expression pattern to capture.</param>
        /// <returns>A regular expression capture group with the specified group name.</returns>
        public static string GetCaptureGroup(string groupName, string regEx)
        {
            return string.Concat(
                @"(?<",
                groupName,
                ">",
                regEx,
                ")");
        }

        /// <summary>
        ///     Creates a capture group for a valid name regular expression pattern.
        /// </summary>
        /// <param name="groupName">The name of the capture group to create.</param>
        /// <returns>A regular expression capture group with the specified group name.</returns>
        public static string GetNameCaptureGroup(string groupName)
        {
            return GetCaptureGroup(groupName, NameRegEx);
        }

        /// <summary>
        ///     Creates a capture group for a namespace regular expression pattern.
        /// </summary>
        /// <param name="groupName">The name of capture group to create.</param>
        /// <returns>A regular expression capture group with the specified group name.</returns>
        public static string GetNamespaceCaptureGroup(string groupName)
        {
            return GetCaptureGroup(groupName, NamespaceRegEx);
        }

        /// <summary>
        ///     Converts a namespace (including wildcards) to a regular expression string.
        /// </summary>
        /// <param name="sourceNamespace">Source namespace to convert to regular expression.</param>
        /// <returns>A namespace converted to a regular expression.</returns>
        public static string NamespaceToRegEx(string sourceNamespace)
        {
            // We need to escape the "." as it's a special character in regular expression syntax.
            var encodedNamespace = sourceNamespace.Replace(".", @"\.");

            // We replace the "*" wildcard with regular expression syntax.
            encodedNamespace = encodedNamespace.Replace(@"*\.", NamespaceRegEx);

            return encodedNamespace;
        }

        #endregion
    }
}