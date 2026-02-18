using CommunityToolkit.Mvvm.ComponentModel;
using MonitorIsland.Abstractions;
using System.Text.Json.Serialization;

namespace MonitorIsland.Models.ComponentSettings
{
    public partial class MonitorComponentSettings : ObservableObject
    {
        [ObservableProperty]
        [property: JsonIgnore]
        private string _displayData = string.Empty;

        [ObservableProperty]
        private string _displayPrefix = string.Empty;

        [ObservableProperty]
        private MonitorProvider? _selectedProvider = null;

        [ObservableProperty]
        private MonitorProviderBase? _selectedProviderBase = null;

        [ObservableProperty]
        private DisplayUnit? _selectedUnit;

        [ObservableProperty]
        private List<DisplayUnit> _availableUnits = [];

        [ObservableProperty]
        private bool _showProviderSettingsControl = false;

        private int _refreshInterval = 1000;
        /// <summary>
        /// 刷新间隔（毫秒）
        /// </summary>
        public int RefreshInterval
        {
            get => _refreshInterval;
            set
            {
                if (value == _refreshInterval) return;
                _refreshInterval = Math.Max(250, value);
                OnPropertyChanged();
            }
        }

        partial void OnSelectedUnitChanged(DisplayUnit? value)
        {
            if (SelectedProviderBase != null)
            {
                SelectedProviderBase.SelectedUnit = value;
            }
        }
    }
}
