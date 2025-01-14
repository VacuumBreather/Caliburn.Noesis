using System.Collections.Generic;
using System.Reflection;
using Caliburn.Noesis;
using Testing;
using UnityEngine;

public class Bootstrapper : Bootstrapper<ShellViewModel>
{
    static Bootstrapper()
    {
        LogManager.GetLog = type => new DebugLog(type);
    }

    protected override IEnumerable<Assembly> SelectAssemblies()
    {
        return new[] { GetType().Assembly };
    }
}
