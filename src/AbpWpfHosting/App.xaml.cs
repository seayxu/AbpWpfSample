using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using System;
using System.IO;
using System.Threading;
using System.Windows;

using Volo.Abp;

namespace AbpWpfHosting
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        private readonly IAbpApplicationWithExternalServiceProvider _application;
        Mutex mutex;

        public App()
        {
            mutex = new Mutex(true, "appdemo", out bool createdNew);

            if (createdNew)
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

                _host = Host
                    .CreateDefaultBuilder(null)
                    .UseAutofac()
                    .UseSerilog()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddApplication<AbpWpfHostingModule>();
                    }).Build();
                _application = _host.Services.GetService<IAbpApplicationWithExternalServiceProvider>();
            }
            else
            {
                MessageBox.Show("App is running . . .");
                Thread.Sleep(1000);
                Environment.Exit(1);
            }
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Log.Information("Starting WPF host.");
                await _host.StartAsync();
                Initialize(_host.Services);

                _host.Services.GetService<MainWindow>()?.Show();
                //new MainWindow().Show();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        protected async override void OnExit(ExitEventArgs e)
        {
            _application.Shutdown();
            await _host.StopAsync();
            _host.Dispose();
        }

        private void Initialize(IServiceProvider serviceProvider)
        {
            _application.Initialize(serviceProvider);
        }
    }
}
