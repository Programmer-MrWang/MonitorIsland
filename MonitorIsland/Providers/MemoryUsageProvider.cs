using ByteSizeLib;
using MonitorIsland.Abstractions;
using MonitorIsland.Attributes;
using MonitorIsland.Helpers;
using MonitorIsland.Models;
using System.Diagnostics;

namespace MonitorIsland.Providers
{
    [MonitorProviderInfo(
        "monitorisland.memoryusageprovider",
        "内存使用量",
        "显示已使用的内存量",
        [DisplayUnit.MB, DisplayUnit.GB, DisplayUnit.TB])]
    internal class MemoryUsageProvider() : MonitorProviderBase
    {
        public override string DefaultPrefix => "内存使用量：";

        private readonly ByteSize _totalMemory = ByteSize.FromBytes(MemoryHelper.GetTotalPhysicalMemory());

        private readonly PerformanceCounter _memoryCounter = new("Memory", "Available Bytes");

        public override string? GetData()
        {
            var availableMemory = ByteSize.FromBytes(_memoryCounter.NextValue());
            var usedMemory = _totalMemory - availableMemory;
            return SelectedUnit switch
            {
                DisplayUnit.MB => usedMemory.MebiBytes.ToString(),
                DisplayUnit.GB => usedMemory.GibiBytes.ToString(),
                DisplayUnit.TB => usedMemory.TebiBytes.ToString(),
                _ => null
            };
        }
    }
}
