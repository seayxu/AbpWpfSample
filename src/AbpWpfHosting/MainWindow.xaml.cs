using Microsoft.Extensions.Logging;

using Serilog;

using System;
using System.Windows;

using Volo.Abp.DependencyInjection;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace AbpWpfHosting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ISingletonDependency
    {
        ILogger Logger;

        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(ILogger<MainWindow> logger):this()
        {
            Logger = logger;
        }

        private void log_Click(object sender, RoutedEventArgs e)
        {
            string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}";
            Log.Debug(log);
            Log.Information(log);
            Log.Warning(log);
            Log.Error(log);
            Log.Fatal(log);

            Logger?.LogDebug(log);
            Logger?.LogInformation(log);
            Logger?.LogWarning(log);
            Logger?.LogError(log);
            Logger?.LogCritical(log);
        }
    }
}
