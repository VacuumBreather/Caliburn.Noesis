using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Caliburn.Noesis
{
    /// <summary>
    /// A source of assemblies that are inspectable by the framework.
    /// </summary>
    public class AssemblySource : BindableCollection<Assembly>
    {
        public AssemblySource()
        {
            FindTypeByNames = InternalFindTypeByNames;
        }

        /// <summary>
        /// Adds a collection of assemblies to AssemblySource
        /// </summary>
        /// <param name="assemblies">The assemblies to add</param>
        public override void AddRange(IEnumerable<Assembly> assemblies)
        {
            foreach(var assembly in assemblies)
            {
                try
                {
                    if (!Contains(assembly))
                        Add(assembly);
                }
                catch (ArgumentException)
                {
                    // ignore
                }
            }
        }

        /// <summary>
        /// Finds a type which matches one of the elements in the sequence of names.
        /// </summary>
        public Func<IEnumerable<string>, Type> FindTypeByNames { get; set; }
            
        public Type InternalFindTypeByNames(IEnumerable<string> names)
        {
            if (names == null)
            {
                return null;
            }

            var type = names
                .Join(this.SelectMany(a => a.ExportedTypes), n => n, t => t.FullName, (n, t) => t)
                .FirstOrDefault();
            return type;
        }
    }

    /// <summary>
    /// A caching subsystem for <see cref="AssemblySource"/>.
    /// </summary>
    public class AssemblySourceCache
    {
        private bool isInstalled;
        private readonly IDictionary<String, Type> TypeNameCache = new Dictionary<string, Type>();

        public AssemblySourceCache()
        {
            Install = InternalInstall;
        }

        /// <summary>
        /// Extracts the types from the spezified assembly for storing in the cache.
        /// </summary>
        public Func<Assembly, IEnumerable<Type>> ExtractTypes = assembly =>
            assembly.ExportedTypes
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => t != typeof(AssemblySource))
                .Where(t =>
                    typeof(INotifyPropertyChanged).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()));

        /// <summary>
        /// Installs the caching subsystem.
        /// </summary>
        public System.Action<AssemblySource> Install { get; set; }
            
        public void InternalInstall(AssemblySource assemblySource)
        {
            if (isInstalled)
            {
                return;
            }

            isInstalled = true;

            assemblySource.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        e.NewItems.OfType<Assembly>()
                            .SelectMany(a => ExtractTypes(a))
                            .Apply(AddTypeAssembly);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Reset:
                        TypeNameCache.Clear();
                        assemblySource
                            .SelectMany(a => ExtractTypes(a))
                            .Apply(AddTypeAssembly);
                        break;
                }
            };

            assemblySource.Refresh();

            assemblySource.FindTypeByNames = names =>
            {
                if (names == null)
                {
                    return null;
                }

                var type = names.Select(n => TypeNameCache.GetValueOrDefault(n)).FirstOrDefault(t => t != null);
                return type;
            };
        }

        private void AddTypeAssembly(Type type)
        {
            if (!TypeNameCache.ContainsKey(type.FullName))
            {
                TypeNameCache.Add(type.FullName, type);
            }
        }
    }
}
