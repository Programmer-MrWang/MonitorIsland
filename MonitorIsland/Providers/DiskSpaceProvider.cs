using ByteSizeLib;
using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Models;
using MonitorIsland.Models.MonitorProviderSettings;

namespace MonitorIsland.Providers
{
    /// <summary>
    /// 磁盘剩余空间监控提供方
    /// </summary>
    [MonitorProviderInfo(
        "monitorisland.diskspace",
        "磁盘剩余空间",
        "监控磁盘剩余空间",
        [DisplayUnit.GB, DisplayUnit.TB, DisplayUnit.MB])]
    public class DiskSpaceProvider : MonitorProviderBase<DiskSpaceSettings>
    {
        public override string DefaultPrefix => "磁盘: ";

        public override string? GetData()
        {
            var driveName = Settings.DriveName ?? "C:\\";
            var drive = new DriveInfo(driveName);
            if (!drive.IsReady)
                return null;
            var freeSpace = ByteSize.FromBytes(drive.TotalFreeSpace);
            return SelectedUnit switch
            {
                DisplayUnit.MB => freeSpace.MebiBytes.ToString(),
                DisplayUnit.GB => freeSpace.GibiBytes.ToString(),
                DisplayUnit.TB => freeSpace.TebiBytes.ToString(),
                _ => null
            };
        }
    }
}
