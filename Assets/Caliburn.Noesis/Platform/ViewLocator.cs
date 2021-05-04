namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>Responsible for mapping view-model types to their corresponding view types.</summary>
    public class ViewLocator
    {
        #region Constants and Fields

        private const string DefaultViewSuffix = "View";

        private readonly List<string> viewSuffixList = new List<string>();

        private string defaultSubNsViewModels;
        private string defaultSubNsViews;

        private bool includeViewSuffixInVmNames;
        private ILogger logger;
        private string nameFormat;
        private bool useNameSuffixesInMappings;
        private string viewModelSuffix;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ViewLocator" /> class.</summary>
        public ViewLocator()
        {
            ConfigureTypeMappings(new TypeMappingConfiguration());
        }

        #endregion

        #region Private Properties

        private ILogger Logger => this.logger ??= LogManager.GetLogger(this);

        private NameTransformer NameTransformer { get; } = new NameTransformer();

        #endregion

        #region Public Methods

        /// <summary>Adds a default type mapping using the standard namespace mapping convention.</summary>
        /// <param name="viewSuffix">(Optional) Suffix for type name. Should be "View" or synonym of "View".</param>
        public void AddDefaultTypeMapping(string viewSuffix = DefaultViewSuffix)
        {
            if (!this.useNameSuffixesInMappings)
            {
                return;
            }

            // Check for <Namespace>.<BaseName><ViewSuffix> construct.
            AddNamespaceMapping(string.Empty, string.Empty, viewSuffix);

            // Check for <Namespace>.ViewModels.<NameSpace>.<BaseName><ViewSuffix> construct.
            AddSubNamespaceMapping(this.defaultSubNsViewModels, this.defaultSubNsViews, viewSuffix);
        }

        /// <summary>Adds a standard type mapping based on simple namespace mapping.</summary>
        /// <param name="nsSource">Namespace of source type.</param>
        /// <param name="nsTargets">Namespaces of target type.</param>
        /// <param name="viewSuffix">(Optional) Suffix for type name. Should be "View" or synonym of "View".</param>
        public void AddNamespaceMapping(string nsSource,
                                        IEnumerable<string> nsTargets,
                                        string viewSuffix = DefaultViewSuffix)
        {
            // We need to terminate with "." in order to concatenate with type name later.
            var nsEncoded = RegExHelper.NamespaceToRegEx(nsSource + ".");

            // Start the pattern search from beginning of string ("^")
            // unless the original string was blank (i.e. special case to indicate "append target to source").
            if (!string.IsNullOrEmpty(nsSource))
            {
                nsEncoded = "^" + nsEncoded;
            }

            // Capture the namespace as "nsOrig" in case we need to use it in the output in the future.
            var nsReplace = RegExHelper.GetCaptureGroup("nsOrig", nsEncoded);
            var nsTargetsRegEx = nsTargets.Select(t => t + ".").ToArray();

            AddTypeMapping(nsReplace, null, nsTargetsRegEx, viewSuffix);
        }

        /// <summary>Adds a standard type mapping based on simple namespace mapping.</summary>
        /// <param name="nsSource">Namespace of source type.</param>
        /// <param name="nsTarget">Namespace of target type.</param>
        /// <param name="viewSuffix">(Optional) Suffix for type name. Should  be "View" or synonym of "View".</param>
        public void AddNamespaceMapping(string nsSource,
                                        string nsTarget,
                                        string viewSuffix = DefaultViewSuffix)
        {
            AddNamespaceMapping(
                nsSource,
                new[]
                    {
                        nsTarget
                    },
                viewSuffix);
        }

        /// <summary>Adds a standard type mapping by substituting one sub-namespace for another.</summary>
        /// <param name="nsSource">Sub-namespace of source type.</param>
        /// <param name="nsTargets">Sub-namespaces of target type.</param>
        /// <param name="viewSuffix">(Optional) Suffix for type name. Should  be "View" or synonym of "View".</param>
        public void AddSubNamespaceMapping(string nsSource,
                                           IEnumerable<string> nsTargets,
                                           string viewSuffix = DefaultViewSuffix)
        {
            // We need to terminate with "." in order to concatenate with type name later.
            var nsEncoded = RegExHelper.NamespaceToRegEx(nsSource + ".");

            var rxBeforeTgt = string.Empty;
            var rxAfterSrc = string.Empty;
            var rxAfterTgt = string.Empty;
            var rxBeforeSrc = string.Empty;

            if (!string.IsNullOrEmpty(nsSource))
            {
                if (!nsSource.StartsWith("*"))
                {
                    rxBeforeSrc = RegExHelper.GetNamespaceCaptureGroup("nsBefore");
                    rxBeforeTgt = @"${nsBefore}";
                }

                if (!nsSource.EndsWith("*"))
                {
                    rxAfterSrc = RegExHelper.GetNamespaceCaptureGroup("nsAfter");
                    rxAfterTgt = "${nsAfter}";
                }
            }

            var rxMid = RegExHelper.GetCaptureGroup("subNs", nsEncoded);
            var rxReplace = string.Concat(rxBeforeSrc, rxMid, rxAfterSrc);
            var nsTargetsRegEx = nsTargets
                                 .Select(t => string.Concat(rxBeforeTgt, t, ".", rxAfterTgt))
                                 .ToArray();

            AddTypeMapping(rxReplace, null, nsTargetsRegEx, viewSuffix);
        }

        /// <summary>Adds a standard type mapping by substituting one sub-namespace for another.</summary>
        /// <param name="nsSource">Sub-namespace of source type.</param>
        /// <param name="nsTarget">Sub-namespace of target type.</param>
        /// <param name="viewSuffix">(Optional) Suffix for type name. Should  be "View" or synonym of "View".</param>
        public void AddSubNamespaceMapping(string nsSource,
                                           string nsTarget,
                                           string viewSuffix = DefaultViewSuffix)
        {
            AddSubNamespaceMapping(
                nsSource,
                new[]
                    {
                        nsTarget
                    },
                viewSuffix);
        }

        /// <summary>Adds a standard type mapping based on namespace RegEx replace and filter patterns.</summary>
        /// <param name="nsSourceReplaceRegEx">RegEx replace pattern for source namespace.</param>
        /// <param name="nsSourceFilterRegEx">RegEx filter pattern for source namespace.</param>
        /// <param name="nsTargetsRegEx">Array of RegEx replace values for target namespaces.</param>
        /// <param name="viewSuffix">(Optional) Suffix for type name. Should  be "View" or synonym of "View".</param>
        public void AddTypeMapping(string nsSourceReplaceRegEx,
                                   string nsSourceFilterRegEx,
                                   IEnumerable<string> nsTargetsRegEx,
                                   string viewSuffix = DefaultViewSuffix)
        {
            RegisterViewSuffix(viewSuffix);

            var repSuffix = this.useNameSuffixesInMappings ? viewSuffix : string.Empty;

            const string BaseGroup = "${basename}";

            var rxBase = RegExHelper.GetNameCaptureGroup("basename");
            var suffix = string.Empty;

            if (this.useNameSuffixesInMappings)
            {
                suffix = this.viewModelSuffix;

                if (!this.viewModelSuffix.Contains(viewSuffix) && this.includeViewSuffixInVmNames)
                {
                    suffix = viewSuffix + suffix;
                }
            }

            var rxSourceFilter = string.IsNullOrEmpty(nsSourceFilterRegEx)
                                     ? null
                                     : string.Concat(
                                         nsSourceFilterRegEx,
                                         string.Format(
                                             this.nameFormat,
                                             RegExHelper.NameRegEx,
                                             suffix),
                                         "$");

            var rxSuffix = RegExHelper.GetCaptureGroup("suffix", suffix);

            NameTransformer.AddRule(
                string.Concat(
                    nsSourceReplaceRegEx,
                    string.Format(this.nameFormat, rxBase, rxSuffix),
                    "$"),
                nsTargetsRegEx.Select(t => t + string.Format(this.nameFormat, BaseGroup, repSuffix))
                              .ToArray(),
                rxSourceFilter);
        }

        /// <summary>Adds a standard type mapping based on namespace RegEx replace and filter patterns.</summary>
        /// <param name="nsSourceReplaceRegEx">RegEx replace pattern for source namespace.</param>
        /// <param name="nsSourceFilterRegEx">RegEx filter pattern for source namespace.</param>
        /// <param name="nsTargetRegEx">RegEx replace value for target namespace.</param>
        /// <param name="viewSuffix">(Optional) Suffix for type name. Should  be "View" or synonym of "View".</param>
        public void AddTypeMapping(string nsSourceReplaceRegEx,
                                   string nsSourceFilterRegEx,
                                   string nsTargetRegEx,
                                   string viewSuffix = DefaultViewSuffix)
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
        ///     Specifies how type mappings are created, including default type mappings. Calling this
        ///     method will clear all existing name transformation rules and create new default type mappings
        ///     according to the configuration.
        /// </summary>
        /// <param name="config">
        ///     An instance of TypeMappingConfiguration that provides the settings for
        ///     configuration
        /// </param>
        public void ConfigureTypeMappings(TypeMappingConfiguration config)
        {
            if (string.IsNullOrEmpty(config.DefaultSubNamespaceForViews))
            {
                throw new ArgumentException(
                    $"{config.DefaultSubNamespaceForViews} cannot be blank.");
            }

            if (string.IsNullOrEmpty(config.DefaultSubNamespaceForViewModels))
            {
                throw new ArgumentException(
                    $"{config.DefaultSubNamespaceForViewModels} cannot be blank.");
            }

            if (string.IsNullOrEmpty(config.NameFormat))
            {
                throw new ArgumentException($"{config.NameFormat} cannot be blank.");
            }

            NameTransformer.Clear();
            this.viewSuffixList.Clear();

            this.defaultSubNsViews = config.DefaultSubNamespaceForViews;
            this.defaultSubNsViewModels = config.DefaultSubNamespaceForViewModels;
            this.nameFormat = config.NameFormat;
            this.useNameSuffixesInMappings = config.UseNameSuffixesInMappings;
            this.viewModelSuffix = config.ViewModelSuffix;
            this.viewSuffixList.AddRange(config.ViewSuffixList);
            this.includeViewSuffixInVmNames = config.IncludeViewSuffixInViewModelNames;

            SetAllDefaults();
        }

        /// <summary>Locates the view type based on the specified model type.</summary>
        /// <param name="modelType">The model type.</param>
        /// <param name="assemblySource">The <see cref="AssemblySource" /> containing the relevant types.</param>
        /// <returns>The located view type or <c>null</c> if no such type could be found.</returns>
        public Type LocateTypeForModelType(Type modelType, AssemblySource assemblySource)
        {
            var modelTypeName = modelType.FullName;

            modelTypeName = modelTypeName?.Substring(
                0,
                modelTypeName.IndexOf('`') < 0 ? modelTypeName.Length : modelTypeName.IndexOf('`'));

            var viewTypeList = TransformName(modelTypeName).ToList();
            var viewType = assemblySource.FindTypeByNames(viewTypeList);

            if (viewType == null)
            {
                Logger.Info(
                    $"No view not found for {modelType.Name}. {(viewTypeList.Any() ? "Searched:" + string.Join(", ", viewTypeList.ToArray()) : "No mappings found")}.");
            }

            return viewType;
        }

        /// <summary>
        ///     This method registers a View suffix or synonym so that View Context resolution works
        ///     properly. It is automatically called internally when calling AddNamespaceMapping(),
        ///     AddDefaultTypeMapping(), or AddTypeMapping(). It should not need to be called explicitly unless
        ///     a rule that handles synonyms is added directly through the NameTransformer.
        /// </summary>
        /// <param name="viewSuffix">Suffix for type name. Should  be "View" or synonym of "View".</param>
        public void RegisterViewSuffix(string viewSuffix)
        {
            if (this.viewSuffixList.Count(s => s == viewSuffix) == 0)
            {
                this.viewSuffixList.Add(viewSuffix);
            }
        }

        /// <summary>Transforms a ViewModel type name into all of its possible View type names.</summary>
        /// <param name="typeName">The name of the ViewModel type being resolved to its companion View.</param>
        /// <returns>Enumeration of transformed names.</returns>
        public IEnumerable<string> TransformName(string typeName)
        {
            return NameTransformer.Transform(typeName);
        }

        #endregion

        #region Private Methods

        private void SetAllDefaults()
        {
            if (this.useNameSuffixesInMappings)
            {
                // Add support for all view suffixes
                this.viewSuffixList.ForEach(AddDefaultTypeMapping);
            }
            else
            {
                AddSubNamespaceMapping(this.defaultSubNsViewModels, this.defaultSubNsViews);
            }
        }

        #endregion
    }
}