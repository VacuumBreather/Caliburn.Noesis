#if UNITY_5_5_OR_NEWER
using global::Noesis;
#else
using System.Windows;
#endif

namespace Caliburn.Noesis
{
    public static class CoroutineExecutionContextExtensions
    {
        public static IServiceLocator GetServiceLocator(this CoroutineExecutionContext context)
        {
            IServiceLocator serviceLocator = null;

            if (context.Source is DependencyObject dependencyObject)
            {
                serviceLocator = dependencyObject.GetServiceLocator();
            }

            return serviceLocator;
        }
    }
}
