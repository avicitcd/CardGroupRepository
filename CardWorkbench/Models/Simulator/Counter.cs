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
    [Serializable]
    [JsonObject]
    public class Counter
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

        //计数器编号
        [XmlElement("counternumber")]
        [DisplayName("计数器编号")]
        [Browsable(false)]
        [JsonProperty("counternumber")]
        public int counternumber { get; set; }

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

        //起始值
        [XmlElement("preset")]
        [DisplayName("起始值")]
        [JsonProperty("preset")]
        public short preset { get; set; }

        //终止值
        [XmlElement("limit")]
        [DisplayName("终止值")]
        [JsonProperty("limit")]
        public short limit { get; set; }
    }
}
