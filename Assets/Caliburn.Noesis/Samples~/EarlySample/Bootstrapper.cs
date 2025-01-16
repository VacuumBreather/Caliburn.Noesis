using System;
using System.Collections.Generic;
using System.Reflection;
using Caliburn.Noesis;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis.Samples.EarlySample
{
    public class Bootstrapper : BootstrapperBase
    {
        static Bootstrapper()
        {
            LogManager.GetLog = type => new DebugLog(type);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { GetType().Assembly };
        }

        protected override async UniTask OnStartupAsync()
        {
            await DisplayRootViewForAsync<ShellViewModel>();
        }
    }
}
