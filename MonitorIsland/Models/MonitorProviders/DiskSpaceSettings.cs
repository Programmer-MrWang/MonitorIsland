using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MonitorIsland.Models.MonitorProviders
{
    public partial class DiskSpaceSettings : ObservableObject
    {
        [ObservableProperty]
        private string _driveName = "C:\\";

        [ObservableProperty]
        private ObservableCollection<string> _availableDriveNames = [];
    }
}
