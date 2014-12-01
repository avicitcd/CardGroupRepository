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
    public class FrameSyncStrategyRegister
    {
        //
        [JsonProperty("McfsErrorToleranceCount")]
        [DisplayName("Error Tolerance")]
        [Description("Error Tolerance")]
        [Range(0, 15)]
        public int McfsErrorToleranceCount { get; set; }

        //
        [JsonProperty("McfsLockToSearchCount")]
        [DisplayName("Lock To Search")]
        [Description("Lock To Search")]
        [Range(1, 15)]
        public int McfsLockToSearchCount { get; set; }

        //
        [JsonProperty("McfsVerifyToLockCount")]
        [DisplayName("Verify To Lock")]
        [Description("Verify To Lock")]
        [Range(1, 15)]
        public int McfsVerifyToLockCount { get; set; }

        //
        [JsonProperty("McfsVerifyToSearchCount")]
        [DisplayName("Verify To Search")]
        [Description("Verify To Search")]
        [Range(1, 15)]
        public int McfsVerifyToSearchCount { get; set; }
    }
}
