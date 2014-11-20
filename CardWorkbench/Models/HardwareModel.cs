using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardWorkbench.Models
{
    public class HardwareModel
    {
        //板卡ID
        public int cardID { get; set; }

        //板卡型号
        public string cardModel { get; set; }

        //板卡功能描述
        public string cardDescription { get; set; }

        //todo: 板卡组成对象
       
    }

    public static class HardwareModelTestCase
    {
        public static IList<HardwareModel> generateData()
        {
            List<HardwareModel> hardwareList = new List<HardwareModel>();
            for (int i = 1; i < 6; i++)
            {
                hardwareList.Add(new HardwareModel() { cardID = i, cardModel = "GDP4425", cardDescription = "接收机，未同步，帧同步，模拟" });
            }
            return hardwareList;

        }
    }
}
