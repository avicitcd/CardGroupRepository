using CardWorkbench.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CardWorkbench.Models
{
    [JsonObject]
    public class FrameStrategyModeControlsRegister
    {
        //位滑
        [XmlElement("MCFS_BIT_SLIP_WINDOW")]
        [DisplayName("位滑")]
        [Description("Bit Slip Window")]
        public MCFSBITSLIPWINDOW MCFS_BIT_SLIP_WINDOW { get; set; }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum MCFSBITSLIPWINDOW {
            [EnumDisplayName("1 Bit")]
            McfsWindow1Bit,
            [EnumDisplayName("3 Bit")]
            McfsWindow3Bit,
            [EnumDisplayName("5 Bit")]
            McfsWindow5Bit,
            [EnumDisplayName("7 Bit")]
            McfsWindow7Bit 
        };

        //输入极性
        [XmlElement("MCFS_INPUT_POLARITY")]
        [DisplayName("输入极性")]
        [Description("Input Polarity")]
        public MCFSINPUTPOLARITY MCFS_INPUT_POLARITY { get; set; }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum MCFSINPUTPOLARITY {
            [EnumDisplayName("Normal")]
            McfsPolarityNormal,
            [EnumDisplayName("Invert")]
            McfsPolarityInvert,
            [EnumDisplayName("Automatic")]
            McfsPolarityAuto,
            [EnumDisplayName("Automatic Invert")]
            McfsPolarityAutoInvert 
        };

        //同步模式
        [XmlElement("MCFS_SYNC_MODE")]
        [DisplayName("同步模式")]
        [Description("Sync Mode")]
        public MCFSSYNCMODE MCFS_SYNC_MODE { get; set; }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum MCFSSYNCMODE {
            [EnumDisplayName("Fixed")]
            McfsSyncModeFixed,
            [EnumDisplayName("Burst")]
            McfsSyncModeBurst,
            [EnumDisplayName("Adaptive")]
            McfsSyncModeAdaptive 
        };

        //同步模式格式
        [XmlElement("MCFS_SYNC_PATTERN_FORMAT")]
        [DisplayName("同步模式格式")]
        [Description("Sync Pattern Format")]
        public MCFSSYNCPATTERNFORMAT MCFS_SYNC_PATTERN_FORMAT { get; set; }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum MCFSSYNCPATTERNFORMAT {
            [EnumDisplayName("Normal")]
            McfsSyncPatternNormal,
            [EnumDisplayName("Alternate Complement")]
            McfsSyncPatternAltComp,
            [EnumDisplayName("Complement")]
            McfsSyncPatternComplement 
        };

        //变长帧位置
        [XmlElement("MCFS_VARIABLE_LENGTH_FRAME_POSITION")]
        [DisplayName("变长帧位置")]
        [Description("Word Synchronous Variable Length Frame")]
        public MCFSVARIABLELENGTHFRAMEPOSITION MCFS_VARIABLE_LENGTH_FRAME_POSITION { get; set; }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum MCFSVARIABLELENGTHFRAMEPOSITION {
            [EnumDisplayName("Random Frame")]
            McfsRandomFramePosition,
            [EnumDisplayName("WordSynchronous Frame")]
            McfsWordSynchronousFramePosition 
        };

        //同步字长度
        [XmlElement("McfsSyncPatternLength")]
        [JsonProperty("McfsSyncPatternLength")]
        [DisplayName("同步字长度")]
        [Description("Sync Pattern Length")]
        [Range(1, 64)]
        public int McfsSyncPatternLength { get; set; }
    }
}
