using ClassIsland.Core.Abstractions.Controls;
using Microsoft.Extensions.Logging;
using MonitorIsland.Abstractions;
using MonitorIsland.Interfaces;
using MonitorIsland.Models;
using MonitorIsland.Models.ComponentSettings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RoutedEventArgs = Avalonia.Interactivity.RoutedEventArgs;

namespace MonitorIsland.Controls.Components
{
    /// <summary>
    /// MonitorComponentSettingsControl.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorComponentSettingsControl : ComponentBase<MonitorComponentSettings>
    {
        public ObservableCollection<CpuTemperatureSensorInfo> AvailableCpuSensors { get; } = [];
        public ILogger<MonitorComponentSettingsControl> Logger { get; }
        private List<MonitorProvider> MonitorProviders => IMonitorService.MonitorProviders;

        public MonitorComponentSettingsControl(ILogger<MonitorComponentSettingsControl> logger)
        {
            Logger = logger;
            InitializeComponent();
        }

        private void MonitorComponentSettingsControl_OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            Settings.PropertyChanged += OnSettingsPropertyChanged;
            // 如果没有选择提供方，选择第一个
            if (Settings.SelectedProvider is null && MonitorProviders.Count > 0)
            {
                Settings.SelectedProvider = MonitorProviders[0];
            }
        }

        private void MonitorComponentSettingsControl_OnUnloaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            Settings.PropertyChanged -= OnSettingsPropertyChanged;
        }

        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.SelectedProvider))
            {
                ChangeSelectedProvider();
            }
        }

        private void ChangeSelectedProvider()
        {
            if (Settings.SelectedProvider is null)
            {
                Unload();
                return;
            }

            // 与全局列表的同一引用时克隆一份，避免多个组件共享同一设置实例
            var selected = Settings.SelectedProvider;
            var template = IMonitorService.MonitorProviders.FirstOrDefault(p => p.Id == selected.Id);
            if (ReferenceEquals(selected, template))
            {
                var cloned = new MonitorProvider
                {
                    Id = selected.Id,
                    Name = selected.Name,
                    SelectedUnit = selected.SelectedUnit,
                    Settings = null // 让后续流程按类型创建独立的设置实例
                };

                Settings.PropertyChanged -= OnSettingsPropertyChanged;
                Settings.SelectedProvider = cloned;
                Settings.PropertyChanged += OnSettingsPropertyChanged;

                selected = cloned;
            }

            var providerInstance = MonitorProviderBase.GetInstance(selected);
            var baseType = providerInstance?.GetType().BaseType;
            if (baseType is not null
                && baseType.IsGenericType
                && baseType.GetGenericTypeDefinition() == typeof(MonitorProviderBase<>))
            {
                var settingsType = baseType.GetGenericArguments()[0];
                var settings = selected.Settings;
                if (settings?.GetType() != settingsType)
                {
                    Unload();
                    settings = Activator.CreateInstance(settingsType);
                }
                selected.Settings = settings;
            }
            else
            {
                Unload();
            }

            UpdateContent();
            var availableUnits = IMonitorService.MonitorProviderInfos[selected.Id].AvailableUnits;
            Settings.SelectedProviderBase = providerInstance;
            Settings.AvailableUnits = availableUnits?.ToList() ?? [];
            Settings.SelectedUnit = Settings.AvailableUnits.Count > 0 ? Settings.AvailableUnits[0] : null;
            Settings.DisplayPrefix = providerInstance?.DefaultPrefix ?? string.Empty;
        }

        private void UpdateContent()
        {
            var newControl = MonitorProviderControlBase.GetInstance(Settings.SelectedProvider);
            if (newControl != null)
            {
                ProviderSettingsControl.Content = newControl;
                Settings.ShowProviderSettingsControl = true;
            }
            else
            {
                ProviderSettingsControl.Content = null;
                Settings.ShowProviderSettingsControl = false;
            }
        }

        private void Unload()
        {
            ProviderSettingsControl.Content = null;
            Settings.ShowProviderSettingsControl = false;
        }
    }
}
