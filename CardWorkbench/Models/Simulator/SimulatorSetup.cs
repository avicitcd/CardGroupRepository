using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace CardWorkbench.Models
{
    [JsonObject]
    [Serializable]
    public class SimulatorSetup
    {
        //帧定义
        [JsonProperty("FormatCreate")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("帧定义")]
        [Description("Frame Definition")]
        public FormatCreate formatCreate { get; set; }

        //固定字设置
        [JsonProperty("FixedWord")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("固定字设置")]
        [Description("Fixed Word Setup")]
        public IList<FixedWord> fixedWordList { get; set; }

        //动态功能设置
        [JsonProperty("Waveform")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("波形设置")]
        [Description("Dynamic Function Setup")]
        public Waveform[] waveformList { get; set; }

        //计数器设置
        [JsonProperty("Counter")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName("计数器设置")]
        [Description("Counter Setup")]
        public Counter[] counterList { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public SimulatorSetup DeepClone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as SimulatorSetup;
            }
        }
    }

}
