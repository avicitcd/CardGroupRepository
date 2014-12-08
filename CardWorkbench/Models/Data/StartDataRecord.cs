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
    /// 记录数据设置实体类
    /// </summary>
    [JsonObject]
    public class StartDataRecord
    {
        //记录包长度
        [DisplayName("记录包大小")]
        [JsonProperty("RecordLength")]
        public long RecordLength { get; set; }

        //文件存放地址
        [DisplayName("文件存放地址")]
        [Browsable(false)]
        [JsonProperty("FileName")]
        public string FileName { get; set; }
        
        //控制通道运行状态
        [JsonProperty("bControlRunState")]
        [DisplayName("控制通道运行状态")]
        public int bControlRunState { get; set; }

        //开启帧模式
        [JsonProperty("bFrameMode")]
        [DisplayName("开启帧模式")]
        public int bFrameMode { get; set; }

        //开启时间
        [JsonProperty("bEnableTime")]
        [DisplayName("输出时间字")]
        public int bEnableTime { get; set; }

        //开启状态
        [JsonProperty("bEnableStatus")]
        [DisplayName("输出状态字")]
        public int bEnableStatus { get; set; }
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum RECORDERMODEL
    {
        [EnumDisplayName("文件")]
        FILE,
        [EnumDisplayName("网络")]
        NETWORK
    };

}
