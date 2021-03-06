namespace Caliburn.Noesis.Extensions
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;

    /// <summary>Provides extension methods for the <see cref="ILogger" /> type.</summary>
    [PublicAPI]
    public static class LoggerExtensions
    {
        #region Public Methods

        /// <summary>Gets a method call tracer which can be used in a using statement.</summary>
        /// <param name="logger">The logger to trace to.</param>
        /// <param name="callingMethodParamValues">The parameters of the method call.</param>
        /// <returns>An <see cref="IDisposable" /> to be used in a using statement.</returns>
        public static IDisposable GetMethodTracer(this ILogger logger, params object[] callingMethodParamValues)
        {
            return logger.IsEnabled(LogLevel.Trace) ? TraceMethod(logger, callingMethodParamValues) : Disposable.Empty;
        }

        /// <summary>Gets a method call tracer which can be used in a using statement.</summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <param name="logger">The logger to trace to.</param>
        /// <param name="param">The parameter of the method call.</param>
        /// <returns>An <see cref="IDisposable" /> to be used in a using statement.</returns>
        public static IDisposable GetMethodTracer<T>(this ILogger logger, T param)
        {
            return logger.IsEnabled(LogLevel.Trace) ? TraceMethod(logger, param) : Disposable.Empty;
        }

        /// <summary>Gets a method call tracer which can be used in a using statement.</summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <param name="logger">The logger to trace to.</param>
        /// <param name="param1">The first parameter of the method call.</param>
        /// <param name="param2">The second parameter of the method call.</param>
        /// <returns>An <see cref="IDisposable" /> to be used in a using statement.</returns>
        public static IDisposable GetMethodTracer<T1, T2>(this ILogger logger, T1 param1, T2 param2)
        {
            return logger.IsEnabled(LogLevel.Trace) ? TraceMethod(logger, param1, param2) : Disposable.Empty;
        }

        /// <summary>Gets a method call tracer which can be used in a using statement.</summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <param name="logger">The logger to trace to.</param>
        /// <param name="param1">The first parameter of the method call.</param>
        /// <param name="param2">The second parameter of the method call.</param>
        /// <param name="param3">The third parameter of the method call.</param>
        /// <returns>An <see cref="IDisposable" /> to be used in a using statement.</returns>
        public static IDisposable GetMethodTracer<T1, T2, T3>(this ILogger logger, T1 param1, T2 param2, T3 param3)
        {
            return logger.IsEnabled(LogLevel.Trace) ? TraceMethod(logger, param1, param2, param3) : Disposable.Empty;
        }

        /// <summary>Gets a method call tracer which can be used in a using statement.</summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the forth parameter.</typeparam>
        /// <param name="logger">The logger to trace to.</param>
        /// <param name="param1">The first parameter of the method call.</param>
        /// <param name="param2">The second parameter of the method call.</param>
        /// <param name="param3">The third parameter of the method call.</param>
        /// <param name="param4">The forth parameter of the method call.</param>
        /// <returns>An <see cref="IDisposable" /> to be used in a using statement.</returns>
        public static IDisposable GetMethodTracer<T1, T2, T3, T4>(this ILogger logger,
                                                                  T1 param1,
                                                                  T2 param2,
                                                                  T3 param3,
                                                                  T4 param4)
        {
            return logger.IsEnabled(LogLevel.Trace)
                       ? TraceMethod(
                           logger,
                           param1,
                           param2,
                           param3,
                           param4)
                       : Disposable.Empty;
        }

        /// <summary>Gets a method call tracer which can be used in a using statement.</summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the forth parameter.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter.</typeparam>
        /// <param name="logger">The logger to trace to.</param>
        /// <param name="param1">The first parameter of the method call.</param>
        /// <param name="param2">The second parameter of the method call.</param>
        /// <param name="param3">The third parameter of the method call.</param>
        /// <param name="param4">The forth parameter of the method call.</param>
        /// <param name="param5">The fifth parameter of the method call.</param>
        /// <returns>An <see cref="IDisposable" /> to be used in a using statement.</returns>
        public static IDisposable GetMethodTracer<T1, T2, T3, T4, T5>(this ILogger logger,
                                                                      T1 param1,
                                                                      T2 param2,
                                                                      T3 param3,
                                                                      T4 param4,
                                                                      T5 param5)
        {
            return logger.IsEnabled(LogLevel.Trace)
                       ? TraceMethod(
                           logger,
                           param1,
                           param2,
                           param3,
                           param4,
                           param5)
                       : Disposable.Empty;
        }

        #endregion

        #region Private Methods

        private static void ExtractCallerInfo(out string methodName,
                                              out ParameterInfo[] methodParams,
                                              out string calledType,
                                              out string callingMethodName,
                                              out string callingType)
        {
            var stackIndex = 1;
            MethodBase method;

            do
            {
                method = new StackFrame(stackIndex++).GetMethod();
            }
            while ((method != null) && ShouldBeIgnored(method));

            methodName = method?.GetTrimmedName();
            methodParams = method?.GetParameters();
            calledType = method?.ReflectedType?.Name ?? "<unknown type>";

            MethodBase methodCalledBy;

            do
            {
                methodCalledBy = new StackFrame(stackIndex++).GetMethod();
            }
            while ((methodCalledBy != null) && ShouldBeIgnored(methodCalledBy));

            callingMethodName = methodCalledBy?.GetTrimmedName();
            callingType = methodCalledBy?.ReflectedType?.Name ?? "<unknown type>";
        }

        private static string GetTrimmedName(this MethodBase method)
        {
            var index = method.Name.LastIndexOf('.') + 1;

            return (index < 0) || (index >= method.Name.Length) ? method.Name : method.Name.Substring(index);
        }

        private static bool ShouldBeIgnored(this MethodBase method)
        {
            return method.Name.Contains("MoveNext") ||
                   method.Name.Contains(nameof(ExtractCallerInfo)) ||
                   (method.DeclaringType?.Name.Contains(nameof(GetMethodTracer)) ?? false) ||
                   (method.DeclaringType?.Name.Contains(nameof(LoggerExtensions)) ?? false) ||
                   (method.DeclaringType?.Name.Contains("Logger") ?? false) ||
                   (method.DeclaringType?.Name.Contains("MethodBuilder") ?? false);
        }

        private static void TraceCallerInfo(ILogger logger,
                                            string methodName,
                                            ParameterInfo[] methodParams,
                                            string calledType,
                                            object[] methodParamValues,
                                            string callingMethod,
                                            string callingType)
        {
            if (methodParams is null || methodParamValues is null)
            {
                return;
            }

            static string TransformParameterValue(object parameterValue)
            {
                switch (parameterValue)
                {
                    case string str:
                        return "\"" + str + "\"";

                    case char c:
                        return "'" + c + "'";

                    case IEnumerable enumerable:
                        return "[" + string.Join(", ", enumerable.Cast<object>().Select(TransformParameterValue)) + "]";

                    case null:
                        return "null";

                    default:
                        return parameterValue.ToString();
                }
            }

            var paramValues = methodParamValues.Select(TransformParameterValue).ToArray();

            string parameterList;

            if (methodParams.Length == paramValues.Length)
            {
                var parameters = methodParams.Select(
                    param => param.IsOut ? $"out {param.Name}" : $"{param.Name}={paramValues[param.Position]}");

                parameterList = string.Join(", ", parameters);
            }
            else
            {
                parameterList = "/* Please update to pass in all parameters */";
            }

            logger.LogTrace(
                "{CallingType}.{CallingMethod}() -> {CalledType}.{Method}({Parameters})",
                callingType,
                callingMethod,
                calledType,
                methodName,
                parameterList);
        }

        private static IDisposable TraceMethod(ILogger logger, params object[] paramValues)
        {
            ExtractCallerInfo(
                out var methodName,
                out var @params,
                out var calledType,
                out var callingMethod,
                out var callingType);

            TraceCallerInfo(
                logger,
                methodName,
                @params,
                calledType,
                paramValues,
                callingMethod,
                callingType);

            return new DisposableAction(
                () =>
                    {
                        logger.LogTrace(
                            "{CallingType}.{CallingMethod}() <- {CalledType}.{Method}()",
                            callingType,
                            callingMethod,
                            calledType,
                            methodName);
                    });
        }

        #endregion
    }
}