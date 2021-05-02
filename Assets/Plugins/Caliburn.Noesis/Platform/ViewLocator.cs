namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    ///     A strategy for determining which view to use for a given model.
    /// </summary>
    public static class ViewLocator
    {
        #region Constants and Fields

        private static readonly List<string> ViewSuffixList = new List<string>();

        private static string defaultSubNsViewModels;
        private static string defaultSubNsViews;
        private static string nameFormat;
        private static string viewModelSuffix;

        private static bool includeViewSuffixInVmNames;
        private static bool useNameSuffixesInMappings;

        #endregion

        #region Constructors and Destructors

        static ViewLocator()
        {
            ConfigureTypeMappings(new TypeMappingConfiguration());
        }

        #endregion

        #region Private Properties

        private static ILogger Logger { get; } = new UnityConsoleLogger(typeof(ViewLocator));

        private static NameTransformer NameTransformer { get; } = new NameTransformer();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds a default type mapping using the standard namespace mapping convention
        /// </summary>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public static void AddDefaultTypeMapping(string viewSuffix = "View")
        {
            if (!useNameSuffixesInMappings)
            {
                return;
            }

            // Check for <Namespace>.<BaseName><ViewSuffix> construct
            AddNamespaceMapping(string.Empty, string.Empty, viewSuffix);

            // Check for <Namespace>.ViewModels.<NameSpace>.<BaseName><ViewSuffix> construct
            AddSubNamespaceMapping(defaultSubNsViewModels, defaultSubNsViews, viewSuffix);
        }

        /// <summary>
        ///     Adds a standard type mapping based on simple namespace mapping
        /// </summary>
        /// <param name="nsSource">Namespace of source type</param>
        /// <param name="nsTargets">Namespaces of target type as an array</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public static void AddNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View")
        {
            // need to terminate with "." in order to concatenate with type name later
            var nsencoded = RegExHelper.NamespaceToRegEx(nsSource + ".");

            // Start pattern search from beginning of string ("^")
            // unless original string was blank (i.e. special case to indicate "append target to source")
            if (!string.IsNullOrEmpty(nsSource))
            {
                nsencoded = "^" + nsencoded;
            }

            // Capture namespace as "origns" in case we need to use it in the output in the future
            var nsreplace = RegExHelper.GetCaptureGroup("origns", nsencoded);

            var nsTargetsRegEx = nsTargets.Select(t => t + ".").ToArray();
            AddTypeMapping(nsreplace, null, nsTargetsRegEx, viewSuffix);
        }

        /// <summary>
        ///     Adds a standard type mapping based on simple namespace mapping
        /// </summary>
        /// <param name="nsSource">Namespace of source type</param>
        /// <param name="nsTarget">Namespace of target type</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public static void AddNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View")
        {
            AddNamespaceMapping(
                nsSource,
                new[]
                    {
                        nsTarget
                    },
                viewSuffix);
        }

        /// <summary>
        ///     Adds a standard type mapping by substituting one subnamespace for another
        /// </summary>
        /// <param name="nsSource">Subnamespace of source type</param>
        /// <param name="nsTargets">Subnamespaces of target type as an array</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public static void AddSubNamespaceMapping(string nsSource, string[] nsTargets, string viewSuffix = "View")
        {
            // need to terminate with "." in order to concatenate with type name later
            var nsencoded = RegExHelper.NamespaceToRegEx(nsSource + ".");

            string rxbeforetgt, rxaftersrc, rxaftertgt;
            var rxbeforesrc = rxbeforetgt = rxaftersrc = rxaftertgt = string.Empty;

            if (!string.IsNullOrEmpty(nsSource))
            {
                if (!nsSource.StartsWith("*"))
                {
                    rxbeforesrc = RegExHelper.GetNamespaceCaptureGroup("nsbefore");
                    rxbeforetgt = @"${nsbefore}";
                }

                if (!nsSource.EndsWith("*"))
                {
                    rxaftersrc = RegExHelper.GetNamespaceCaptureGroup("nsafter");
                    rxaftertgt = "${nsafter}";
                }
            }

            var rxmid = RegExHelper.GetCaptureGroup("subns", nsencoded);
            var nsreplace = string.Concat(rxbeforesrc, rxmid, rxaftersrc);

            var nsTargetsRegEx = nsTargets.Select(t => string.Concat(rxbeforetgt, t, ".", rxaftertgt)).ToArray();
            AddTypeMapping(nsreplace, null, nsTargetsRegEx, viewSuffix);
        }

        /// <summary>
        ///     Adds a standard type mapping by substituting one subnamespace for another
        /// </summary>
        /// <param name="nsSource">Subnamespace of source type</param>
        /// <param name="nsTarget">Subnamespace of target type</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public static void AddSubNamespaceMapping(string nsSource, string nsTarget, string viewSuffix = "View")
        {
            AddSubNamespaceMapping(
                nsSource,
                new[]
                    {
                        nsTarget
                    },
                viewSuffix);
        }

        /// <summary>
        ///     Adds a standard type mapping based on namespace RegEx replace and filter patterns
        /// </summary>
        /// <param name="nsSourceReplaceRegEx">RegEx replace pattern for source namespace</param>
        /// <param name="nsSourceFilterRegEx">RegEx filter pattern for source namespace</param>
        /// <param name="nsTargetsRegEx">Array of RegEx replace values for target namespaces</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public static void AddTypeMapping(
            string nsSourceReplaceRegEx,
            string nsSourceFilterRegEx,
            string[] nsTargetsRegEx,
            string viewSuffix = "View")
        {
            RegisterViewSuffix(viewSuffix);

            var replist = new List<string>();
            var repsuffix = useNameSuffixesInMappings ? viewSuffix : string.Empty;
            const string basegrp = "${basename}";

            foreach (var t in nsTargetsRegEx)
            {
                replist.Add(t + string.Format(nameFormat, basegrp, repsuffix));
            }

            var rxbase = RegExHelper.GetNameCaptureGroup("basename");
            var suffix = string.Empty;

            if (useNameSuffixesInMappings)
            {
                suffix = viewModelSuffix;

                if (!viewModelSuffix.Contains(viewSuffix) && includeViewSuffixInVmNames)
                {
                    suffix = viewSuffix + suffix;
                }
            }

            var rxsrcfilter = string.IsNullOrEmpty(nsSourceFilterRegEx)
                                  ? null
                                  : string.Concat(
                                      nsSourceFilterRegEx,
                                      string.Format(nameFormat, RegExHelper.NameRegEx, suffix),
                                      "$");

            var rxsuffix = RegExHelper.GetCaptureGroup("suffix", suffix);

            NameTransformer.AddRule(
                string.Concat(nsSourceReplaceRegEx, string.Format(nameFormat, rxbase, rxsuffix), "$"),
                replist.ToArray(),
                rxsrcfilter);
        }

        /// <summary>
        ///     Adds a standard type mapping based on namespace RegEx replace and filter patterns
        /// </summary>
        /// <param name="nsSourceReplaceRegEx">RegEx replace pattern for source namespace</param>
        /// <param name="nsSourceFilterRegEx">RegEx filter pattern for source namespace</param>
        /// <param name="nsTargetRegEx">RegEx replace value for target namespace</param>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View". (Optional)</param>
        public static void AddTypeMapping(
            string nsSourceReplaceRegEx,
            string nsSourceFilterRegEx,
            string nsTargetRegEx,
            string viewSuffix = "View")
        {
            AddTypeMapping(
                nsSourceReplaceRegEx,
                nsSourceFilterRegEx,
                new[]
                    {
                        nsTargetRegEx
                    },
                viewSuffix);
        }

        /// <summary>
        ///     Specifies how type mappings are created, including default type mappings. Calling this method will
        ///     clear all existing name transformation rules and create new default type mappings according to the
        ///     configuration.
        /// </summary>
        /// <param name="config">An instance of TypeMappingConfiguration that provides the settings for configuration</param>
        public static void ConfigureTypeMappings(TypeMappingConfiguration config)
        {
            if (string.IsNullOrEmpty(config.DefaultSubNamespaceForViews))
            {
                throw new ArgumentException("DefaultSubNamespaceForViews field cannot be blank.");
            }

            if (string.IsNullOrEmpty(config.DefaultSubNamespaceForViewModels))
            {
                throw new ArgumentException("DefaultSubNamespaceForViewModels field cannot be blank.");
            }

            if (string.IsNullOrEmpty(config.NameFormat))
            {
                throw new ArgumentException("NameFormat field cannot be blank.");
            }

            NameTransformer.Clear();
            ViewSuffixList.Clear();

            defaultSubNsViews = config.DefaultSubNamespaceForViews;
            defaultSubNsViewModels = config.DefaultSubNamespaceForViewModels;
            nameFormat = config.NameFormat;
            useNameSuffixesInMappings = config.UseNameSuffixesInMappings;
            viewModelSuffix = config.ViewModelSuffix;
            ViewSuffixList.AddRange(config.ViewSuffixList);
            includeViewSuffixInVmNames = config.IncludeViewSuffixInViewModelNames;

            SetAllDefaults();
        }

        /// <summary>
        ///     Locates the view type based on the specified model type.
        /// </summary>
        /// <param name="modelType">The model type.</param>
        /// <returns>The view type.</returns>
        public static Type LocateTypeForModelType(Type modelType)
        {
            var viewTypeName = modelType.FullName;

            viewTypeName = viewTypeName.Substring(
                0,
                viewTypeName.IndexOf('`') < 0 ? viewTypeName.Length : viewTypeName.IndexOf('`'));

            var viewTypeList = TransformName(viewTypeName).ToList();
            var viewType = AssemblySource.FindTypeByNames(viewTypeList);

            if (viewType == null)
            {
                Logger.Warn("View not found. Searched: {0}.", string.Join(", ", viewTypeList.ToArray()));
            }

            return viewType;
        }

        /// <summary>
        ///     This method registers a View suffix or synonym so that View Context resolution works properly.
        ///     It is automatically called internally when calling AddNamespaceMapping(), AddDefaultTypeMapping(),
        ///     or AddTypeMapping(). It should not need to be called explicitly unless a rule that handles synonyms
        ///     is added directly through the NameTransformer.
        /// </summary>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View".</param>
        public static void RegisterViewSuffix(string viewSuffix)
        {
            if (ViewSuffixList.Count(s => s == viewSuffix) == 0)
            {
                ViewSuffixList.Add(viewSuffix);
            }
        }

        /// <summary>
        ///     Transforms a ViewModel type name into all of its possible View type names.
        /// </summary>
        /// <param name="typeName">The name of the ViewModel type being resolved to its companion View.</param>
        /// <returns>Enumeration of transformed names</returns>
        public static IEnumerable<string> TransformName(string typeName)
        {
            return NameTransformer.Transform(typeName);
        }

        #endregion

        #region Private Methods

        private static void SetAllDefaults()
        {
            if (useNameSuffixesInMappings)
            {
                // Add support for all view suffixes
                ViewSuffixList.ForEach(AddDefaultTypeMapping);
            }
            else
            {
                AddSubNamespaceMapping(defaultSubNsViewModels, defaultSubNsViews);
            }
        }

        #endregion
    }
}