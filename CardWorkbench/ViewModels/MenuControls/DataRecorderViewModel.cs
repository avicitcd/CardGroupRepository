using CardWorkbench.AcroInterface;
using CardWorkbench.Models;
using CardWorkbench.Models.Data;
using CardWorkbench.test;
using CardWorkbench.Utils;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.LayoutControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CardWorkbench.ViewModels.MenuControls
{
    public class DataRecorderViewModel : ViewModelBase
    {
        public static readonly string TEXTBLOCK_RECORDDEVICE_NAME = "recordDeviceTextBlock"; //记录设备名称

        public static readonly string TEXTBLOCK_RECORDCHANNEL_NAME = "recordChannelTextBlock";  //记录通道名称

        public static readonly string LAYOUTGROUP_NETWORK_NAME = "networkLayoutGroup";  //“网络”layoutGroup名称

        public static readonly string LAYOUTGROUP_FILE_NAME = "fileLayoutGroup";  //“文件”layoutGroup名称

        public static readonly string COMBOBOX_RECORDMODEL_NAME = "recordModelComboBox"; //“记录模式”下拉框名称

        public static readonly string SPINEDIT_RECORDLENGTH_NAME = "recordLengthSpinEdit"; //"记录包大小"spinedit名称

        public static readonly string BUTTONEDIT_FILEBROWSER_NAME = "fileBrowser"; //文件存放地址按钮名称

        public static readonly string TEXTEDIT_RECORDERIPADDRESS_NAME = "recorderIpAddressTextEdit"; //ip地址编辑框名称

        public static readonly string TEXTEDIT_RECORDERPORT_NAME = "recorderPortTextEdit"; //ip端口编辑框名称

        public static readonly string CHECKEDIT_TIMEWORD_NAME = "outputTimeWordCheckEdit"; //输出时间字单选框名称

        public static readonly string CHECKEDIT_STATUSWORD_NAME = "outputStatusWordCheckEdit"; //输出状态字单选框名称

        public static readonly string CHECKEDIT_FRAMEMODE_NAME = "frameModeCheckEdit"; //开启帧模式单选框名称

        public static readonly string IMAGE_RECORD_FRAMEDATA_NAME = "recordFrameDataImg"; //“记录”按钮image名称

        public static readonly string PANEL_RECORDTIME_NAME = "recordTimePanel";         //回放时间显示panel名称

        public static readonly string IMAGE_RECORDTIME_NAME = "recordTimeImage";        //回放时间显示image名称

        public static readonly string TEXTBLOCK_RECORDTIME_NAME = "recordTimeText";   //回放时间显示文本名称

        public static readonly string PATH_IMAGE_PLAY_RECORD = "/Images/play/record.png";

        public static readonly string PATH_IMAGE_PLAY_RECORDING = "/Images/play/recording.png";

        public static readonly string TOOLTIP_BUTTON_RECORD_FRAMEDATA = "记录";

        public static readonly string TOOLTIP_BUTTON_STOP_RECORD_FRAMEDATA = "停止记录";

        public static readonly string DEFAULTEXT = "dat";

        public static readonly string DEFAULTFILENAME = "mcfs-data-1";

        public static readonly string FILTER = "Data Files|*.dat";

        public static readonly int FILTERINDEX = 1;

        public static readonly bool OVERWRITEPROMPT = true;

        private long recorderLength = 0;

        private DispatcherTimer record_timer = null;    //记录数据时的timer
        
        private DateTime startTime; //记录数据时的起始时间
        
        private TimeSpan timePassed, timeSinceLastStop; //记录间隔时间，自上次记录停止的间隔时间(暂时没用到)
        
        private TextBlock recordTimeText; //记录时间显示文本
        public StartDataRecord startDataRecord { get; set; }  //记录数据配置实体类

        public DataReceiver dataReceiver { get; set; }  //传输数据配置实体类

        public virtual bool DialogResult { get; protected set; }
        public virtual string ResultFileName { get; protected set; }
        public ISaveFileDialogService SaveFileDialogService { get { return GetService<ISaveFileDialogService>(); } }
        public IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }

        public DataRecorderReceiveDataCallBack dataRecorderReceiveDataCallBack { get; set; }
        public DataRecorderViewModel()
        {
            dataRecorderReceiveDataCallBack = new DataRecorderReceiveDataCallBack();
        }

        /// <summary>
        /// 记录数据界面初始化命令
        /// </summary>
        public ICommand dataRecorderLoadCommand
        {
            get { return new DelegateCommand<TextBlock>(onDataRecorderLoad, x => { return true; }); }
        }

        private void onDataRecorderLoad(TextBlock e)
        {
            //DataRecorderCallBackTest.testCallBack();

            string[] deviceAndChannelID = MainWindowViewModel.getSelectChannelInfo(e);
            if (deviceAndChannelID != null && !String.IsNullOrEmpty(deviceAndChannelID[deviceAndChannelID.Length - 1]))
            {
                //当前设备
                Device currentDevice = DevicesManager.getDeviceByID(deviceAndChannelID[0]);
                FrameworkElement root = LayoutHelper.GetTopLevelVisual(e);
                TextBlock recordDeviceTextEdit = (TextBlock)LayoutHelper.FindElementByName(root, TEXTBLOCK_RECORDDEVICE_NAME);
                recordDeviceTextEdit.Text = currentDevice.deviceModel;

                //当前通道
                TextBlock recordChannelTextEdit = (TextBlock)LayoutHelper.FindElementByName(root, TEXTBLOCK_RECORDCHANNEL_NAME);
                Channel currentChannel = DevicesManager.getChannelByID(deviceAndChannelID[0], deviceAndChannelID[1]);
                recordChannelTextEdit.Text = currentChannel.channelName;

            }
        }


        /// <summary>
        /// 记录模式选择切换命令
        /// </summary>
        public ICommand recordModelCommand
        {
            get { return new DelegateCommand<EditValueChangedEventArgs>(onRecordModelChange, x => { return true; }); }
        }

        private void onRecordModelChange(EditValueChangedEventArgs e)
        {
            FrameworkElement root = LayoutHelper.GetRoot(e.Source as ComboBoxEdit);
            LayoutGroup networkLayoutGroup = LayoutHelper.FindElementByName(root, LAYOUTGROUP_NETWORK_NAME) as LayoutGroup;
            LayoutGroup fileLayoutGroup = LayoutHelper.FindElementByName(root, LAYOUTGROUP_FILE_NAME) as LayoutGroup;

            if (RECORDERMODEL.NETWORK == (RECORDERMODEL)e.NewValue)
            {
                networkLayoutGroup.IsCollapsed = false;
                networkLayoutGroup.IsEnabled = true;
                fileLayoutGroup.IsCollapsed = true;
                fileLayoutGroup.IsEnabled = false;
            }
            else if (RECORDERMODEL.FILE == (RECORDERMODEL)e.NewValue)
            {
                fileLayoutGroup.IsCollapsed = false;
                fileLayoutGroup.IsEnabled = true;
                networkLayoutGroup.IsCollapsed = true;
                networkLayoutGroup.IsEnabled = false;
            }
        }

        /// <summary>
        /// 记录文件路径选择命令
        /// </summary>
        public ICommand recorderFilePathSettingCommand
        {
            get { return new DelegateCommand<ButtonEdit>(onRecorderFilePathSettingClick, x => { return true; }); }
        }
        private void onRecorderFilePathSettingClick(ButtonEdit e)
        {
            SaveFileDialogService.DefaultExt = DEFAULTEXT;
            SaveFileDialogService.DefaultFileName = DEFAULTFILENAME;
            SaveFileDialogService.Filter = FILTER;
            SaveFileDialogService.FilterIndex = FILTERINDEX;
            DialogResult = SaveFileDialogService.ShowDialog();
            if (DialogResult)
            {
                e.EditValue = SaveFileDialogService.GetFullFileName();
            }
        }

        /// <summary>
        /// "记录"数据按钮命令
        /// </summary>
        public ICommand recordDataCommand
        {
            get { return new DelegateCommand<RoutedEventArgs>(onRecordDataClick, x => { return true; }); }
        }

        private void onRecordDataClick(RoutedEventArgs e)
        {
            ToggleButton recordFrameData_btn = e.Source as ToggleButton;
            Image recordFrameDataImg = recordFrameData_btn.FindName(IMAGE_RECORD_FRAMEDATA_NAME) as Image;
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(recordFrameData_btn as DependencyObject);
            StackPanel recordTimeTextPanel = LayoutHelper.FindElementByName(root, PANEL_RECORDTIME_NAME) as StackPanel;
            Image recordFlashImage = recordTimeTextPanel.FindName(IMAGE_RECORDTIME_NAME) as Image;
            recordTimeText = recordTimeTextPanel.FindName(TEXTBLOCK_RECORDTIME_NAME) as TextBlock;
            ComboBoxEdit recordModelComboBox = LayoutHelper.FindElementByName(root, COMBOBOX_RECORDMODEL_NAME) as ComboBoxEdit;
            CheckEdit timeWordCheckEdit = LayoutHelper.FindElementByName(root, CHECKEDIT_TIMEWORD_NAME) as CheckEdit;
            CheckEdit statusWordCheckEdit = LayoutHelper.FindElementByName(root, CHECKEDIT_STATUSWORD_NAME) as CheckEdit;
            CheckEdit frameModeCheckEdit = LayoutHelper.FindElementByName(root, CHECKEDIT_FRAMEMODE_NAME) as CheckEdit;
            SpinEdit recordLengthSpinEdit = LayoutHelper.FindElementByName(root, SPINEDIT_RECORDLENGTH_NAME) as SpinEdit;
            acro.Acro1626P acro1626P = null;
            acro.DataReceiver acroDataReceiver = null;
            Channel currentChannel = null;
            //判断当前通道状态
            string[] deviceAndChannelID = MainWindowViewModel.getSelectChannelInfo(recordFrameData_btn);
            if (deviceAndChannelID != null && !String.IsNullOrEmpty(deviceAndChannelID[deviceAndChannelID.Length - 1]))
            {
                currentChannel = DevicesManager.getChannelByID(deviceAndChannelID[0], deviceAndChannelID[1]);
                if (currentChannel.channelStatus.bRun == 0)
                {
                    MessageBoxService.Show(
                       messageBoxText: "当前通道尚未启动，请启动后再记录数据!",
                       caption: "警告",
                       button: MessageBoxButton.OK,
                       icon: MessageBoxImage.Warning);
                    recordFrameData_btn.IsChecked = false; //恢复记录按钮未点击
                    return;
                }
            }

            //验证数据包大小设置是否通过
            bool isTimeWordChecked = timeWordCheckEdit.IsChecked.HasValue ? (bool)timeWordCheckEdit.IsChecked : false;
            bool isStatusWordChecked = statusWordCheckEdit.IsChecked.HasValue ? (bool)statusWordCheckEdit.IsChecked : false;
            InitializeWordProperties initializeWordProperties = WordPropertiesManager.findCurrentWordProperties(deviceAndChannelID[0], deviceAndChannelID[1]);
            if (initializeWordProperties == null)
            {
                MessageBoxService.Show(
                      messageBoxText: "无法记录数据，请先在【通道设置】中初始化字属性!",
                      caption: "警告",
                      button: MessageBoxButton.OK,
                      icon: MessageBoxImage.Warning);
                recordFrameData_btn.IsChecked = false; //恢复记录按钮未点击
                return;
            }
            if (!validateRecordLength(initializeWordProperties, isTimeWordChecked, isStatusWordChecked, int.Parse(recordLengthSpinEdit.EditValue as string)))
            {
                MessageBoxService.Show(
                       messageBoxText: "当前设置的数据包大小已超过32768字节，请重新填写【数据包子帧数】!",
                       caption: "警告",
                       button: MessageBoxButton.OK,
                       icon: MessageBoxImage.Warning);
                recordFrameData_btn.IsChecked = false; //恢复记录按钮未点击
                return;
            }

            //开始记录
            if (recordFrameData_btn.IsChecked == true)
            {
                //准备记录配置数据
                ButtonEdit fileBrowserButtonEdit = LayoutHelper.FindElementByName(root, BUTTONEDIT_FILEBROWSER_NAME) as ButtonEdit;
                TextEdit ipAddressTextEdit = LayoutHelper.FindElementByName(root, TEXTEDIT_RECORDERIPADDRESS_NAME) as TextEdit;
                TextEdit portTextEdit = LayoutHelper.FindElementByName(root, TEXTEDIT_RECORDERPORT_NAME) as TextEdit;
                String fileName = "";

                if (RECORDERMODEL.FILE == (RECORDERMODEL)recordModelComboBox.EditValue)
                {
                    fileName = fileBrowserButtonEdit.EditValue as string;
                }
                else if (RECORDERMODEL.NETWORK == (RECORDERMODEL)recordModelComboBox.EditValue)
                {
                    fileName = ipAddressTextEdit.EditValue + ":" + portTextEdit.EditValue;
                    //网络模式下需初始化传输数据配置类
                    dataReceiver = new DataReceiver()
                    {
                        addr = ipAddressTextEdit.EditValue as string,
                        port = portTextEdit.EditValue != null ? int.Parse(portTextEdit.EditValue as string) : 0,
                        frameLength = initializeWordProperties.FrameLength,
                        wordSize = (int)initializeWordProperties.MCFS_WPM_WORD_SIZE + 1,
                        IDPosition = initializeWordProperties.IDPosition,
                        bEnableStatus = isStatusWordChecked ? 1 : 0,
                        bEnableTime = isTimeWordChecked ? 1 : 0
                    };
                }
                //初始化开始记录配置类
                startDataRecord = new StartDataRecord()
                {
                    RecordLength = recorderLength,
                    FileName = fileName,
                    bControlRunState = 0,
                    bEnableTime = isTimeWordChecked ? 1 : 0,
                    bEnableStatus = isStatusWordChecked ? 1 : 0,
                    bFrameMode = frameModeCheckEdit.IsChecked == true ? 1 : 0
                };
                //开始记录配置
                try
                {
                    string json = JsonConvert.SerializeObject(startDataRecord, Formatting.Indented);  //序列化获得要设置的json数据
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{").Append("\"").Append(typeof(StartDataRecord).Name)
                      .Append("\"").Append(": ").Append(json).Append("}");

                    //调用板卡接口设置开始记录数据
                    acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                    acro1626P.startDataRecord(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]), sb.ToString());
                }
                catch (Exception ex)
                {
                    MessageBoxService.Show(
                        messageBoxText: "记录数据出错，请检查填写是否完整!",
                        caption: "错误",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    TextBox logTextBox = UIControlHelper.getLogTextBox(null, recordFrameData_btn); //日志面板textbox
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText(ex.Message + "\n");
                    }
                    acro1626P.stopDataRecord(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]), 0); //停止记录配置
                    recordFrameData_btn.IsChecked = false; //恢复记录按钮未点击
                    return;
                }

                //数据传输
                if (dataReceiver != null)
                {
                        string json = JsonConvert.SerializeObject(dataReceiver, Formatting.Indented);  //序列化获得要设置的json数据
                        StringBuilder sb = new StringBuilder();
                        sb.Append("{").Append("\"").Append(typeof(DataReceiver).Name)
                          .Append("\"").Append(": ").Append(json).Append("}");
                    try
                    {
                        //调用板卡接口设置传输数据
                        if (acroDataReceiver == null)
                        {
                            acroDataReceiver = new acro.DataReceiver();
                        }
                        acroDataReceiver.startReceive(sb.ToString(), new DataRecorderReceiveDataCallBack());

                        //while (true)
                        //{
                        //    Thread.Sleep(2000);
                        //    Console.WriteLine("-------");
                        //}

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("~~~~~~"+ex.Message);
                        MessageBoxService.Show(
                            messageBoxText: "接收数据时出错!",
                            caption: "错误",
                            button: MessageBoxButton.OK,
                            icon: MessageBoxImage.Error);
                        TextBox logTextBox = UIControlHelper.getLogTextBox(null, recordFrameData_btn); //日志面板textbox
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText(ex.Message + "\n");
                        }
                        //停止计时，更改UI显示
                        changeStopRecordDataUI(recordFrameData_btn, recordFrameDataImg, recordFlashImage, recordTimeTextPanel);
                        recordFrameData_btn.IsChecked = false; //恢复记录按钮未点击
                        acro1626P.stopDataRecord(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]), 0); //停止记录配置
                        acroDataReceiver.stopReceive();
                    }
                }
               
                //网络模式下需要单独考虑存储
                //if (RECORDERMODEL.NETWORK == (RECORDERMODEL)recordModelComboBox.EditValue)
                //{
                //}
            }
            else
            {
                try
                {
                    //停止记录
                    if (acro1626P == null)
                    {
                        acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                    }
                    acro1626P.stopDataRecord(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]), 0);
                    //调用板卡接口设置传输数据
                    if (acroDataReceiver == null)
                    {
                        acroDataReceiver = new acro.DataReceiver();
                    }
                    DataRecorderReceiveDataCallBack dataRecorderReceiveDataCallBack = new DataRecorderReceiveDataCallBack();
                    acroDataReceiver.stopReceive();
                }
                catch (Exception ex)
                {
                    MessageBoxService.Show(
                         messageBoxText: "停止记录数据出错!",
                         caption: "错误",
                         button: MessageBoxButton.OK,
                         icon: MessageBoxImage.Error);
                    TextBox logTextBox = UIControlHelper.getLogTextBox(null, recordFrameData_btn); //日志面板textbox
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText(ex.Message + "\n");
                    }
                    recordFrameData_btn.IsChecked = false; //恢复记录按钮未点击
                }
                //停止计时，更改UI显示
                changeStopRecordDataUI(recordFrameData_btn, recordFrameDataImg, recordFlashImage, recordTimeTextPanel);
            }

        }

        private bool validateRecordLength(InitializeWordProperties initializeWordProperties, bool isTimeWordChecked, bool isStatusWordChecked, int recordLength)
        {
            int maxRecordDataLength = 32768;
            long currentRecordDataLength = 0;
            int frameLength = initializeWordProperties.FrameLength;
            int wordByte = ((int)initializeWordProperties.MCFS_WPM_WORD_SIZE + 1) / 8;
            if (!isTimeWordChecked && !isStatusWordChecked) //无时间、无状态
            {
                currentRecordDataLength = (frameLength * wordByte) * recordLength;
            }
            else if (isTimeWordChecked && isStatusWordChecked) //有时间，有状态
            {
                currentRecordDataLength = (frameLength * wordByte + 12) * recordLength;
            }
            else if (isTimeWordChecked && !isStatusWordChecked)  //有时间，无状态
            {
                currentRecordDataLength = (frameLength * wordByte + 8) * recordLength;
            }
            else if (!isTimeWordChecked && isStatusWordChecked) //有状态，无时间
            {
                currentRecordDataLength = (frameLength * wordByte + 4) * recordLength;
            }
            if (currentRecordDataLength <= maxRecordDataLength)
            {
                recorderLength = currentRecordDataLength;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置时间文本的handle函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void record_timer_tick(object sender, object e)
        {
            timePassed = DateTime.Now - startTime;
            recordTimeText.Text = convertNumberString((timeSinceLastStop + timePassed).Hours) + ":"
                                + convertNumberString((timeSinceLastStop + timePassed).Minutes) + ":"
                                + convertNumberString((timeSinceLastStop + timePassed).Seconds) + "."
                                + convertNumberString((timeSinceLastStop + timePassed).Milliseconds);
        }

        /// <summary>
        /// 时间值（时分秒）文本转换，转换数字为两位数文本
        /// </summary>
        /// <param name="number">时分秒数值</param>
        /// <returns>时分秒文本</returns>
        private string convertNumberString(int number)
        {
            string result;
            if (number < 10)
            {
                result = "0" + number;
                return result;
            }
            result = number.ToString();
            return result;
        }

        /// <summary>
        /// 开始记录数据时UI设置
        /// </summary>
        private void changeStartRecordDataUI(ToggleButton recordFrameData_btn, Image recordFrameDataImg, Image recordFlashImage, StackPanel recordTimeTextPanel)
        {
            //更改按钮外观
            recordFrameDataImg.Source = new BitmapImage(new Uri(PATH_IMAGE_PLAY_RECORDING, UriKind.Relative));
            recordFrameData_btn.ToolTip = TOOLTIP_BUTTON_STOP_RECORD_FRAMEDATA;
            //显示记录时间，开始动画
            recordTimeTextPanel.Visibility = Visibility.Visible;
            DoubleAnimation da = new DoubleAnimation();
            da.From = 1;
            da.To = 0.2;
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;
            da.Duration = TimeSpan.FromMilliseconds(500);
            recordFlashImage.BeginAnimation(TextBlock.OpacityProperty, da);
            //开始记录，启动记录时间文本的定时器
            record_timer = new DispatcherTimer();
            record_timer.Tick += record_timer_tick;
            record_timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            startTime = DateTime.Now;
            record_timer.Start();
        }

        /// <summary>
        /// 停止记录数据时UI设置
        /// </summary>
        /// <param name="recordFrameDataButton">记录数据按钮对象</param>
        /// <param name="recordFrameDataImg">记录数据按钮图标</param>
        /// <param name="recordFlashImage">记录数据时显示的图片</param>
        /// <param name="recordTimeTextPanel">记录数据时显示的panel</param>
        private void changeStopRecordDataUI(ToggleButton recordFrameDataButton, Image recordFrameDataImg, Image recordFlashImage, StackPanel recordTimeTextPanel)
        {
            //停止记录计时
            if (record_timer != null)
            {
                record_timer.Stop();
                record_timer = null;
            }
            else
            {
                return; //当前没有记录数据则退出
            }
            //重置记录按钮状态  
            recordFrameDataButton.IsChecked = false;
            //还原按钮外观
            recordFrameDataImg.Source = new BitmapImage(new Uri(PATH_IMAGE_PLAY_RECORD, UriKind.Relative));
            recordFrameDataButton.ToolTip = TOOLTIP_BUTTON_RECORD_FRAMEDATA;

            //TODO 持续写入数据文件或者发送数据至网络

            //结束动画，隐藏记录时间
            recordFlashImage.BeginAnimation(TextBlock.OpacityProperty, null);
            recordTimeTextPanel.Visibility = Visibility.Hidden;

        }
    }
}
