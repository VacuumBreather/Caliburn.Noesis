namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
    using NoesisApp;
    using TriggerBase = NoesisApp.TriggerBase;
#else
    using System.Windows;
    using Microsoft.Xaml.Behaviors;
    using TriggerBase = Microsoft.Xaml.Behaviors.TriggerBase;
#endif


    /// <summary>
    ///   Host's attached properties related to routed UI messaging.
    /// </summary>
    public static class Message
    {
        internal static readonly DependencyProperty HandlerProperty =
            DependencyPropertyHelper.RegisterAttached(
                "Handler",
                typeof(object),
                typeof(Message),
                null
                );

        static readonly ILog Log = LogManager.GetLog(typeof(Message));

        static readonly DependencyProperty MessageTriggersProperty =
            DependencyPropertyHelper.RegisterAttached(
                "MessageTriggers",
                typeof(TriggerBase[]),
                typeof(Message),
                null
                );

        /// <summary>
        ///   Places a message handler on this element.
        /// </summary>
        /// <param name="d"> The element. </param>
        /// <param name="value"> The message handler. </param>
        public static void SetHandler(DependencyObject d, object value)
        {
            Log.Info("Setting handler for {0} to {1}.", d, value);
            d.SetValue(HandlerProperty, value);
        }

        /// <summary>
        ///   Gets the message handler for this element.
        /// </summary>
        /// <param name="d"> The element. </param>
        /// <returns> The message handler. </returns>
        public static object GetHandler(DependencyObject d)
        {
            return d.GetValue(HandlerProperty);
        }

        /// <summary>
        ///   A property definition representing attached triggers and messages.
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyPropertyHelper.RegisterAttached(
                "Attach",
                typeof(string),
                typeof(Message),
                null, 
                OnAttachChanged
                );

        /// <summary>
        ///   Sets the attached triggers and messages.
        /// </summary>
        /// <param name="d"> The element to attach to. </param>
        /// <param name="attachText"> The parsable attachment text. </param>
        public static void SetAttach(DependencyObject d, string attachText)
        {
            d.SetValue(AttachProperty, attachText);
        }

        /// <summary>
        ///   Gets the attached triggers and messages.
        /// </summary>
        /// <param name="d"> The element that was attached to. </param>
        /// <returns> The parsable attachment text. </returns>
        public static string GetAttach(DependencyObject d)
        {
            return d.GetValue(AttachProperty) as string;
        }

        static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (object.ReferenceEquals(e.NewValue, e.OldValue))
            {
                return;
            }

            var messageTriggers = (TriggerBase[])d.GetValue(MessageTriggersProperty);

            var allTriggers = Interaction.GetTriggers(d);

            if (messageTriggers != null)
            {
                messageTriggers.Apply(x => allTriggers.Remove(x));
            }

            var newTriggers = Parser.Parse(d, e.NewValue as string).ToArray();
            newTriggers.Apply(trigger => allTriggers.Add(trigger));

            if (newTriggers.Length > 0)
            {
                d.SetValue(MessageTriggersProperty, newTriggers);
            }
            else
            {
                d.ClearValue(MessageTriggersProperty);
            }
        }
    }
}
