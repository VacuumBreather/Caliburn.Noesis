﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// Interface for platform specific operations that need enlightenment.
    /// </summary>
    public interface IPlatformProvider
    {
        /// <summary>
        ///   Indicates whether or not the framework is in design-time mode.
        /// </summary>
        bool InDesignMode { get; }

        /// <summary>
        /// Whether or not classes should execute property change notications on the UI thread.
        /// </summary>
        bool PropertyChangeNotificationsOnUIThread { get; }

        /// <summary>
        ///   Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        void BeginOnUIThread(System.Action action);

        /// <summary>
        ///   Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name = "action">The action to execute.</param>
        UniTask OnUIThreadAsync(Func<UniTask> action);

        /// <summary>
        ///   Executes the action on the UI thread.
        /// </summary>
        /// <param name = "action">The action to execute.</param>
        void OnUIThread(System.Action action);

        /// <summary>
        /// Used to retrieve the root, non-framework-created view.
        /// </summary>
        /// <param name="view">The view to search.</param>
        /// <returns>The root element that was not created by the framework.</returns>
        /// <remarks>In certain instances the services create UI elements.
        /// For example, if you ask the window manager to show a UserControl as a dialog, it creates a window to host the UserControl in.
        /// The WindowManager marks that element as a framework-created element so that it can determine what it created vs. what was intended by the developer.
        /// Calling GetFirstNonGeneratedView allows the framework to discover what the original element was. 
        /// </remarks>
        object GetFirstNonGeneratedView(object view);

        /// <summary>
        /// Executes the handler the fist time the view is loaded.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="handler">The handler.</param>
        void ExecuteOnFirstLoad(object view, Action<object> handler);

        /// <summary>
        /// Executes the handler the next time the view's LayoutUpdated event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="handler">The handler.</param>
        void ExecuteOnLayoutUpdated(object view, Action<object> handler);
    }
}
