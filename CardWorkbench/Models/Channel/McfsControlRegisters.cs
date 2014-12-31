using CardWorkbench.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CardWorkbench.Models
{
    [JsonObject]
    [XmlRoot("McfsControlRegisters")]
    public class McfsControlRegisters
    {
        [XmlElement("ControlRegister")]
        [JsonProperty("ControlRegister")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName(McfsControlRegistersConstants.controlRegister)]
        [Description("Channel Control Registers")]
        public ControlRegister ControlRegister { get; set; }

        [XmlElement("FrameStrategyModeControlsRegister")]
        [JsonProperty("FrameStrategyModeControlsRegister")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName(McfsControlRegistersConstants.frameStrategyModeControlsRegister)]
        [Description("Frame Strategy Mode Control Register")]
        public FrameStrategyModeControlsRegister FrameStrategyModeControlsRegister { get; set; }

        [XmlElement("FrameSyncStrategyRegister")]
        [JsonProperty("FrameSyncStrategyRegister")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName(McfsControlRegistersConstants.frameSyncStrategyRegister)]
        [Description("Frame Sync Strategy Register")]
        public FrameSyncStrategyRegister FrameSyncStrategyRegister { get; set; }

        [XmlElement("SyncPatternRegisters")]
        [JsonProperty("SyncPatternRegisters")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DisplayName(McfsControlRegistersConstants.syncPatternRegisters)]
        [Description("Sync Pattern&Mask control Registers")]
        public SyncPatternRegisters SyncPatternRegisters { get; set; }

        [XmlElement("InitializeWordProperties")]
        [JsonIgnore]
        [Browsable(false)]
        public InitializeWordProperties initializeWordProperties { get; set; }
    }

     public class McfsControlRegistersConstants {
       public  const string controlRegister = "通道";
       public  const string frameStrategyModeControlsRegister = "帧属性";
       public  const string frameSyncStrategyRegister = "帧同步";
       public  const string syncPatternRegisters = "同步字";
     }
}
