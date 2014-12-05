using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.ViewModels.MenuControls
{
    public class DataRecorderReceiveDataCallBack : acro.DataUpdateCallback
    {
        public override void update(int id, string data, int size)
        {
            Console.WriteLine("接收数据中....");
            Console.WriteLine(data);

        }
    }
}
