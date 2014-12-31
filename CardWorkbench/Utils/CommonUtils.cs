using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CardWorkbench.Utils
{
    public class CommonUtils
    {
        /// <summary>
        /// 获取本机默认ip地址
        /// </summary>
        /// <returns></returns>
        public static string getLocalIpAddress()
        {
            //设置本机默认ip值
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                    break;
                }
            }
            return AddressIP;
        }

        /// <summary>
        /// byte数组转16进制表示
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>16进制字符串</returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        } 
    }
}
