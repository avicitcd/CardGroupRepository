using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CardWorkbench.Models
{
    [Serializable]
    [JsonObject]
    public class Counter
    {
        //是否启用
        [DisplayName("是否启用")]
        [JsonIgnore]
        public bool isEnable { get; set; }

        //计数器编号
        [DisplayName("计数器编号")]
        [Browsable(false)]
        [JsonProperty("counternumber")]
        public int counternumber { get; set; }

        //字号
        [DisplayName("字号")]
        [JsonProperty("wordnumber")]
        public int wordnumber { get; set; }

        //字间隔
        [DisplayName("字间隔")]
        [JsonProperty("wordinterval")]
        public int wordinterval { get; set; }

        //起始值
        [DisplayName("起始值")]
        [JsonProperty("preset")]
        public short preset { get; set; }

        //终止值
        [DisplayName("终止值")]
        [JsonProperty("limit")]
        public short limit { get; set; }
    }
}
