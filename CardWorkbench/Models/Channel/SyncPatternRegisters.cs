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
    [JsonObject]
    [MetadataType(typeof(DataAnnotationValidationMetadata))]
    public class SyncPatternRegisters
    {
        //同步字1(低位)
        [JsonProperty("McfsSyncPattern1")]
        [DisplayName("同步字位1(低位)")]
        [Description("Sync Pattern LSW")]
        [CustomValidation(typeof(DataAnnotationValidationMetadata), "IsSyncPatternValid")]
        public string McfsSyncPattern1 { get; set; }

        //
        [JsonProperty("McfsSyncPattern2")]
        [DisplayName("同步字位2")]
        [Description("Sync Pattern")]
        [CustomValidation(typeof(DataAnnotationValidationMetadata), "IsSyncPatternValid")]
        public string McfsSyncPattern2 { get; set; }

        //
        [JsonProperty("McfsSyncPattern3")]
        [DisplayName("同步字位3")]
        [Description("Sync Pattern")]
        [CustomValidation(typeof(DataAnnotationValidationMetadata), "IsSyncPatternValid")]
        public string McfsSyncPattern3 { get; set; }

        //同步字高位
        [JsonProperty("McfsSyncPattern4")]
        [DisplayName("同步字位4(高位)")]
        [Description("Sync Pattern MSW")]
        [CustomValidation(typeof(DataAnnotationValidationMetadata), "IsSyncPatternValid")]
        public string McfsSyncPattern4 { get; set; }

        //掩码低位
        [JsonProperty("McfsSyncMask1")]
        [DisplayName("掩码位1(低位)")]
        [Description("Sync Mask LSW")]
        [CustomValidation(typeof(DataAnnotationValidationMetadata), "IsSyncPatternValid")]
        public string McfsSyncMask1 { get; set; }

        //
        [JsonProperty("McfsSyncMask2")]
        [DisplayName("掩码位2")]
        [Description("Sync Mask")]
        [CustomValidation(typeof(DataAnnotationValidationMetadata), "IsSyncPatternValid")]
        public string McfsSyncMask2 { get; set; }

        //
        [JsonProperty("McfsSyncMask3")]
        [DisplayName("掩码位3")]
        [Description("Sync Mask")]
        [CustomValidation(typeof(DataAnnotationValidationMetadata), "IsSyncPatternValid")]
        public string McfsSyncMask3 { get; set; }

        //掩码高位
        [JsonProperty("McfsSyncMask4")]
        [DisplayName("掩码位4(高位)")]
        [Description("Sync Mask MSW")]
        [CustomValidation(typeof(DataAnnotationValidationMetadata), "IsSyncPatternValid")]
        public string McfsSyncMask4 { get; set; }

    }

    public class DataAnnotationValidationMetadata {
        public static ValidationResult IsSyncPatternValid(string syncPattern)
        {
            int minValue = 0;
            int maxValue = 65535;
            try
            {
                int currentValue = Convert.ToInt32(syncPattern, 16);
                if (currentValue >= minValue && currentValue <= maxValue)
                {
                    return ValidationResult.Success;
                }
                else {
                    return new ValidationResult("输入范围应该是 0 到 fff 数值之间");
                }
            }
            catch (Exception)
            {

                return new ValidationResult("输入范围应该是 0 到 fff 数值之间");
            }
        }

        public static ValidationResult IsFixedWordValueValid(string fixWordValue)
        {
            long minValue = 0;
            long maxValue = 65535;
            try
            {
                long value = long.Parse(fixWordValue);
                if (value >= minValue && value <= maxValue)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("输入范围应该是 0 到 65535 数值之间");
                }
            }
            catch (Exception)
            {
                //用户输入的文本不是10进制情况下尝试转16进制
                try
                {
                    long currentValue = Convert.ToInt32(fixWordValue, 16);
                    if (currentValue >= minValue && currentValue <= maxValue)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("输入范围应该是 0 到 fff 数值之间");
                    }
                }
                catch (Exception)
                {

                    return new ValidationResult("输入范围应该是 0 到 fff 数值之间");
                }
            } 
            
        }
    }
}
