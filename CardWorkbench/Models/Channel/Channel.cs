using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.Models
{
    /// <summary>
    /// 通道对象
    /// </summary>
    [JsonObject]
    public class Channel
    {
        //通道ID
        [JsonProperty("channelID")]
        public string channelID { get; set; }

        //通道名称
        [JsonProperty("channelName")]
        public string channelName { get; set; }

        //通道状态
        public ChannelStatus channelStatus { get; set; }

    }
}
