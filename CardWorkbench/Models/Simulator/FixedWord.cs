using DevExpress.Mvvm.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CardWorkbench.Models
{
    //[MetadataType(typeof(FixedWordMetadata))]
    [Serializable]
    [JsonObject]
    [MetadataType(typeof(DataAnnotationValidationMetadata))]
    public class FixedWord
    {
        //值
        [DisplayName("值")]
        [CustomValidation(typeof(DataAnnotationValidationMetadata), "IsFixedWordValueValid")]
        [JsonIgnore]
        public string valueNonFormat { get; set; }

        //值(10进制)
        [DisplayName("值(dec.)")]
        [Browsable(false)]
        [JsonProperty("value")]
        public long value { get; set; }

        //字号
        [DisplayName("字号")]
        [Range(1, int.MaxValue)]
        [JsonProperty("wordnumber")]
        public int wordnumber { get; set; }

        //字间隔
        [DisplayName("字间隔")]
        [JsonProperty("wordinterval")]
        public int wordinterval { get; set; }
    }
    //public static class FixedWordMetadata
    //{
    //    public static void BuildMetadata(MetadataBuilder<FixedWord> builder)
    //    {
    //        builder.Property(x => x.value).ToString();
    //    }
    //}
}
