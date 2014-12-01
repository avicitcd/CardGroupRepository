using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.Models
{
    /// <summary>
    /// 通道状态
    /// </summary>
    [JsonObject]
    public class ChannelStatus
    {
        //开启状态
        [JsonProperty("bRun")]
        public int bRun { get; set; }

        //同步模式错误数
        [JsonProperty("ErrorsInSyncPattern")]
        public int ErrorsInSyncPattern { get; set; }

        //同步模式是否超过16个错误
        [JsonProperty("bMoreThan16ErrorsInSyncPattern")]
        public int bMoreThan16ErrorsInSyncPattern { get; set; }

        //输入是否反向
        [JsonProperty("bInputInverted")]
        public int bInputInverted { get; set; }

        //是否丢失输入时钟
        [JsonProperty("bLostInputClock")]
        public int bLostInputClock { get; set; }

        //位滑是否修正
        [JsonProperty("bBitSlipCorrected")]
        public int bBitSlipCorrected { get; set; }

        //同步模式是否ok
        [JsonProperty("bSyncPatternOk")]
        public int bSyncPatternOk { get; set; }

        //帧同步状态
        [JsonConverter(typeof(StringEnumConverter))]
        public MCFSSYNCSTATUS MCFS_SYNC_STATUS { get; set; }

        public enum MCFSSYNCSTATUS { SEARCH, VERIFY, LOCK };
    }
}
