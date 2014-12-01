using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CardWorkbench.Models
{
    public class FrameModel 
    {
        public int FrameID { get; set; }

        public string SyncWord { get; set; }
        public string Word1 { get; set; }
        public string Word2 { get; set; }
        public string Word3 { get; set; }
        public string ID { get; set; }
        public string Word5 { get; set; }
        public string Word6 { get; set; }
        public string Word7 { get; set; }
        public string Word8 { get; set; }
        public string Word9 { get; set; }
        public string Word10 { get; set; }
        public string Word11 { get; set; }
        public string Word12 { get; set; }

    }

    public static class FrameTestCase
    {
        public static IList<FrameModel> generateData()
        {
            List<FrameModel> frameModelList = new List<FrameModel>();
            for (int i = 1; i < 9; i++)
            {
                frameModelList.Add(new FrameModel() { FrameID = i, SyncWord = "EB101", Word1 = "CODE", Word2 = "CODE", Word3 = "CODE", ID = i + "", Word5 = "CODE", Word6 = "CODE", Word7 = "CODE", Word8 = "CODE", Word9 = "CODE", Word10 = "CODE", Word11 = "CODE", Word12 = "CODE" });
            }
            return frameModelList;

        }
    }
}
