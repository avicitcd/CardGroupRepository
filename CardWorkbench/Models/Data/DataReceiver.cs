using CardWorkbench.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CardWorkbench.Models.Data
{
    /// <summary>
    /// 网络数据接收类
    /// </summary>
    [JsonObject]
    public class DataReceiver
    {
        //ip地址
        [JsonProperty("addr")]
        public string addr { get; set; }

        //端口
        [JsonProperty("port")]
        public int port { get; set; }

        //ID字位置
        [JsonProperty("IDPosition")]
        public long IDPosition { get; set; }

        //帧长
        [JsonProperty("frameLength")]
        public int frameLength { get; set; }

        //字长
        [JsonProperty("wordSize")]
        public int wordSize { get; set; }

        //是否有时间标签
        [JsonProperty("bEnableTime")]
        public int bEnableTime { get; set; }

        //是否有状态标签
        [JsonProperty("bEnableStatus")]
        public int bEnableStatus { get; set; }
    }

}
