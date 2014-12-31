﻿using CardWorkbench.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CardWorkbench.Models
{
    [Serializable]
    [JsonObject]
    [MetadataType(typeof(DataAnnotationValidationMetadata))]
    public class FormatCreate
    {
        //字长
        [XmlElement("wordsize")]
        [DisplayName("字长")]
        [JsonProperty("wordsize")]
        [Range(4, 16)]
        public int wordsize { get; set; }

        //帧长
        [XmlElement("numberofwords")]
        [DisplayName("帧长")]
        [JsonProperty("numberofwords")]
        [Range(4, 32768)]
        public int numberofwords { get; set; }

        //同步字16进制
        [XmlElement("syncpattern16")]
        [DisplayName("同步字(hex.)")]
        [CustomValidation(typeof(DataAnnotationValidationMetadata), "IsSyncPatternValid")]
        [JsonIgnore]
        public string syncpattern16 { get; set; }

        //同步字10进制
        [DisplayName("同步字(dec.)")]
        [JsonProperty("syncpattern")]
        [Browsable(false)]
        public string syncpattern { get; set; }

        //位速率
        [XmlElement("bitrate")]
        [DisplayName("位速率")]
        [JsonProperty("bitrate")]
        public double bitrate { get; set; }

        //位速率单位
        [Browsable(false)]
        [JsonIgnore]
        public BITRATEUNITS BITRATE_UNITS { get; set; }

        //码型
        [XmlElement("CODE_TYPE")]
        [DisplayName("码型")]
        public CODETYPE CODE_TYPE { get; set; }

        //排列顺序
        [XmlElement("ORIENTATION")]
        [DisplayName("排列顺序")]
        public ORIENTATION _ORIENTATION { get; set; }

    }
        public enum BITRATEUNITS { MHz, KHz, Hz };
        
        [TypeConverter(typeof(EnumTypeConverter))]
        public enum CODETYPE
        {
            [EnumDisplayName("NRZL")]
            SsNRZL,
            [EnumDisplayName("NRZM")]
            SsNRZM,
            [EnumDisplayName("NRZS")]
            SsNRZS,
            [EnumDisplayName("DBIPM")]
            SsDBIPM,
            [EnumDisplayName("RNRZ11")]
            SsRNRZ11,
            [EnumDisplayName("RNRZ15")]
            SsRNRZ15,
            [EnumDisplayName("RNRZ17")]
            SsRNRZ17,
            [EnumDisplayName("RNRZ23")]
            SsRNRZ23,
            [EnumDisplayName("DBIPS")]
            SsDBIPS,
            [EnumDisplayName("BIPL")]
            SsBIPL,
            [EnumDisplayName("BIPM")]
            SsBIPM,
            [EnumDisplayName("BIPS")]
            SsBIPS,
            [EnumDisplayName("DMM")]
            SsDMM,
            [EnumDisplayName("DMS")]
            SsDMS,
            [EnumDisplayName("DMMM")]
            SsDMMM,
            [EnumDisplayName("DMSS")]
            SsDMSS
        }

        [TypeConverter(typeof(EnumTypeConverter))]
        public enum ORIENTATION
        {
            [EnumDisplayName("MSB")]
            SsMSBWOrientation,
            [EnumDisplayName("LSB")]
            SsLSBWOrientation 
        }

}
