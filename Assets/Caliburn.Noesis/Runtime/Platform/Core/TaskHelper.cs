using System;
using System.Threading;
#if !UNITY_5_5_OR_NEWER
using System.Threading.Tasks;
#endif
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    public static class TaskHelper
    {
        public static async UniTask Delay(TimeSpan delay, CancellationToken cancellationToken = default)
        {
#if UNITY_5_5_OR_NEWER
            await UniTask.Delay(delay, cancellationToken: cancellationToken);
#else
            await Task.Delay(delay, cancellationToken);
#endif
        }
        public static async UniTask Delay(int millisecondsDelay, CancellationToken cancellationToken = default)
        {
#if UNITY_5_5_OR_NEWER
            await UniTask.Delay(millisecondsDelay, cancellationToken: cancellationToken);
#else
            await Task.Delay(millisecondsDelay, cancellationToken);
#endif
        }
    }
}
