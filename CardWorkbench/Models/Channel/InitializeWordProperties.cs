using CardWorkbench.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CardWorkbench.Models
{
    /// <summary>
    /// 初始化字属性model类
    /// </summary>
    [JsonObject]
    public class InitializeWordProperties
    {
        //帧长
        [JsonProperty("FrameLength")]
        [Display(GroupName = "<group1>", Name = "帧长", Order = 0)]
        public int FrameLength { get; set; }

        //字长
        [Display(GroupName = "<group1>", Name = "字长", Order = 0)]
        public MCFSWPMWORDSIZE MCFS_WPM_WORD_SIZE { get; set; }
         
        //传输顺序
        [Display(GroupName = "<group1>", Name = "传输顺序", Order = 0)]
        public MCFSWPMWORDORIENTATION MCFS_WPM_WORD_ORIENTATION { get; set; }

        //对齐方式
        [Display(GroupName = "<group1>", Name = "对齐方式", Order = 0)]
        public MCFSWPMDATAJUSTIFICATION MCFS_WPM_DATA_JUSTIFICATION { get; set; }

        //串行化
        [Display(GroupName = "<group2>", Name = "串行化", Order = 0)]
        public MCFSWPMSERIALIZER MCFS_WPM_SERIALIZER { get; set; }

        //压缩
        [Display(GroupName = "<group2>", Name = "压缩", Order = 0)]
        public MCFSWPMSUPPRESS MCFS_WPM_SUPPRESS { get; set; }

        //变长帧
        [Display(GroupName = "<group2>", Name = "变长帧", Order = 0)]
        public MCFSWPMVARIABLELENGTHFRAME MCFS_WPM_VARIABLE_LENGTH_FRAME { get; set; }

        //帧结束
        [Display(GroupName = "<group2>", Name = "帧结束", Order = 0)]
        public MCFSWPMENDOFFRAME MCFS_WPM_END_OF_FRAME { get; set; }
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum MCFSWPMWORDSIZE
    {
        [EnumDisplayName("1 Bit")]
        McfsWordSize1Bit,
        [EnumDisplayName("2 Bits")]
        McfsWordSize2Bits,
        [EnumDisplayName("3 Bits")]
        McfsWordSize3Bits,
        [EnumDisplayName("4 Bits")]
        McfsWordSize4Bits,
        [EnumDisplayName("5 Bits")]
        McfsWordSize5Bits,
        [EnumDisplayName("6 Bits")]
        McfsWordSize6Bits,
        [EnumDisplayName("7 Bits")]
        McfsWordSize7Bits,
        [EnumDisplayName("8 Bits")]
        McfsWordSize8Bits,
        [EnumDisplayName("9 Bits")]
        McfsWordSize9Bits,
        [EnumDisplayName("10 Bits")]
        McfsWordSize10Bits,
        [EnumDisplayName("11 Bits")]
        McfsWordSize11Bits,
        [EnumDisplayName("12 Bits")]
        McfsWordSize12Bits,
        [EnumDisplayName("13 Bits")]
        McfsWordSize13Bits,
        [EnumDisplayName("14 Bits")]
        McfsWordSize14Bits,
        [EnumDisplayName("15 Bits")]
        McfsWordSize15Bits,
        [EnumDisplayName("16 Bits")]
        McfsWordSize16Bits,
        [EnumDisplayName("17 Bits")]
        McfsWordSize17Bits,
        [EnumDisplayName("18 Bits")]
        McfsWordSize18Bits,
        [EnumDisplayName("19 Bits")]
        McfsWordSize19Bits,
        [EnumDisplayName("20 Bits")]
        McfsWordSize20Bits,
        [EnumDisplayName("21 Bits")]
        McfsWordSize21Bits,
        [EnumDisplayName("22 Bits")]
        McfsWordSize22Bits,
        [EnumDisplayName("23 Bits")]
        McfsWordSize23Bits,
        [EnumDisplayName("24 Bits")]
        McfsWordSize24Bits,
        [EnumDisplayName("25 Bits")]
        McfsWordSize25Bits,
        [EnumDisplayName("26 Bits")]
        McfsWordSize26Bits,
        [EnumDisplayName("27 Bits")]
        McfsWordSize27Bits,
        [EnumDisplayName("28 Bits")]
        McfsWordSize28Bits,
        [EnumDisplayName("29 Bits")]
        McfsWordSize29Bits,
        [EnumDisplayName("30 Bits")]
        McfsWordSize30Bits,
        [EnumDisplayName("31 Bits")]
        McfsWordSize31Bits,
        [EnumDisplayName("32 Bits")]
        McfsWordSize32Bits
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum MCFSWPMWORDORIENTATION
    {
        [EnumDisplayName("MSB First")]
        McfsOrientationMsbFirst,
        [EnumDisplayName("LSB First")]
        McfsOrientationLsbFirst
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum MCFSWPMDATAJUSTIFICATION
    {
        [EnumDisplayName("右侧")]
        McfsRightJustified,
        [EnumDisplayName("左侧")]
        McfsLeftJustified
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum MCFSWPMSERIALIZER
    {
        [EnumDisplayName("关闭")]
        McfsSerializerDisabled,
        [EnumDisplayName("开启")]
        McfsSerailizerEnabled
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum MCFSWPMSUPPRESS
    {
        [EnumDisplayName("关闭")]
        McfsSuppressDisabled,
        [EnumDisplayName("开启")]
        McfsSuppressEnabled
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum MCFSWPMVARIABLELENGTHFRAME
    {
        [EnumDisplayName("关闭")]
        McfsVariableLengthFrameDisabled,
        [EnumDisplayName("开启")]
        McfsVariableLengthFrameEnabled
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum MCFSWPMENDOFFRAME
    {
        [EnumDisplayName("关闭")]
        McfsEndOfFrameDisabled,
        [EnumDisplayName("开启")]
        McfsEndOfFrameEnabled
    };
}
