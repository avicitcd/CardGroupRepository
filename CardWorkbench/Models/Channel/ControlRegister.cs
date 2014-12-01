using CardWorkbench.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CardWorkbench.Models
{
    [JsonObject]
    public class ControlRegister
    {
        //译码
        [DisplayName("译码")]
        [Description("Decode")]
        public MCFSDECODE MCFS_DECODE { get; set; }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum MCFSDECODE {
            [EnumDisplayName("NRZ-L")]
            McfsDecodeNrzl,
            [EnumDisplayName("NRZ-S")]
            McfsDecodeNrzs 
        };

        //输入时钟极性
        [DisplayName("输入时钟极性")]
        [Description("Input Clock Polarity")]
        public MCFSINPUTCLOCKPOLARITY MCFS_INPUT_CLOCK_POLARITY { get; set; }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum MCFSINPUTCLOCKPOLARITY {
            [EnumDisplayName("0 Degree")]
            McfsPolarity0Degree,
            [EnumDisplayName("180 Degree")]
            McfsPolarity180Degree 
        };

        //输入方式
        [DisplayName("输入方式")]
        [Description("Input Source")]
        public MCFSINPUTSOURCE MCFS_INPUT_SOURCE { get; set; }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum MCFSINPUTSOURCE {
            [EnumDisplayName("Simulator")]
            McfsSimInput,
            [EnumDisplayName("TTL")]
            McfsTTLInput,
            [EnumDisplayName("422")]
            Mcfs422Input,
            [EnumDisplayName("Serializer")]
            McfsSerializerInput 
        };

        //消息字长
        [DisplayName("消息字长")]
        [Description("Message Format")]
        public MCFSMESSAGEWORDLENGTH MCFS_MESSAGE_WORD_LENGTH { get; set; }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum MCFSMESSAGEWORDLENGTH {
            [EnumDisplayName("32 bit")]
            McfsMessageWordlength32,
            [EnumDisplayName("16 bit")]
            McfsMessageWordlength16 
        };

        //看门狗定时器
        [DisplayName("看门狗定时器")]
        [Description("WatchDog Timer")]
        public MCFSWATCHDOGTIMER MCFS_WATCHDOG_TIMER { get; set; }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum MCFSWATCHDOGTIMER {
            [EnumDisplayName("Disabled")]
            McfsWatchDogDisable,
            [EnumDisplayName("52 Ms")]
            McfsWatchDog52Ms,
            [EnumDisplayName("410 Us")]
            McfsWatchDog410Us,
            [EnumDisplayName("1.6 Us")]
            McfsWatchDog1_6Us 
        };
    }
}
