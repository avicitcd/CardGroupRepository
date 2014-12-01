using CardWorkbench.AcroInterface;
using CardWorkbench.Models;
using CardWorkbench.test;
using CardWorkbench.Utils;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace CardWorkbench.ViewModels.CommonTools
{
    public class HardwareRecognitionViewModel
    {
       
        public readonly string GRID_HARDWARERECOGNITION_NAME = "hardwareRecognitionGrid";   //硬件设备grid名称

        public static GridControl grid ;  //此变量在mainwindow的对话框点击"添加"时可获取

        public HardwareRecognitionViewModel() {
        }

        /// <summary>
        /// 设备Grid加载时命令
        /// </summary>
        public ICommand hardwareRecognitionLoadedCommand
        {
            get { return new DelegateCommand<RoutedEventArgs>(onHardwareRecognitionLoaded, x => { return true; }); }
        }

        private void onHardwareRecognitionLoaded(RoutedEventArgs e)
        {
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);
            grid = LayoutHelper.FindElementByName(root, GRID_HARDWARERECOGNITION_NAME) as GridControl;
            //grid.ItemsSource = findDeviceByXml();
            grid.ItemsSource = findDeviceByJson();
        }

        private IList<Device> findDeviceByJson() {
            List<Device> hardwareList = new List<Device>();
            string json = "";

            if (Constants.isDebugMode)
            {
                json = DeviceScanSimTest.simDeviceJsonStr();

            }
            else 
            { 
                try
                {
                    //调用板卡接口扫描当前设备
                    acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                    json = acro1626P.scanDevice();
                }
                catch (System.ApplicationException ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }

            var str = JObject.Parse(json).SelectToken("device").ToString();
            hardwareList = JsonConvert.DeserializeObject<List<Device>>(str);
            //设置设备描述
            if (hardwareList != null)
            {
                foreach (Device device in hardwareList)
                {
                    StringBuilder deviceDescrptionBuilder = new StringBuilder();
                    if (device.channelList != null)
                    {
                        foreach (Channel channel in device.channelList)
                        {
                            deviceDescrptionBuilder.Append(channel.channelName);
                            deviceDescrptionBuilder.Append(",");
                        }
                    }
                    if (device.simulator != null)
                    {
                         deviceDescrptionBuilder.Append(device.simulator.simulatorName);
                         deviceDescrptionBuilder.Append(",");
                    }
                    device.deviceDescription = deviceDescrptionBuilder.ToString(0, deviceDescrptionBuilder.Length - 1);
                }
            }
            return hardwareList;
        }

        /// <summary>
        /// 从xml查找设备清单
        /// </summary>
        /// <returns>返回设备清单列表</returns>
        /**
        private static IList<Device> findDeviceByXml()
        {
            List<Device> hardwareList = new List<Device>();
            ////////// TODO: 以下xml查询代码还可以优化 ///////////
            try
            {
                string deviceID, deviceModel;
                XmlDocument testXmlDoc = XmlParserUtil.getXmlDocumnetInstance("C:\\Users\\Neptune\\Desktop\\Hardware.xml");
                XmlNodeList nodeList = XmlParserUtil.getChildNodesListBySingleParentNodeName("Box");
                foreach (XmlNode deviceNode in nodeList)
                {
                    Device device = new Device();
                    deviceID = XmlParserUtil.getNodeAttributeValue(deviceNode, "id");
                    deviceModel = XmlParserUtil.getNodeAttributeValue(deviceNode, "model");
                    StringBuilder deviceDescrptionBuilder = new StringBuilder();
                    device.deviceID = deviceID;
                    device.deviceModel = deviceModel;

                    //获得设备是否含通道或模拟器
                    if (deviceNode.HasChildNodes)
                    {
                        foreach (XmlNode node in deviceNode.ChildNodes)
                        {
                            if ("Channels".Equals(node.Name) && node.HasChildNodes)
                            {
                                IList<Channel> channelList = new List<Channel>();
                                foreach (XmlNode channelNode in node.ChildNodes)
                                {
                                    //设置设备通道列表
                                    Channel channel = new Channel();
                                    channel.channelName = XmlParserUtil.getNodeAttributeValue(channelNode, "name");
                                    channel.channelID = XmlParserUtil.getNodeAttributeValue(channelNode, "id");
                                    channelList.Add(channel);
                                    device.channelList = channelList;
                                    //通道名称加入设备描述
                                    deviceDescrptionBuilder.Append(XmlParserUtil.getNodeAttributeValue(channelNode, "name"));
                                    deviceDescrptionBuilder.Append(",");
                                    //查询通道状态
                                    if (channelNode.HasChildNodes)
                                    {
                                        foreach (XmlNode statusNode in channelNode.ChildNodes)
                                        {
                                            if ("ChannelStatus".Equals(statusNode.Name) && statusNode.HasChildNodes)
                                            {
                                                ChannelStatus channelStatus = new ChannelStatus();
                                                foreach (XmlNode statusItem in statusNode.ChildNodes)
                                                {
                                                    if ("bRun".Equals(statusItem.Name))
                                                    {
                                                        channelStatus.bRun = int.Parse(statusItem.InnerText); //通道是否允许
                                                    }
                                                }
                                                channel.channelStatus = channelStatus; //设置通道状态
                                            }
                                        }
                                    }
                                }
                            }
                            else if ("Simulator".Equals(node.Name))
                            {
                                //设置设备模拟器
                                Simulator simulator = new Simulator();
                                simulator.simulatorID = XmlParserUtil.getNodeAttributeValue(node, "id");
                                simulator.simulatorName = XmlParserUtil.getNodeAttributeValue(node, "name");
                                device.simulator = simulator;
                                //模拟器名称加入设备描述
                                deviceDescrptionBuilder.Append(XmlParserUtil.getNodeAttributeValue(node, "name"));
                                deviceDescrptionBuilder.Append(",");

                            }
                        }
                    }
                    device.deviceDescription = deviceDescrptionBuilder.ToString(0, deviceDescrptionBuilder.Length - 1);
                    hardwareList.Add(device);
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("获取设备配置信息失败!");
            }
            catch (Exception e)
            {
                if (e.Source != null)
                    Console.WriteLine(e.Message);
                throw e;
            }

            return hardwareList;
        }
         */

    }
}
