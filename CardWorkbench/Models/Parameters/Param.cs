using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace CardWorkbench.Models
{
    /// <summary>
    /// 参数实体类
    /// </summary>
    public class Param
    {
        public int paramId { get; set; }

        [DisplayName("CalibrateType")]
        public int calibrateTypeId { get; set; }

        [DisplayName("ParamSortType")]
        public int paramSortTypeId { get; set; }

        public string paramName { get; set; }

        public string paramChineseName { get; set; }

        public virtual CalibrateType calibrateType { get; set; }
        public virtual ParamSortType paramSortType { get; set; }
    }

    /// <summary>
    /// 测试数据
    /// </summary>
    public static class SampleData
    {
        public static List<Param> paramList { get; set; }
        public static List<CalibrateType> calibrateTypeList { get; set; }
        public static List<ParamSortType> paramSortTypeList { get; set; }
        public static void initData()   //初始化数据
        {
            if (paramList != null)
            {
                return;
            }

            calibrateTypeList = new List<CalibrateType>
            {
                new CalibrateType { calibrateTypeName = "点对校准" },
                new CalibrateType { calibrateTypeName = "多项式校准" }
              
                };

            int i = 0;
            calibrateTypeList.ForEach(x => x.calibrateTypeId = ++i);

            paramSortTypeList = new List<ParamSortType>
            {
                    new ParamSortType { paramSortTypeName = "飞控" },
                    new ParamSortType { paramSortTypeName = "航电" },
                    new ParamSortType { paramSortTypeName = "USDC" },
                    new ParamSortType { paramSortTypeName = "IDMP" },
                    new ParamSortType { paramSortTypeName = "其它" }

            };
            i = 0;
            paramSortTypeList.ForEach(x => x.paramSortTypeId = ++i);

            paramList = new List<Param>
            {
                new Param { paramName = "Rock" , paramChineseName = "名称1", 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Jazz" , paramChineseName = "名称2" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "航电") },
                new Param { paramName = "Metal" , paramChineseName = "名称3" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "多项式校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "IDMP") },
                new Param { paramName = "Alternative" , paramChineseName = "名称4" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Disco" , paramChineseName = "名称5"  , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "IDMP") },
                new Param { paramName = "Blues" , paramChineseName = "名称6" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "多项式校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "航电") },
                new Param { paramName = "Latin" , paramChineseName = "名称7" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "USDC") },
                new Param { paramName = "Reggae" , paramChineseName = "名称8" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Pop" , paramChineseName = "名称9", 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "多项式校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Classical" , paramChineseName = "名称10", 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "USDC") },
                   new Param { paramName = "Rock" , paramChineseName = "名称1", 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "ID" , paramChineseName = "ID参数" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "其它") },
                new Param { paramName = "Metal" , paramChineseName = "名称3" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "多项式校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "IDMP") },
                new Param { paramName = "Alternative" , paramChineseName = "名称4" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Disco" , paramChineseName = "名称5"  , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "IDMP") },
                new Param { paramName = "Blues" , paramChineseName = "名称6" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "多项式校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "航电") },
                new Param { paramName = "Latin" , paramChineseName = "名称7" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "USDC") },
                new Param { paramName = "Reggae" , paramChineseName = "名称8" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Pop" , paramChineseName = "名称9", 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "多项式校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Classical" , paramChineseName = "名称10", 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "USDC") },
                    new Param { paramName = "Rock" , paramChineseName = "名称1", 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Jazz" , paramChineseName = "名称2" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "航电") },
                new Param { paramName = "Metal" , paramChineseName = "名称3" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "多项式校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "IDMP") },
                new Param { paramName = "Alternative" , paramChineseName = "名称4" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Disco" , paramChineseName = "名称5"  , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "IDMP") },
                new Param { paramName = "Blues" , paramChineseName = "名称6" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "多项式校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "航电") },
                new Param { paramName = "Latin" , paramChineseName = "名称7" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "USDC") },
                new Param { paramName = "Reggae" , paramChineseName = "名称8" , 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Pop" , paramChineseName = "名称9", 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "多项式校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "飞控") },
                new Param { paramName = "Classical" , paramChineseName = "名称10", 
                    calibrateType = calibrateTypeList.Single(a => a.calibrateTypeName == "点对校准"),
                    paramSortType = paramSortTypeList.Single(a => a.paramSortTypeName == "USDC") }
            };

            
        }
    }
}
