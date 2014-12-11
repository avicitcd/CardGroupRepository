using CardWorkbench.Models.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CardWorkbench.Network
{
    /// <summary>
    /// UDP接收记录数据客户端类
    /// </summary>
    public class UdpRetrieveRecordDataClient
    {
        // 定义UDP接收
        private UdpClient udpReceive = null;
        private IPEndPoint ipEndPoint = null;
        private CancellationTokenSource cancellationTokenSource = null; //task取消token标识
        private BinaryWriter writer = null;
        //private Task task = null;
        // 异步状态同步
        //private ManualResetEvent sendDone = new ManualResetEvent(false);
        //private ManualResetEvent receiveDone = new ManualResetEvent(false);

        public UdpRetrieveRecordDataClient(DataReceiver dataReceiver, bool isMulticast)
        {
            udpReceive = new UdpClient(dataReceiver.port);//建立接收端的UdpClient实例，并定义端口
            IPAddress ipAddress = IPAddress.Parse(dataReceiver.addr);
            ipEndPoint = new IPEndPoint(ipAddress, dataReceiver.port); //获取或设置IP地址与端口
            //判断是否是组播地址
            if (isMulticast)
            {
                try
                {
                    udpReceive.JoinMulticastGroup(ipAddress);//加入组播组
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
           
        }



        // 接收函数
        public void ReceiveData()
        {
            cancellationTokenSource = new CancellationTokenSource();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Record\\";
            string fileName = "recordData_" + DateTime.Now.Ticks + ".dat";
            FileStream recordFs = File.Open(filePath + fileName, FileMode.Create);
            writer = new BinaryWriter(recordFs);
            Task.Factory.StartNew(() =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (udpReceive.Available <= 0) continue;
                    if (udpReceive.Client == null) return;
                    Byte[] receiveBytes = udpReceive.Receive(ref ipEndPoint);
                    Console.WriteLine("数据记录中............");
                    writer.Write(receiveBytes); //写入本地文件
                    writer.Flush();
                }
            }, cancellationTokenSource.Token);
        }


        public void stopReceiveData()
        {
            cancellationTokenSource.Cancel();  //取消任务请求
            if (cancellationTokenSource.Token.IsCancellationRequested && udpReceive != null)
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
                udpReceive.Close();//关闭协议
                udpReceive = null;//为协议至空值
            }
        }

        // 接收回调函数
        //public void ReceiveCallback(IAsyncResult iar)
        //{
        //    UdpState udpState = iar.AsyncState as UdpState;
        //    if (iar.IsCompleted)
        //    {
        //        //Byte[] receiveBytes = udpState.udpClient.Receive(ref udpReceiveState.ipEndPoint);
        //        Byte[] receiveBytes = udpState.udpClient.EndReceive(iar, ref udpReceiveState.ipEndPoint);
        //        //string receiveString = Encoding.Unicode.GetString(receiveBytes);
        //        Console.WriteLine("Received: {0}", "............");
        //        //receiveDone.Set();
        //        //messageReceived = true;

        //    }
        //}

    }
}
