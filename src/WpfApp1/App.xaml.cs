using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using System;
using System.IO;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IHost _host;

        public App()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("serilog.json")
            .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo
                        .Map(
                        "app", configuration.GetSection("Serilog:Properties").GetValue<string>("app"),
                        (name, m) => m.Map(e => e,
                            (v, wt) => wt.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"logs\{name}\{v.Timestamp:yyyyMM}\{v.Timestamp:yyyyMMdd}-{name}.txt"))), sinkMapCountLimit: 0)
                .CreateLogger();

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services=> 
                {
                    services.AddSingleton<MainWindow>();
                })
                .UseSerilog()
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            _host.Services.GetService<MainWindow>()?.Show();
            //new MainWindow().Show();
        }
    }
}
