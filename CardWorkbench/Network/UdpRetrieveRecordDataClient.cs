using CardWorkbench.AcroInterface;
using CardWorkbench.Models.Data;
using CardWorkbench.Utils;
using CardWorkbench.ViewModels.CommonTools;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CardWorkbench.Network
{
    /// <summary>
    /// UDP接收记录数据客户端类
    /// </summary>
    public class UdpRetrieveRecordDataClient
    {
        public static UdpClient udpReceive = null;  // 定义UDP接收
        public static BinaryWriter writer = null;
        public static FileStream recordFs = null;
        public static DispatcherTimer timer = new DispatcherTimer();  //grid数据刷新的timer
        public static DataReceiver dataReceiver = null;
        public static bool isMulticast = false; //是否组播标识
        // 异步状态同步
        //private ManualResetEvent sendDone = new ManualResetEvent(false);
        //private ManualResetEvent receiveDone = new ManualResetEvent(false);
        public static bool isFileRecording = false;  //当前是否在用文件模式记录
        private IPEndPoint ipEndPoint = null;
        private CancellationTokenSource cancellationTokenSource = null; //task取消token标识
        private DynamicBindingList row_lst = new DynamicBindingList();  //grid数据List
        private static GridControl frameDataGrid = null;   //原始帧grid对象
        private static int frameSize = 0;  //帧长
        private static int grid_frame_num = 0; //帧深
        private static int wordByteCount = 0;   //字长(字节)
        private static Byte[] frameDumpReceiveBytes = null; //原始帧模块接收的数据
        private int frameID = 1;   //子帧号
        private static readonly int FRAMEDUMP_DATARECEIVE_TIMER_INTERVAL = 1000;    //原始帧获取数据间隔时间
        private static readonly int WORD_TIME_PER_FRAME_LENGTH = 8;    //时间字字节长度
        private static readonly int WORD_STATUS_PER_FRAME_LENGTH = 4;    //状态字字节长度
        private static int max_read_bytes_num_word_time_and_status_per_frame = 0;    //每帧存在时间和状态字时开始到读取完时间状态字节次数
        private static int max_read_bytes_num_word_time_per_frame = 0;    //每帧存在时间字时开始到读取完时间字节次数
        private static int max_read_bytes_num_word_status_per_frame = 0;    //每帧存在状态字时开始到读取完状态字节次数

        private static bool isExistTimeOrStatusWord = false; //是否每帧存在时间、状态字

        public UdpRetrieveRecordDataClient() { }

        public UdpRetrieveRecordDataClient(DataReceiver dataReceiver, bool isMulticast)
        {
            UdpRetrieveRecordDataClient.dataReceiver = dataReceiver;    //保存dataReceiver作为全局变量供原始帧显示模块判断时间、状态和组播条件
            UdpRetrieveRecordDataClient.isMulticast = isMulticast;    //保存isMulticast作为全局变量供原始帧显示模块判断时间、状态和组播条件
            IPAddress ipAddress = IPAddress.Parse(dataReceiver.addr);
            //没有开始记录
            if (udpReceive == null)
            {
                udpReceive = new UdpClient(dataReceiver.port);//建立接收端的UdpClient实例，并定义端口
                ipEndPoint = new IPEndPoint(ipAddress, dataReceiver.port); //获取或设置IP地址与端口
            }

            //判断是否是组播地址
            if (isMulticast)
            {
                try
                {
                    udpReceive.JoinMulticastGroup(ipAddress);//加入组播组
                }
                catch (Exception ex)
                {
                    udpReceive.Close();
                    udpReceive = null;
                    throw ex;
                }
            }

        }

        /// <summary>
        /// 记录时接收数据
        /// </summary>
        public void ReceiveData()
        {
            if (udpReceive == null)
            {
                return;
            }
            cancellationTokenSource = new CancellationTokenSource();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Record\\";
            string fileName = "recordData_" + DateTime.Now.Ticks + ".dat";
            recordFs = File.Open(filePath + fileName, FileMode.Create);
            writer = new BinaryWriter(recordFs);
            if (recordFs == null || writer == null)
            {
                return;
            }
            Task.Factory.StartNew(() =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (udpReceive.Available <= 0) continue;
                    if (udpReceive.Client == null) return;
                    Byte[] receiveBytes = udpReceive.Receive(ref ipEndPoint);
                    if (timer.IsEnabled)  //如果原始帧模块正在运行则需要把接收的数据放到全局数据供原始帧获取
                    {
                        lock (this)
                        {
                            frameDumpReceiveBytes = new Byte[receiveBytes.Length];
                            receiveBytes.CopyTo(frameDumpReceiveBytes, 0);
                            //Console.WriteLine("receiveBytes："+receiveBytes.Length);
                            //Console.WriteLine("frameDumpReceiveBytes：" + frameDumpReceiveBytes.Length);
                        }
                    }
                    //Console.WriteLine("数据记录中............");
                    writer.Write(receiveBytes); //写入本地文件
                    writer.Flush();
                }
            }, cancellationTokenSource.Token);
        }

        /// <summary>
        /// 原始帧显示时接收数据
        /// </summary>
        /// <param name="frameDataGrid">原始帧grid对象</param>
        /// <param name="frameSize">帧长</param>
        /// <param name="wordSize">字长</param>
        /// <param name="wordSize">帧深</param>
        public void frameDumpReceiveData(GridControl frameDataGrid, int frameSize, int wordSize, int grid_frame_num)
        {
            UdpRetrieveRecordDataClient.frameDataGrid = frameDataGrid;
            UdpRetrieveRecordDataClient.frameSize = frameSize;
            UdpRetrieveRecordDataClient.wordByteCount = wordSize / 8;
            UdpRetrieveRecordDataClient.grid_frame_num = grid_frame_num;
            frameDataGrid.ItemsSource = row_lst;    //设定grid的数据集为row_lst
            if (dataReceiver != null && (dataReceiver.bEnableTime == 1 || dataReceiver.bEnableStatus == 1))  //设定原始帧是否带有时间/状态字
            {
                isExistTimeOrStatusWord = true;
                max_read_bytes_num_word_time_and_status_per_frame = (WORD_TIME_PER_FRAME_LENGTH + WORD_STATUS_PER_FRAME_LENGTH) / wordByteCount;
                max_read_bytes_num_word_time_per_frame = WORD_TIME_PER_FRAME_LENGTH / wordByteCount;
                max_read_bytes_num_word_status_per_frame = WORD_STATUS_PER_FRAME_LENGTH / wordByteCount;
            }
            else {
                isExistTimeOrStatusWord = false;
            }

            timer.Interval = TimeSpan.FromMilliseconds(FRAMEDUMP_DATARECEIVE_TIMER_INTERVAL);
            cancellationTokenSource = new CancellationTokenSource();
            if (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                timer.Tick += new EventHandler(doWork);
            }
            timer.Start();
        }

        private void doWork(object sender, EventArgs e)
        {
            if (udpReceive == null)
            {
                return;
            }
            //1.开始接收数据的任务
            if (cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }
            if (writer == null || recordFs == null) //如果没有同时开启记录模块，原始帧主动获取数据，否则数据由记录模块提供
            {
                Task frameDumpTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        frameDumpReceiveBytes = udpReceive.Receive(ref ipEndPoint);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("错误：" + ex.Message);
                    }
                });
            }
            if (frameDumpReceiveBytes == null)
            {
                return;
            }
            //2. 开始界面取数据显示
            if (row_lst.Count == UdpRetrieveRecordDataClient.grid_frame_num)
            {
                row_lst.Clear(); //接收到了数据后且界面帧数等于设定帧深时再清空数据集
            }
            displayFrameData(frameDataGrid, ref frameDumpReceiveBytes, row_lst);
        }

        /// <summary>
        /// 开始原始帧Grid界面数据显示
        /// </summary>
        /// <param name="frameDataGrid">原始帧grid对象</param>
        /// <param name="receiveBytes">接收到的字节数据</param>
        /// <param name="row_lst">grid数据list</param>
        private void displayFrameData(GridControl frameDataGrid, ref byte[] frameDumpReceiveBytes, DynamicBindingList row_lst)
        {
            using (MemoryStream stream = new MemoryStream(frameDumpReceiveBytes))
            {
                byte[] bytes = null;
                int readCount = 1;  //读取字数
                int lossByteNum = 0; //有时间或状态字时需要丢弃的次数
                int numBytesToRead = frameDumpReceiveBytes.Length;  //剩余还要读取的字节长度
                DynamicDictionary frameDumpDisplayDic = new DynamicDictionary();  //子帧数据dictionary

                while (numBytesToRead >= 0)
                {
                    bytes = new byte[UdpRetrieveRecordDataClient.wordByteCount];
                    if (readCount > UdpRetrieveRecordDataClient.frameSize)   //读取的数据满了一帧，则又从头开始
                    {
                        lossByteNum = 0;
                        //frameDataGrid.Dispatcher.BeginInvoke(new Action(() =>
                        //{
                        frameDumpDisplayDic.SetValue(FrameDumpViewModel.COLUMN_FRAMEDUMP_FRAMEID_NAME, frameID);
                        row_lst.Add(frameDumpDisplayDic);
                        if (row_lst.Count > UdpRetrieveRecordDataClient.grid_frame_num)    //达到指定帧深后从首行更新
                        {
                            row_lst.RemoveAt(UdpRetrieveRecordDataClient.grid_frame_num);
                            int updaterow = frameID % grid_frame_num == 0 ? grid_frame_num - 1 : frameID % grid_frame_num - 1;
                            row_lst[updaterow] = frameDumpDisplayDic;
                            frameDataGrid.RefreshRow(updaterow);
                        }
                        //}));
                        readCount = 1;
                        frameID += 1;
                        frameDumpDisplayDic = new DynamicDictionary();
                    }
                    int n = stream.Read(bytes, 0, UdpRetrieveRecordDataClient.wordByteCount);
                    if (n == 0)
                    {
                        break;
                    }
                    numBytesToRead -= n;
                    ////start 存在时间或状态字时，每帧的开头需处理时间，状态
                    if (isExistTimeOrStatusWord && readCount == 1)
                    {
                        if (dataReceiver.bEnableTime == 1 
                            && dataReceiver.bEnableStatus == 1
                            && lossByteNum < max_read_bytes_num_word_time_and_status_per_frame)   //有时间有状态
                        {
                            lossByteNum += 1;
                            continue;
                        }
                        if (dataReceiver.bEnableTime == 1 
                            && dataReceiver.bEnableStatus == 0
                            && lossByteNum < max_read_bytes_num_word_time_per_frame)  //有时间无状态
                        {
                            lossByteNum += 1;
                            continue;
                        }
                        if (dataReceiver.bEnableTime == 0 
                            && dataReceiver.bEnableStatus == 1
                            && lossByteNum < max_read_bytes_num_word_status_per_frame)  //有状态无时间
                        {
                            lossByteNum += 1;
                            continue;
                        }
                        readCount = 1;
                    }
                    frameDumpDisplayDic.SetValue(FrameDumpViewModel.COLUMN_FRAMEDUMP_WORD_NAME_PREFIX + readCount, CommonUtils.byteToHexStr(bytes));
                    //// end

                    readCount++;
                }
            }
            //frameDumpReceiveBytes = null;
        }

        /// <summary>
        /// 停止记录模块接收数据
        /// </summary>
        /// <param name="deviceNo">设备号</param>
        /// <param name="channelNo">通道号</param>
        public void stopRecordReceiveData(int deviceNo, int channelNo)
        {
            acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
            UdpRetrieveRecordDataClient.isFileRecording = false; //重置是否文件记录模式标识
            UdpRetrieveRecordDataClient.isMulticast = false;  //重置是否组播模式标识
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();  //取消任务请求
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (writer != null)
                    {
                        writer.Close();
                        writer = null;
                    }
                    if (recordFs != null)
                    {
                        recordFs.Close();
                        recordFs = null;
                    }
                }
            }
            if (!timer.IsEnabled)   //同时原始帧模块没有启动显示
            {
                if (udpReceive != null)
                {
                    udpReceive.Close();//关闭协议
                    udpReceive = null;//为协议至空值
                }
                acro1626P.stopDataRecord(deviceNo, channelNo, 0);       //请求关闭数据发送
            }

        }

        /// <summary>
        /// 停止原始帧模块接收显示数据
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <param name="channelNo"></param>
        public void stopFrameDumpReceiveData(int deviceNo, int channelNo)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                frameDumpReceiveBytes = null; //清空接收数据
            }
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();  //取消任务请求
            }
            if (writer == null || recordFs == null) //如同时没有开启记录数据模块
            {
                if (udpReceive != null)
                {
                    udpReceive.Close();//关闭协议
                    udpReceive = null;//为协议至空值
                }
                Acro1626pHelper.getCurrentAcro1626PInstance().stopDataRecord(deviceNo, channelNo, 0); //请求关闭数据发送
            }
        }

        /// <summary>
        /// 记录模块配置修改后，尝试重新开始原始帧接收数据
        /// </summary>
        public void tryRestartFrameDumpReceiveData()
        {
            if (UdpRetrieveRecordDataClient.frameDataGrid != null)
            {
                frameDumpReceiveData(UdpRetrieveRecordDataClient.frameDataGrid, UdpRetrieveRecordDataClient.frameSize, UdpRetrieveRecordDataClient.wordByteCount * 8, UdpRetrieveRecordDataClient.grid_frame_num);
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
