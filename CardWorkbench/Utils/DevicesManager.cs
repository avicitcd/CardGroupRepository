using CardWorkbench.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.Utils
{
    public class DevicesManager
    {
        private static IList<Device> devicelist = null;
        private static object _lock = new object();

        public static IList<Device> getCurrentDeviceListInstance()
        {
            if (devicelist == null)
            {
                lock (_lock)
                {
                    if (devicelist == null)
                    {
                        return null;
                    }

                }
            }
            return devicelist;
        }

        public static void setCurrentDeviceList(IList<Device> list) {
            devicelist = list;
        }

        /// <summary>
        /// 根据ID查找设备
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <returns>设备对象</returns>
        public static Device getDeviceByID(string deviceID)
        {
            if (devicelist != null)
            {
                foreach (Device device in devicelist)
                {
                    if (deviceID.Equals(device.deviceID))
                    {
                        return device;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 根据ID查找通道
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <param name="channelID">通道ID</param>
        /// <returns>通道对象</returns>
        public static Channel getChannelByID(string deviceID, string channelID)
        {
            if (devicelist != null)
            {
                foreach (Device device in devicelist)
                {
                    if (deviceID.Equals(device.deviceID))
                    {
                        foreach (Channel channel in device.channelList)
                        {
                            if (channelID.Equals(channel.channelID))
                            {
                                return channel;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 根据设备ID查找模拟器
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <returns>模拟器对象</returns>
        public static Simulator getSimulatorByDeviceID(string deviceID) {
            if (devicelist != null)
            {
                foreach (Device device in devicelist)
                {
                    if (deviceID.Equals(device.deviceID))
                    {
                        return device.simulator;
                    }
                }
            }
            return null;
        }
       
    }

}
