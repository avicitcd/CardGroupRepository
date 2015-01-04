using CardWorkbench.AcroInterface;
using CardWorkbench.Models;
using CardWorkbench.Models.Data;
using CardWorkbench.Network;
using CardWorkbench.Utils;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CardWorkbench.ViewModels.CommonTools
{
    public class FrameDumpViewModel : ViewModelBase
    {
        //Button名称
        public readonly string BUTTON_PLAY_FRAMEDATA_NAME = "startFrameData_btn";
        public readonly string BUTTON_RECORD_FRAMEDATA_NAME = "recordFrameData_btn";
        //Button Image控件名称
        public readonly string IMAGE_PLAY_FRAMEDATA_NAME = "playFrameDataImg";
        public readonly string IMAGE_RECORD_FRAMEDATA_NAME = "recordFrameDataImg";
        //Image资源路径
        public readonly string PATH_IMAGE_PLAY_PAUSE = "/Images/play/pause.png";
        public readonly string PATH_IMAGE_PLAY_PLAY = "/Images/play/play.png";
        public readonly string PATH_IMAGE_PLAY_RECORD = "/Images/play/record.png";
        public readonly string PATH_IMAGE_PLAY_RECORDING = "/Images/play/recording.png";

        //按钮提示文本
        public readonly string TOOLTIP_BUTTON_PLAY_FRAMEDATA = "开始";
        public readonly string TOOLTIP_BUTTON_PAUSE_FRAMEDATA = "暂停";

        //FRAMEData的Grid名称
        public readonly string GRID_FRAMEDATA_NAME = "frameGrid";
        //显示"当前时间"的textbox名称
        //public readonly string TEXTBOX_CURRENTTIME_NAME = "currentTimeTextbox";
        //显示设备和通道的下拉框名称
        public static readonly string COMBOBOX_FRAMEDUMPDEVICE_NAME = "frameDumpDeviceCombobox";
        public static readonly string COMBOBOX_FRAMEDUMPDCHANNEL_NAME = "frameDumpChannelCombobox";
        //帧长、字长和帧深文本框名称
        public readonly string TEXTEDIT_FRAMESIZE_NAME = "frameDumpFrameSize";
        public readonly string TEXTEDIT_WORDSIZE_NAME = "frameDumpWordSize";
        public readonly string SPINEDIT_FRAMENUM_NAME = "frameDumpFrameNum";
        //模式切换下拉框名称
        public readonly string COMBOBOX_CHANGEMODEL_NAME = "modeChangeComboBox";
        //模式切换下拉选项名称
        public readonly string COMBOBOX_CHANGEMODEL_REALTIME = "实时";
        public readonly string COMBOBOX_CHANGEMODEL_PLAYBACK = "回放";
        //回放设置组名称
        public readonly string LAYOUTGROUP_PLAYBACK_NAME = "playBack_setting_group";
        //回放时间显示panel名称
        public readonly string PANEL_RECORDTIME_NAME = "recordTimePanel";
        //回放时间显示image名称
        public readonly string IMAGE_RECORDTIME_NAME = "recordTimeImage";
        //回放时间显示文本名称
        public readonly string TEXTBLOCK_RECORDTIME_NAME = "recordTimeText";
        //原始帧grid字属性列名前缀
        public static readonly string COLUMN_FRAMEDUMP_WORD_NAME_PREFIX = "Word";
        //原始帧grid子帧序号列名称
        public static readonly string COLUMN_FRAMEDUMP_FRAMEID_NAME = "FrameID";

        private DispatcherTimer timer = new DispatcherTimer();  //模拟grid数据刷新的timer
        GridControl frameDataGrid = null;
        int frameNum = 9;  //临时子帧号
        int fullFrameCount = 0;  //已接收的全帧个数
        int updateRowCount = 0; //更新的行
        int filterTempRow = 0;  //筛选后的更新行
        bool isTimerPause = false;  //刷新计时器是否暂停
        string filterFrameID = "ALL";  //筛选子帧ID的临时变量
        bool isReset = false; //是否重置接收数据
        private UdpRetrieveRecordDataClient udpRetrieveRecordDataClient { get; set; }

        //获得文件选择对话框服务
        public IOpenFileDialogService OpenFileDialogService { get { return GetService<IOpenFileDialogService>(); } }
        public IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }

        /// <summary>
        /// 原始帧显示模块打开加载后执行命令
        /// </summary>
        public ICommand frameDumpLoadedCommand {
            get { return new DelegateCommand<RoutedEventArgs>(onframeDumpLoaded, x => { return true; }); }
        }

        private void onframeDumpLoaded(RoutedEventArgs e)
        {
            if (DXSplashScreen.IsActive)
                DXSplashScreen.Close();
            IList<Device> currentDeviceList = DevicesManager.getCurrentDeviceListInstance();
            if (currentDeviceList != null)
            {
                //当前设备
                FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);
                ComboBoxEdit recordDeviceComboBoxEdit = LayoutHelper.FindElementByName(root, COMBOBOX_FRAMEDUMPDEVICE_NAME) as ComboBoxEdit;
                recordDeviceComboBoxEdit.ItemsSource = currentDeviceList;
            }
            //当前系统时间显示
            //FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);
            //TextBox currentTimeTextbox = LayoutHelper.FindElementByName(root, TEXTBOX_CURRENTTIME_NAME) as TextBox;
            //DispatcherTimer _timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 1), DispatcherPriority.Normal, delegate
            //{
            //    currentTimeTextbox.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //}, currentTimeTextbox.Dispatcher);
        }

        /// <summary>
        /// 原始帧模块点击关闭界面命令 
        /// </summary>
        public ICommand frameDumpCloseCommand
        {
            get { return new DelegateCommand<object>(onFrameDumpCloseClick, x => { return true; }); }
        }

        private void onFrameDumpCloseClick(object obj)
        {
            DocumentPanel panel = obj as DocumentPanel;
            //获得现在原始帧是否在运行的全局标志，如果在运行，则弹出提示
            if (UdpRetrieveRecordDataClient.udpReceive != null && UdpRetrieveRecordDataClient.timer.IsEnabled)
            {
                MessageBoxResult msgResult = DXMessageBox.Show("原始帧模块正在运行，确定关闭此界面?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (msgResult == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    FrameworkElement root = LayoutHelper.GetTopLevelVisual(panel as DependencyObject);
                    //判断开始的条件配置是否已满足
                    ComboBoxEdit recordDeviceComboBoxEdit = LayoutHelper.FindElementByName(root, COMBOBOX_FRAMEDUMPDEVICE_NAME) as ComboBoxEdit;
                    ComboBoxEdit recordChannelComboBoxEdit = LayoutHelper.FindElementByName(root, COMBOBOX_FRAMEDUMPDCHANNEL_NAME) as ComboBoxEdit;
                    string deviceID = recordDeviceComboBoxEdit.EditValue as string;
                    string channelID = recordChannelComboBoxEdit.EditValue as string;
                    if (!String.IsNullOrEmpty(deviceID) && !String.IsNullOrEmpty(channelID))
                    {
                        try
                        {
                            new UdpRetrieveRecordDataClient().stopFrameDumpReceiveData(int.Parse(deviceID), int.Parse(channelID));
                        }
                        catch (Exception ex)
                        {
                            MessageBoxService.Show(
                                messageBoxText: "关闭原始帧出错!",
                                caption: "错误",
                                button: MessageBoxButton.OK,
                                icon: MessageBoxImage.Error);
                            TextBox logTextBox = UIControlHelper.getLogTextBox(null, panel); //日志面板textbox
                            if (logTextBox != null)
                            {
                                logTextBox.AppendText(ex.Message + "\n");
                            }
                        }
                    }
                }
            }
            panel.Closed = true;
        }

        /// <summary>
        /// 设备下拉联动命令 
        /// </summary>
        public ICommand deviceSelectChangedCommand
        {
            get { return new DelegateCommand<EditValueChangedEventArgs>(onDeviceSelectChanged, x => { return true; }); }
        }

        private void onDeviceSelectChanged(EditValueChangedEventArgs e)
        {
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);
            ComboBoxEdit recordChannelComboBoxEdit = LayoutHelper.FindElementByName(root, COMBOBOX_FRAMEDUMPDCHANNEL_NAME) as ComboBoxEdit;
            recordChannelComboBoxEdit.EditValue = null;
            recordChannelComboBoxEdit.ItemsSource = null;
            string deviceID = e.NewValue as string;
            if (String.IsNullOrEmpty(deviceID))
            {
                return;
            }
            Device selectDevice = DevicesManager.getDeviceByID(deviceID) as Device;
            if (selectDevice != null)
            {
                IList<Channel> currentChannelList = selectDevice.channelList;
                if (currentChannelList != null)
                {
                    recordChannelComboBoxEdit.ItemsSource = currentChannelList;
                }
            }
        }
        /// <summary>
        /// 通道下拉联动命令 
        /// </summary>
        public ICommand channelSelectChangedCommand
        {
            get { return new DelegateCommand<EditValueChangedEventArgs>(onChannelSelectChanged, x => { return true; }); }
        }

        private void onChannelSelectChanged(EditValueChangedEventArgs e)
        {
            string channelID = e.NewValue as string;
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);
            ComboBoxEdit recordDeviceComboBoxEdit = LayoutHelper.FindElementByName(root, COMBOBOX_FRAMEDUMPDEVICE_NAME) as ComboBoxEdit;
            string deviceID = recordDeviceComboBoxEdit.EditValue as string;
            if (String.IsNullOrEmpty(deviceID) || String.IsNullOrEmpty(channelID))
            {
                return;
            }
            //重置帧长，字长
            TextEdit frameSizeTextEdit = LayoutHelper.FindElementByName(root, TEXTEDIT_FRAMESIZE_NAME) as TextEdit;
            TextEdit wordSizeTextEdit = LayoutHelper.FindElementByName(root, TEXTEDIT_WORDSIZE_NAME) as TextEdit;
            frameSizeTextEdit.EditValue = String.Empty;
            wordSizeTextEdit.EditValue = String.Empty;
            //获得当前设定通道的帧长，字长
            InitializeWordProperties wordProperties = WordPropertiesManager.findCurrentWordProperties(deviceID, channelID);
            ControlRegister controlRegister = findControlRegister(deviceID, channelID, root);
            if (wordProperties != null && controlRegister != null)
            {
                int wordSize = 0;
                int frameSize = wordProperties.FrameLength;
                int initWordSize = (int)wordProperties.MCFS_WPM_WORD_SIZE + 1;
                int messageWordSize = 16;
                if (controlRegister.MCFS_MESSAGE_WORD_LENGTH == CardWorkbench.Models.ControlRegister.MCFSMESSAGEWORDLENGTH.McfsMessageWordlength32)
	            {
                    messageWordSize = 32;
	            }
                wordSize = initWordSize > messageWordSize ? initWordSize : messageWordSize;
                //设置属性值
                frameSizeTextEdit.EditValue = frameSize.ToString();
                wordSizeTextEdit.EditValue = wordSize.ToString();
            }
        }

        /// <summary>
        /// 根据设备ID，通道ID找到寄存器对象信息
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <param name="channelID">通道ID</param>
        /// <param name="root">可视化root元素</param>
        /// <returns></returns>
        private ControlRegister findControlRegister(string deviceID, string channelID, FrameworkElement root)
        {
            string json = string.Empty;
            try
            {
                //从板卡接口获取当前的通道设置数据
                acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                json = acro1626P.getChannelSetup(int.Parse(deviceID), int.Parse(channelID));
            }
            catch (Exception ex)
            {
                TextBox logTextBox = UIControlHelper.getLogTextBox(null, root);
                if (logTextBox != null)
                {
                    logTextBox.AppendText(ex.Message + "\n");
                }
            }
            var str = JObject.Parse(json).SelectToken(typeof(McfsControlRegisters).Name).ToString();
            McfsControlRegisters  mcfsControlRegisters = JsonConvert.DeserializeObject<McfsControlRegisters>(str);
            if (mcfsControlRegisters != null)
            {
                return mcfsControlRegisters.ControlRegister;
            }
            return null;
        }

        /// <summary>
        /// 点击“开始”接收或播放数据命令
        /// </summary>
        public ICommand startFrameDataCommand
        {
            get { return new DelegateCommand<RoutedEventArgs>(onStartFrameDataClick, x => { return true; }); }
        }

        public void onStartFrameDataClick(RoutedEventArgs e)
        {
            ToggleButton startFrameData_btn = e.Source as ToggleButton;
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(startFrameData_btn as DependencyObject);
            //判断开始的条件配置是否已满足
            ComboBoxEdit recordDeviceComboBoxEdit = LayoutHelper.FindElementByName(root, COMBOBOX_FRAMEDUMPDEVICE_NAME) as ComboBoxEdit;
            ComboBoxEdit recordChannelComboBoxEdit = LayoutHelper.FindElementByName(root, COMBOBOX_FRAMEDUMPDCHANNEL_NAME) as ComboBoxEdit;
            TextEdit frameSizeTextEdit = LayoutHelper.FindElementByName(root, TEXTEDIT_FRAMESIZE_NAME) as TextEdit;
            TextEdit wordSizeTextEdit = LayoutHelper.FindElementByName(root, TEXTEDIT_WORDSIZE_NAME) as TextEdit;
            SpinEdit frameNumSpinEdit = LayoutHelper.FindElementByName(root, SPINEDIT_FRAMENUM_NAME) as SpinEdit;

            if (String.IsNullOrEmpty(recordDeviceComboBoxEdit.Text as string) || String.IsNullOrEmpty(recordChannelComboBoxEdit.Text as string) || 
                String.IsNullOrEmpty(frameSizeTextEdit.EditValue as string) || String.IsNullOrEmpty(wordSizeTextEdit.EditValue as string))
            {
                 MessageBoxService.Show(
                       messageBoxText: "无法开始原始帧显示，请检查是否已选择指定的【设备】和【通道】以及对应的字属性已初始化!",
                       caption: "警告",
                       button: MessageBoxButton.OK,
                       icon: MessageBoxImage.Warning);
                 startFrameData_btn.IsChecked = false;
                return;
            }

            //获得必要的界面对象
            Image playFrameDataImg = startFrameData_btn.FindName(IMAGE_PLAY_FRAMEDATA_NAME) as Image;
            frameDataGrid = (GridControl)LayoutHelper.FindElementByName(root, GRID_FRAMEDATA_NAME);
            string deviceID = recordDeviceComboBoxEdit.EditValue as string; //设备ID
            string channelID = recordChannelComboBoxEdit.EditValue as string; //通道ID

            if (startFrameData_btn.IsChecked == true)   //开始播放
            {
                //1.更改按钮外观
                playFrameDataImg.Source = new BitmapImage(new Uri(PATH_IMAGE_PLAY_PAUSE, UriKind.Relative));
                startFrameData_btn.ToolTip = TOOLTIP_BUTTON_PAUSE_FRAMEDATA;
                //2.构建grid的列
                int grid_columns_count = int.Parse(frameSizeTextEdit.EditValue as string); //帧长
                InitializeWordProperties wordProperty = WordPropertiesManager.findCurrentWordProperties(deviceID, channelID);    
                long IDPosition = 0;
                if (wordProperty != null)
                {
                    IDPosition = wordProperty.IDPosition; //获得ID字位置
                }
                buildFrameDumpGridColumn(frameDataGrid, grid_columns_count, IDPosition);
                //3.开始尝试接收数据
                int grid_word_size = int.Parse(wordSizeTextEdit.EditValue as string); //字长
                int grid_frame_num = (int)frameNumSpinEdit.Value; //帧深
                bool isStartDisplay = try2ReceiveUDPData(frameDataGrid, deviceID, channelID, grid_columns_count, grid_word_size, grid_frame_num);
                if (!isStartDisplay)
                {
                    //还原按钮外观
                    playFrameDataImg.Source = new BitmapImage(new Uri(PATH_IMAGE_PLAY_PLAY, UriKind.Relative));
                    startFrameData_btn.ToolTip = TOOLTIP_BUTTON_PLAY_FRAMEDATA;
                    startFrameData_btn.IsChecked = false; 
                    return;
                }
                
                //timer.Interval = TimeSpan.FromMilliseconds(0);
                //if (!isTimerPause)
                //{
                //    timer.Tick += new EventHandler(doWork);
                //}
                //timer.Start();
            }
            else   //暂停原始帧显示
            {
                stopReceiveUDPData(frameDataGrid, deviceID, channelID);
                //还原按钮外观
                playFrameDataImg.Source = new BitmapImage(new Uri(PATH_IMAGE_PLAY_PLAY, UriKind.Relative));
                startFrameData_btn.ToolTip = TOOLTIP_BUTTON_PLAY_FRAMEDATA;

                //控制刷新计时器
                //timer.Stop();
                //isTimerPause = true; //暂停刷新
            }
        }

        /// <summary>
        /// 构建原始帧grid
        /// </summary>
        /// <param name="frameDataGrid">grid对象</param>
        /// <param name="column_count">grid的字列个数</param>
        /// <param name="column_count">id字位置</param>
        private void buildFrameDumpGridColumn(GridControl frameDataGrid, int column_count, long IDPosition) 
        {
            frameDataGrid.Columns.Clear();
            GridColumn frameIDColumn = new GridColumn();
            frameIDColumn.Name = COLUMN_FRAMEDUMP_FRAMEID_NAME;
            frameIDColumn.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
            frameIDColumn.FieldName = COLUMN_FRAMEDUMP_FRAMEID_NAME;
            frameIDColumn.AllowResizing = DefaultBoolean.False;
            frameIDColumn.Header = "序号";
            frameIDColumn.VisibleIndex = 0;
            frameIDColumn.Fixed = FixedStyle.Left;
            frameIDColumn.Width = 60;
            frameIDColumn.Visible = true;
            frameIDColumn.Binding = new Binding("RowData.Row." + frameIDColumn.FieldName) { Mode = BindingMode.TwoWay };
            TextEditSettings textEditSetting = new TextEditSettings();
            textEditSetting.HorizontalContentAlignment = EditSettingsHorizontalAlignment.Center;
            frameIDColumn.EditSettings = textEditSetting;
            frameDataGrid.Columns.Add(frameIDColumn);   //加入子帧序号列
            for (int i = 1; i < column_count + 1; i++)
            {
                GridColumn word_column = new GridColumn();
                word_column.Name = COLUMN_FRAMEDUMP_WORD_NAME_PREFIX + i;
                word_column.Header = COLUMN_FRAMEDUMP_WORD_NAME_PREFIX + i;
                word_column.FieldName = COLUMN_FRAMEDUMP_WORD_NAME_PREFIX + i;
                word_column.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                word_column.VisibleIndex = i;
                word_column.Visible = true;
                word_column.Binding = new Binding("RowData.Row." + word_column.FieldName) { Mode = BindingMode.TwoWay };
                if (i == IDPosition)    //设定ID字列样式
                {
                    FrameworkElementFactory frameElementFactory = new FrameworkElementFactory(typeof(TextBlock));
                    frameElementFactory.SetBinding(TextBlock.TextProperty, new Binding("RowData.Row." + word_column.FieldName));
                    frameElementFactory.SetValue(TextBlock.BackgroundProperty, Brushes.Orange);
                    frameElementFactory.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
                    DataTemplate dataTemplate = new DataTemplate();
                    dataTemplate.VisualTree = frameElementFactory;
                    word_column.CellTemplate = dataTemplate;
                }

                frameDataGrid.Columns.Add(word_column); //加入子帧字属性列
            }
        }

        private bool try2ReceiveUDPData(GridControl frameDataGrid, string deviceID, string channelID, int grid_columns_count, int grid_word_size, int grid_frame_num) 
        {
            //校验当前是否有文件模式的数据记录功能在运行，有则不允许执行展示原始帧
            if (UdpRetrieveRecordDataClient.isFileRecording)
            {
                MessageBoxService.Show(
                   messageBoxText: "当前通道正在用【文件】模式记录数据，无法同时进行原始帧显示!",
                   caption: "警告",
                   button: MessageBoxButton.OK,
                   icon: MessageBoxImage.Warning);
                return false;
            }

            //校验现在是否有网络模式的数据记录功能在运行，有则不允许执行请求发送数据
            if (UdpRetrieveRecordDataClient.udpReceive == null)
            {
                //请求发送数据
                if (!requestSendData(frameDataGrid, deviceID, channelID))
	            {
                    return false;
	            }
            }
            //开始接收数据
            if (!receiveData(frameDataGrid, grid_columns_count, grid_word_size, grid_frame_num))
            {
                return false;
            }

            return true;
        }

        private bool requestSendData(GridControl frameDataGrid, string deviceID, string channelID) 
        {
            //发送地址
            string sendDataUrl = CommonUtils.getLocalIpAddress() + ":" + Constants.receiveDataPort;
            //初始化请求发送数据配置
            StartDataRecord startDataRecord = new StartDataRecord()
            {
                RecordLength = 32768,   //32768
                FileName = sendDataUrl,
                bControlRunState = 0,
                bEnableTime =  0,
                bEnableStatus = 0,
                bFrameMode = 0
            };
            //开始请求发送数据
            try
            {
                string json = JsonConvert.SerializeObject(startDataRecord, Formatting.Indented);  //序列化获得要设置的json数据
                StringBuilder sb = new StringBuilder();
                sb.Append("{").Append("\"").Append(typeof(StartDataRecord).Name)
                  .Append("\"").Append(": ").Append(json).Append("}");

                //调用板卡接口设置开始请求数据
                Acro1626pHelper.getCurrentAcro1626PInstance().startDataRecord(int.Parse(deviceID), int.Parse(channelID), sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBoxService.Show(
                    messageBoxText: "请求数据出错，请检查后重试!",
                    caption: "错误",
                    button: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error);
                TextBox logTextBox = UIControlHelper.getLogTextBox(null, frameDataGrid); //日志面板textbox
                if (logTextBox != null)
                {
                    logTextBox.AppendText(ex.Message + "\n");
                }
                //Acro1626pHelper.getCurrentAcro1626PInstance().stopDataRecord(int.Parse(deviceID), int.Parse(channelID), 0); //请求停止发送数据
                if (udpRetrieveRecordDataClient != null)
                {
                    udpRetrieveRecordDataClient.stopFrameDumpReceiveData(int.Parse(deviceID), int.Parse(channelID));
                }
                return false;
            }
            return true;
        }

        private bool receiveData(GridControl frameDataGrid, int grid_columns_count, int grid_word_size, int grid_frame_num) 
        {
            DataReceiver dataReceiver = null;
            try
            {
                if (UdpRetrieveRecordDataClient.writer == null || UdpRetrieveRecordDataClient.recordFs == null)  //获取当前全局dataReceiver对象
                {
                    dataReceiver = new DataReceiver();
                    dataReceiver.addr = CommonUtils.getLocalIpAddress();
                    dataReceiver.port = Constants.receiveDataPort;
                }
                else
                {
                    dataReceiver = UdpRetrieveRecordDataClient.dataReceiver;
                }

                udpRetrieveRecordDataClient = UdpRetrieveRecordDataClient.isMulticast == true ? new UdpRetrieveRecordDataClient() : new UdpRetrieveRecordDataClient(dataReceiver, false);
                udpRetrieveRecordDataClient.frameDumpReceiveData(frameDataGrid, grid_columns_count, grid_word_size, grid_frame_num);
            }
            catch (Exception ex)
            {
                MessageBoxService.Show(
                    messageBoxText: "接收数据时出错!",
                    caption: "错误",
                    button: MessageBoxButton.OK,
                    icon: MessageBoxImage.Error);
                TextBox logTextBox = UIControlHelper.getLogTextBox(null, frameDataGrid); //日志面板textbox
                if (logTextBox != null)
                {
                    logTextBox.AppendText(ex.Message + "\n");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 停止接收数据
        /// </summary>
        /// <param name="frameDataGrid">原始帧grid</param>
        /// <param name="deviceID">设备ID</param>
        /// <param name="channelID">通道ID</param>
        private void stopReceiveUDPData(GridControl frameDataGrid, string deviceID, string channelID) 
        {
            try
            {
                //Acro1626pHelper.getCurrentAcro1626PInstance().stopDataRecord(int.Parse(deviceID), int.Parse(channelID), 0);
                if (udpRetrieveRecordDataClient != null)
                {
                    udpRetrieveRecordDataClient.stopFrameDumpReceiveData(int.Parse(deviceID), int.Parse(channelID));
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.Show(
                     messageBoxText: "停止原始帧显示出错!",
                     caption: "错误",
                     button: MessageBoxButton.OK,
                     icon: MessageBoxImage.Error);
                TextBox logTextBox = UIControlHelper.getLogTextBox(null, frameDataGrid); //日志面板textbox
                if (logTextBox != null)
                {
                    logTextBox.AppendText(ex.Message + "\n");
                }
            }
        }

        /** demo展示数据
        private void doWork(object sender, EventArgs e)
        {
            List<FrameModel> row_lst = frameDataGrid.ItemsSource as List<FrameModel>;
            //开始更新datagrid
            // foreach (FrameModel rowModel in row_lst.OrderBy(rowModel => rowModel.FrameID))

            frameDataGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (frameNum % row_lst.Count == 1)  //翻页
                {
                    updateRowCount = 0; //还原成更新第一行
                    isReset = false; //还原重置标记
                    fullFrameCount += 1;  //接收到全帧数递增1
                }
                if (isReset)
                {
                    updateRowCount = 0; //还原成更新第一行
                    isReset = false; //还原重置标记
                }

                if ("ALL".Equals(this.filterFrameID))
                {
                    int ID = updateRowCount + 1;
                    row_lst[updateRowCount] = new FrameModel() { FrameID = frameNum, SyncWord = "EB101", Word1 = "FF" + frameNum, Word2 = "FFF", Word3 = "FF" + frameNum, ID = ID + "", Word5 = "FFFF", Word6 = "FFFF", Word7 = "FFFF", Word8 = "FFFF", Word9 = "FFFF", Word10 = "FFFF", Word11 = "FFFF", Word12 = "FFFF" };
                    frameDataGrid.RefreshRow(updateRowCount);
                }
                else
                {
                    int result = int.Parse(this.filterFrameID);
                    //Console.WriteLine("result:" + result);
                    //Console.WriteLine("frameNum:" + frameNum);
                    //Console.WriteLine("fullFrameCount:" + fullFrameCount);

                    if (frameNum == result || frameNum == result + row_lst.Count * fullFrameCount)  //所有是过滤值的才更新
                    {
                        if (filterTempRow % row_lst.Count == 0)
                        {
                            filterTempRow = 0;
                        }
                        //Console.WriteLine("update row:"+updateRowCount);
                        row_lst[filterTempRow] = new FrameModel() { FrameID = frameNum, SyncWord = "EB101", Word1 = "FF" + frameNum, Word2 = "FFF", Word3 = "FF" + frameNum, ID = result + "", Word5 = "FFFF", Word6 = "FFFF", Word7 = "FFFF", Word8 = "FFFF", Word9 = "FFFF", Word10 = "FFFF", Word11 = "FFFF", Word12 = "FFFF" };
                        frameDataGrid.RefreshRow(filterTempRow);
                        filterTempRow++;
                    }
                }
                updateRowCount++;
                frameNum++;

            }));
        }
        **/

        /**
        /// <summary>
        /// 过滤子帧号命令
        /// </summary>
        public ICommand filterFrameIDCommand {
            get { return new DelegateCommand<RoutedEventArgs>(onfilterFrameIDChanged, x => { return true; }); }
        }

        private void onfilterFrameIDChanged(RoutedEventArgs e) {
            ComboBoxEdit comboBox = e.Source as ComboBoxEdit;
            string filterId = comboBox.EditValue as string;
            filterFrameID = filterId;
            isReset = true;
        }
         * 
         * /

        /**
        /// <summary>
        /// 暂停接收数据,更改按钮状态。如当前正在记录则弹出保存对话框
        /// </summary>
        /// <param name="startFrameData_btn">播放数据按钮对象</param>
        /// <param name="recordFrameDataButton">记录数据按钮对象</param>
        /// <param name="playFrameDataImg">播放数据按钮图标</param>
        /// <param name="recordFrameDataImg">记录数据按钮图标</param>
        /// <param name="recordFlashImage">记录时间文本图标</param>
        /// <param name="recordTimeTextPanel">记录时间panel</param>
        private void pauseReceiveFrameData(ToggleButton startFrameData_btn, Image playFrameDataImg, Image recordFlashImage, StackPanel recordTimeTextPanel)
        {
            //如果正在接收数据，还原“播放”按钮状态和外观
            if (startFrameData_btn.IsChecked == true)
            {
                //还原“播放”按钮状态和外观
                playFrameDataImg.Source = new BitmapImage(new Uri(PATH_IMAGE_PLAY_PLAY, UriKind.Relative));
                startFrameData_btn.ToolTip = TOOLTIP_BUTTON_PLAY_FRAMEDATA;

                //TODO 暂停播放数据
                
                //----临时停止刷新计时器----
                    timer.Stop();
                    isTimerPause = true; //暂停刷新
                //-----
            }
           
        }

        /// <summary>
        /// 选择回放文件按钮命令
        /// </summary>
        public ICommand selectPlayBackDatafileCommand
        {
            get { return new DelegateCommand<RoutedEventArgs>(onSelectPlayBackDatafile, x => { return true; }); }
        }
        private void onSelectPlayBackDatafile(RoutedEventArgs e)
        {
            Button buttonInfo = e.Source as Button;
            ButtonEdit be = LayoutHelper.FindLayoutOrVisualParentObject<ButtonEdit>(buttonInfo as DependencyObject);
            if (be == null)
                return;
            
            OpenFileDialogService.Filter = "数据文件|*.dat";
            OpenFileDialogService.FilterIndex = 1;
            bool DialogResult = OpenFileDialogService.ShowDialog();
            if (DialogResult)
            {
               IFileInfo file = OpenFileDialogService.Files.First();
               be.EditValue = file.Name;
            }
            
        }
        **/

    }

}
