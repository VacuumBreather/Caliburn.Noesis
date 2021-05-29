namespace Caliburn.Noesis.HelloWorld
{
    using System.Windows;
    using Samples;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            LogConfigurator.SetMinimumLogLevel();
            base.OnStartup(e);
        }
    }
}
