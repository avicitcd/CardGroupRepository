using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.test
{
    /// <summary>
    /// 模拟板卡设备信息。供硬件扫描用
    /// </summary>
    public class DeviceScanSimTest
    {
        public static string simDeviceJsonStr() {
           string json = @"{
                                        'device': [
                                            {
                                                'deviceID': 1,
                                                'deviceModel': '1626P',
                                                'channelList': [
                                                    {
                                                        'channelID': 1,
                                                        'channelName': '通道1'
                                                    },
                                                    {
                                                        'channelID': 2,
                                                        'channelName': '通道2'
                                                    }
                                                ],
                                                'simulator': {
                                                    'simulatorID': 1,
                                                    'simulatorName': '模拟器'
                                                }
                                            },
                                            { 
                                                'deviceID': 2,
                                                'deviceModel': '1000P',
                                                'channelList': [
                                                    {
                                                        'channelID': 1,
                                                        'channelName': '通道1'
                                                    },
                                                    {
                                                        'channelID': 2,
                                                        'channelName': '通道2'
                                                    },
                                                    {
                                                        'channelID': 3,
                                                        'channelName': '通道3'
                                                    },
                                                    {
                                                        'channelID': 4,
                                                        'channelName': '通道4'
                                                    }
                                                ],
                                                'simulator': null
                                            }
                                        ]
                                    }";
           return json;
        }

    }
}
