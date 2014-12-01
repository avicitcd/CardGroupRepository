using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.Models
{
    /// <summary>
    /// 模拟器状态
    /// </summary>
    [JsonObject]
    public class SimulatorStatus
    {
        //帧同步状态
        [JsonConverter(typeof(StringEnumConverter))]
        public SIMULATORSTATUS SIMULATOR_STATUS { get; set; }

        public enum SIMULATORSTATUS { IDLE, RUNA, RUNB, INSERTFRAME };
    }
}
