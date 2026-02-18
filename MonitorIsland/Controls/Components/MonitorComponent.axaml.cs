using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using Microsoft.Extensions.Logging;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using MonitorIsland.Models.ComponentSettings;
using System.ComponentModel;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace MonitorIsland.Controls.Components
{
    /// <summary>
    /// MonitorComponent.xaml 的交互逻辑
    /// </summary>
    [ComponentInfo(
        "AE533FE2-A53F-4104-8C38-37DA018A98BB",
        "监控",
        "\uEE21",
        "监控您电脑的各种信息"
    )]
    public partial class MonitorComponent : ComponentBase<MonitorComponentSettings>
    {
        private readonly DispatcherTimer _timer;
        private readonly IMonitorService MonitorService;

        public ILogger<MonitorComponent> Logger { get; }

        private List<MonitorProvider> MonitorProviders => IMonitorService.MonitorProviders;

        public MonitorComponent(ILogger<MonitorComponent> logger, IMonitorService monitorService)
        {
            Logger = logger;
            MonitorService = monitorService;
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Tick += OnTimer_Ticked;
        }

        private void OnTimer_Ticked(object? sender, EventArgs e)
        {
            UpdateMonitorData();
        }

        /// <summary>
        /// 更新监控数据
        /// </summary>
        private async void UpdateMonitorData()
        {
            if (Settings.SelectedProviderBase == null)
            {
                Logger.LogWarning("没有选择监控提供方");
                return;
            }
            var value = await MonitorService.GetDataFromProviderAsync(Settings.SelectedProviderBase);
            Settings.DisplayData = value ?? "N/A";
        }

        private void MonitorComponent_OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
            Settings.PropertyChanged += OnSettingsPropertyChanged;

            _timer.Start();
        }

        private void MonitorComponent_OnUnloaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            _timer.Stop();
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
        }

        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.RefreshInterval):
                    _timer.Interval = TimeSpan.FromMilliseconds(Settings.RefreshInterval);
                    break;
            }
        }
    }
}