using CardWorkbench.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CardWorkbench.Models
{
    [JsonObject]
    [Serializable]
    public class Waveform
    {
        //是否启用
        [DisplayName("是否启用")]
        [JsonIgnore]
        [XmlIgnore]
        public bool isEnable { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        [XmlElement("isEnableXml")]
        public string isEnableXml
        {
            get { return this.isEnable ? "1" : "0"; }
            set { this.isEnable = XmlConvert.ToBoolean(value); }
        }

        //波形器编号
        [XmlElement("waveformnumber")]
        [DisplayName("波形器编号")]
        [Browsable(false)]
        [JsonProperty("waveformnumber")]
        public int waveformnumber { get; set; }

        //字号
        [XmlElement("wordnumber")]
        [DisplayName("字号")]
        [JsonProperty("wordnumber")]
        public int wordnumber { get; set; }

        //字间隔
        [XmlElement("wordinterval")]
        [DisplayName("字间隔")]
        [JsonProperty("wordinterval")]
        public int wordinterval { get; set; }

        //频率
        [XmlElement("rate")]
        [DisplayName("频率")]
        [JsonProperty("rate")]
        public double rate { get; set; }

        //频率单位
        [JsonIgnore]
        [Browsable(false)]
        public RATEUNITS RATE_UNITS { get; set; }

        //波形
        [XmlElement("WAVE_FORM")]
        [DisplayName("波形")]
        public WAVEFORM WAVE_FORM { get; set; }

        //数据类型
        [XmlElement("DATATYPE")]
        [DisplayName("排列顺序")]
        public DATATYPE _DATATYPE { get; set; }
    }

    public enum RATEUNITS { Hz, KHz };

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum WAVEFORM
    {
        [EnumDisplayName("RAMP")]
        SsRamp,
        [EnumDisplayName("SINE")]
        SsSine,
        [EnumDisplayName("SQR")]
        SsSquareWave,
        [EnumDisplayName("TRI")]
        SsTriangle
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum DATATYPE
    {
        [EnumDisplayName("1CM")]
        SsOnesComp,
        [EnumDisplayName("2CM")]
        SsTwosComp,
        [EnumDisplayName("SMG")]
        SsSignMagnitude,
        [EnumDisplayName("OBN")]
        SsOffsetBinary 
    }
}
