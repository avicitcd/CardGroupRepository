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
    /// 编辑字属性model类
    /// </summary>
    [JsonObject]
    public class ModifyWordProperty
    {
        //起始字
        [JsonProperty("WordStart")]
        [Display(GroupName = "<group1>", Name = "起始字", Order = 0)]
        public int WordStart { get; set; }

        //结束字
        [JsonProperty("WordEnd")]
        [Display(GroupName = "<group1>", Name = "结束字", Order = 0)]
        public int WordEnd { get; set; }

        //字长
        [Display(GroupName = "<group1>", Name = "字长", Order = 0)]
        public MCFSWPMWORDSIZE MCFS_WPM_WORD_SIZE { get; set; }
         
        //传输顺序
        [Display(GroupName = "<group2>", Name = "传输顺序", Order = 0)]
        public MCFSWPMWORDORIENTATION MCFS_WPM_WORD_ORIENTATION { get; set; }

        //对齐方式
        [Display(GroupName = "<group2>", Name = "对齐方式", Order = 0)]
        public MCFSWPMDATAJUSTIFICATION MCFS_WPM_DATA_JUSTIFICATION { get; set; }

        //串行化
        [Display(GroupName = "<group2>", Name = "串行化", Order = 0)]
        public MCFSWPMSERIALIZER MCFS_WPM_SERIALIZER { get; set; }

        //压缩
        [Display(GroupName = "<group3>", Name = "压缩", Order = 0)]
        public MCFSWPMSUPPRESS MCFS_WPM_SUPPRESS { get; set; }

        //变长帧
        [Display(GroupName = "<group3>", Name = "变长帧", Order = 0)]
        public MCFSWPMVARIABLELENGTHFRAME MCFS_WPM_VARIABLE_LENGTH_FRAME { get; set; }

        //帧结束
        [Display(GroupName = "<group3>", Name = "帧结束", Order = 0)]
        public MCFSWPMENDOFFRAME MCFS_WPM_END_OF_FRAME { get; set; }
    }

    
}
