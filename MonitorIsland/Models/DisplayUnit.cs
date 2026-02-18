using System.ComponentModel;

namespace MonitorIsland.Models
{
    /// <summary>
    /// œ‘ æµ•Œª
    /// </summary>
    public enum DisplayUnit
    {
        [Description("MB")]
        MB,

        [Description("GB")]
        GB,

        [Description("TB")]
        TB,

        [Description("%")]
        Percent,

        [Description("°„C")]
        Celsius
    }
}
