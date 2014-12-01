using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardWorkbench.Models
{
    [JsonObject]
    public class Device
    {
        //设备ID
        [JsonProperty("deviceID")]
        public string deviceID { get; set; }

        //设备型号
        [JsonProperty("deviceModel")]
        public string deviceModel { get; set; }

        //设备描述
        public string deviceDescription { get; set; }

        //设备通道
        [JsonProperty("channelList")]
        public IList<Channel> channelList { get; set; }

        //设备模拟器
        [JsonProperty("simulator")]
        public Simulator simulator { get; set; }
    }

  
}
