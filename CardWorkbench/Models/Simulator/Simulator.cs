using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.Models
{
    /// <summary>
    /// 模拟器对象
    /// </summary>
    [JsonObject]
    public class Simulator
    {
        //模拟器ID
        [JsonProperty("simulatorID")]
        public string simulatorID { get; set; }

        //模拟器名称
        [JsonProperty("simulatorName")]
        public string simulatorName { get; set; }

        //模拟器状态
        public SimulatorStatus simulatorStatus { get; set; }
    }
}
