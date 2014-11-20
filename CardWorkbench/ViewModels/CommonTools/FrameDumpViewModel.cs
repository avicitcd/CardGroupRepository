using CardWorkbench.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections.Generic;
using System.Data;
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
        public readonly string TOOLTIP_BUTTON_RECORD_FRAMEDATA = "记录";
        public readonly string TOOLTIP_BUTTON_STOP_RECORD_FRAMEDATA = "停止记录";

        //FRAMEData的Grid名称
        public readonly string GRID_FRAMEDATA_NAME = "frameGrid";
        //显示"当前时间"的textbox名称
        public readonly string TEXTBOX_CURRENTTIME_NAME = "currentTimeTextbox";
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
        
        private DispatcherTimer record_timer = null;    //记录数据时的timer
        private DateTime startTime; //记录数据时的起始时间
        private TimeSpan timePassed, timeSinceLastStop; //记录间隔时间，自上次记录停止的间隔时间(暂时没用到)
        private TextBlock recordTimeText; //记录时间显示文本

        private DispatcherTimer timer = new DispatcherTimer();  //模拟grid数据刷新的timer
        GridControl frameDataGrid = null;
        int frameNum = 9;  //临时子帧号
        int fullFrameCount = 0;  //已接收的全帧个数
        int updateRowCount = 0; //更新的行
        int filterTempRow = 0;  //筛选后的更新行
        bool isTimerPause = false;  //刷新计时器是否暂停
        string filterFrameID = "ALL";  //筛选子帧ID的临时变量
        bool isReset = false; //是否重置接收数据

        //获得文件选择对话框服务
        public IOpenFileDialogService OpenFileDialogService { get { return GetService<IOpenFileDialogService>(); } }

        //获得保存文件对话框服务
        public ISaveFileDialogService SaveFileDialogService { get { return GetService<ISaveFileDialogService>(); } }  

        /// <summary>
        /// 原始帧显示模块打开加载后执行命令
        /// </summary>
        public ICommand frameDumpLoadedCommand {
            get { return new DelegateCommand<RoutedEventArgs>(onframeDumpLoaded, x => { return true; }); }
        }

        private void onframeDumpLoaded(RoutedEventArgs e)
        {
           
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);

            StackPanel recordTimeTextPanel = LayoutHelper.FindElementByName(root, PANEL_RECORDTIME_NAME) as StackPanel;

            TextBox currentTimeTextbox = recordTimeTextPanel.FindName(TEXTBOX_CURRENTTIME_NAME) as TextBox;
            DispatcherTimer _timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 1), DispatcherPriority.Normal, delegate
            {
                currentTimeTextbox.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }, currentTimeTextbox.Dispatcher);
        }

        /// <summary>
        /// 点击“开始”接收或播放数据命令
        /// </summary>
        public ICommand startFrameDataCommand
        {
            get { return new DelegateCommand<RoutedEventArgs>(onStartFrameDataClick, x => { return true; }); }
        }

        private void onStartFrameDataClick(RoutedEventArgs e)
        {
            //获得必要的界面对象
            ToggleButton startFrameData_btn = e.Source as ToggleButton;
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(startFrameData_btn as DependencyObject);
            ToggleButton recordFrameDataButton = LayoutHelper.FindElementByName(root, BUTTON_RECORD_FRAMEDATA_NAME) as ToggleButton;
            Image playFrameDataImg = startFrameData_btn.FindName(IMAGE_PLAY_FRAMEDATA_NAME) as Image;
            Image recordFrameDataImg = recordFrameDataButton.FindName(IMAGE_RECORD_FRAMEDATA_NAME) as Image;
            Image recordFlashImage = LayoutHelper.FindElementByName(root, IMAGE_RECORDTIME_NAME) as Image;
            StackPanel recordTimeTextPanel = LayoutHelper.FindElementByName(root, PANEL_RECORDTIME_NAME) as StackPanel;
            ComboBoxEdit comboBox = LayoutHelper.FindElementByName(root, COMBOBOX_CHANGEMODEL_NAME) as ComboBoxEdit;
            string modeComboBoxValue = comboBox.EditValue as string;
            frameDataGrid = (GridControl)LayoutHelper.FindElementByName(root, GRID_FRAMEDATA_NAME);

            if (startFrameData_btn.IsChecked == true)   //开始播放
            {
                //更改按钮外观
                playFrameDataImg.Source = new BitmapImage(new Uri(PATH_IMAGE_PLAY_PAUSE, UriKind.Relative));
                startFrameData_btn.ToolTip = TOOLTIP_BUTTON_PAUSE_FRAMEDATA;
                //当前模式是实时时记录按钮可点击
                if (COMBOBOX_CHANGEMODEL_REALTIME.Equals(modeComboBoxValue))
                {
                    recordFrameDataButton.IsEnabled = true;
                }

                //新建一个线程去更新datagrid
                //List<FrameModel> row_lst = frameDataGrid.ItemsSource as List<FrameModel>;
                //foreach (FrameModel rowModel in row_lst)
                //{
                //    UpdateFrameGridWorker thread_worker = new UpdateFrameGridWorker(rowModel);
                //    Thread workerThread = new Thread(thread_worker.DoWork);
                //    workerThread.Start();
                //    thread_worker.DoWork();
                //}
                timer.Interval = TimeSpan.FromMilliseconds(500);
                if (!isTimerPause)
                {
                    timer.Tick += new EventHandler(doWork);
                }
                timer.Start();
                
            }
            else   //暂停播放
            {
                //还原按钮外观
                playFrameDataImg.Source = new BitmapImage(new Uri(PATH_IMAGE_PLAY_PLAY, UriKind.Relative));
                startFrameData_btn.ToolTip = TOOLTIP_BUTTON_PLAY_FRAMEDATA;

                //如果正在记录则需停止记录
                if (recordFrameDataButton.IsChecked == true)
                {
                    stopRecordFrameData(recordFrameDataButton, recordFrameDataImg, recordFlashImage, recordTimeTextPanel);
                }
                recordFrameDataButton.IsEnabled = false; //记录按钮不可用
     
                //控制刷新计时器
                timer.Stop();
                isTimerPause = true; //暂停刷新
            }

        }

        /// <summary>
        /// "记录"数据按钮命令
        /// </summary>
        public ICommand recordFrameDataCommand
        {
            get { return new DelegateCommand<RoutedEventArgs>(onRecordFrameDataClick, x => { return true; }); }
        }

        private void onRecordFrameDataClick(RoutedEventArgs e)
        {
            ToggleButton recordFrameData_btn = e.Source as ToggleButton;
            Image recordFrameDataImg = recordFrameData_btn.FindName(IMAGE_RECORD_FRAMEDATA_NAME) as Image;
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(recordFrameData_btn as DependencyObject);
            StackPanel recordTimeTextPanel = LayoutHelper.FindElementByName(root, PANEL_RECORDTIME_NAME) as StackPanel;
            Image recordFlashImage = recordTimeTextPanel.FindName(IMAGE_RECORDTIME_NAME) as Image;
            recordTimeText = recordTimeTextPanel.FindName(TEXTBLOCK_RECORDTIME_NAME) as TextBlock;
            
            if (recordFrameData_btn.IsChecked == true)  //开始记录
            {
                //TODO 需要判断 1.当前是否有支持记录的设备 2.当前是否开始已开始运行实时数据

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
            else
            {
                //停止记录
                stopRecordFrameData(recordFrameData_btn, recordFrameDataImg, recordFlashImage, recordTimeTextPanel);
                //恢复记录按钮状态
                recordFrameData_btn.IsEnabled = true;
            }

        }

        /// <summary>
        /// 设置时间文本的handle函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void record_timer_tick(object sender, object e) {
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
        private string convertNumberString(int number) {
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
        /// 停止记录数据
        /// </summary>
        /// <param name="recordFrameDataButton">记录数据按钮对象</param>
        /// <param name="recordFrameDataImg">记录数据按钮图标</param>
        /// <param name="recordFlashImage">记录数据时显示的图片</param>
        /// <param name="recordTimeTextPanel">记录数据时显示的panel</param>
        private void stopRecordFrameData(ToggleButton recordFrameDataButton, Image recordFrameDataImg, Image recordFlashImage, StackPanel recordTimeTextPanel)
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
            //弹出记录文件“另存为...”对话框
            SaveFileDialogService.Filter = "数据文件|*.dat"; ;
            SaveFileDialogService.FilterIndex = 1;
            bool saveResult = SaveFileDialogService.ShowDialog();
            if (saveResult)
            {
                //TODO 写入数据文件
                MessageBox.Show("保存成功");
            }
            //结束动画，隐藏记录时间
            recordFlashImage.BeginAnimation(TextBlock.OpacityProperty, null);
            recordTimeTextPanel.Visibility = Visibility.Hidden;

        }

        private void doWork(object sender, EventArgs e)
        {
            List<FrameModel> row_lst = frameDataGrid.ItemsSource as List<FrameModel>;
            //开始更新datagrid
           // foreach (FrameModel rowModel in row_lst.OrderBy(rowModel => rowModel.FrameID))

            frameDataGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (frameNum % row_lst.Count == 1 )  //翻页
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
                    int ID = updateRowCount+1;
                    row_lst[updateRowCount] = new FrameModel() { FrameID = frameNum, SyncWord = "EB101", Word1 = "FF"+frameNum, Word2 = "FFF", Word3 = "FF"+frameNum, ID = ID + "", Word5 = "FFFF", Word6 = "FFFF", Word7 = "FFFF", Word8 = "FFFF", Word9 = "FFFF", Word10 = "FFFF", Word11 = "FFFF", Word12 = "FFFF" };
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

        /// <summary>
        /// 设置当前模式命令
        /// </summary>
        public ICommand changeModelCommand
        {
            get { return new DelegateCommand<RoutedEventArgs>(onChangeModel, x => { return true; }); }
        }

        private void onChangeModel(RoutedEventArgs e)
        {
            //TODO 1. 如有数据，则先停止暂停当前的数据播放

            //2.切换模式
            ComboBoxEdit comboBox = e.Source as ComboBoxEdit;
            string model = comboBox.EditValue as string;
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(comboBox as DependencyObject);
            LayoutGroup playBackSettingGroup = LayoutHelper.FindElementByName(root, LAYOUTGROUP_PLAYBACK_NAME) as LayoutGroup;
            ToggleButton recordFrameDataBtn = LayoutHelper.FindElementByName(root, BUTTON_RECORD_FRAMEDATA_NAME) as ToggleButton;
            ToggleButton startFrameData_btn = LayoutHelper.FindElementByName(root, BUTTON_PLAY_FRAMEDATA_NAME) as ToggleButton;
            Image playFrameDataImg = startFrameData_btn.FindName(IMAGE_PLAY_FRAMEDATA_NAME) as Image;
            Image recordFrameDataImg = recordFrameDataBtn.FindName(IMAGE_RECORD_FRAMEDATA_NAME) as Image;
            Image recordFlashImage = LayoutHelper.FindElementByName(root, IMAGE_RECORDTIME_NAME) as Image;
            StackPanel recordTimeTextPanel = LayoutHelper.FindElementByName(root, PANEL_RECORDTIME_NAME) as StackPanel;

            if (COMBOBOX_CHANGEMODEL_REALTIME.Equals(model))  //实时模式
            {
                //回放设置不可见
                playBackSettingGroup.IsEnabled = false;
                playBackSettingGroup.IsCollapsed = true;
               
            }
            else if (COMBOBOX_CHANGEMODEL_PLAYBACK.Equals(model)) //回放模式
            {
                //回放设置可见
                playBackSettingGroup.IsEnabled = true;
                playBackSettingGroup.IsCollapsed = false;
            }
            //暂停接收数据,如当前正在记录则弹出保存对话框
            pauseReceiveFrameData(startFrameData_btn, recordFrameDataBtn, playFrameDataImg, recordFrameDataImg,recordFlashImage, recordTimeTextPanel);
            //还原“播放”按钮状态
            startFrameData_btn.IsChecked = false;
        }

        /// <summary>
        /// 暂停接收数据,更改按钮状态。如当前正在记录则弹出保存对话框
        /// </summary>
        /// <param name="startFrameData_btn">播放数据按钮对象</param>
        /// <param name="recordFrameDataButton">记录数据按钮对象</param>
        /// <param name="playFrameDataImg">播放数据按钮图标</param>
        /// <param name="recordFrameDataImg">记录数据按钮图标</param>
        /// <param name="recordFlashImage">记录时间文本图标</param>
        /// <param name="recordTimeTextPanel">记录时间panel</param>
        private void pauseReceiveFrameData(ToggleButton startFrameData_btn, ToggleButton recordFrameDataButton, Image playFrameDataImg, Image recordFrameDataImg, Image recordFlashImage, StackPanel recordTimeTextPanel)
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
            //如果正在记录数据，提示另存为，隐藏记录时间文本
            if (recordFrameDataButton.IsChecked == true)
            {
                stopRecordFrameData(recordFrameDataButton, recordFrameDataImg, recordFlashImage, recordTimeTextPanel);
            }
            recordFrameDataButton.IsEnabled = false; //重置记录按钮状态        

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
            //System.Windows.Forms.FileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    be.EditValue = dialog.FileName;
            //}
            OpenFileDialogService.Filter = "数据文件|*.dat";
            OpenFileDialogService.FilterIndex = 1;
            bool DialogResult = OpenFileDialogService.ShowDialog();
            if (DialogResult)
            {
               IFileInfo file = OpenFileDialogService.Files.First();
               be.EditValue = file.Name;
            }
            
        }

    }

    //子线程刷新
    //public class UpdateFrameGridWorker
    //{
    //    List<FrameModel> row_lst;
    //    GridControl frameDataGrid;
    //    public UpdateFrameGridWorker(List<FrameModel> row_lst, GridControl frameDataGrid)
    //    {
    //        this.row_lst = row_lst;
    //        this.frameDataGrid = frameDataGrid;
    //    }

    //    public void DoWork()
    //    {
    //        //开始更新datagrid
    //       // foreach (FrameModel rowModel in row_lst.OrderBy(rowModel => rowModel.FrameID))
    //        frameDataGrid.Dispatcher.BeginInvoke(new Action(() =>
    //        {
    //            for (int i = 0; i < row_lst.Count; i++)
    //            {
    //                row_lst[i] = new FrameModel() { FrameID = 9, SyncWord = "EB101", Word1 = "FFFF", Word2 = "FFF", Word3 = "FFFF", ID = 9 + "", Word5 = "FFFF", Word6 = "FFFF", Word7 = "FFFF", Word8 = "FFFF", Word9 = "FFFF", Word10 = "FFFF", Word11 = "FFFF", Word12 = "FFFF" };
    //                frameDataGrid.RefreshRow(i);
    //            }
    //        }));
    //            //row.ItemArray = new object[] { 9, "EB101", "FFFF", "FFFF", "FFFF", 9 + "", "FFFF", "FFFF", "FFFF", "FFFF", "FFFF", "FFFF", "FFFF", "FFFF" };
    //    }
    //    public void RequestStop()
    //    {
    //        _shouldStop = true;
    //    }
    //    // Volatile is used as hint to the compiler that this data
    //    // member will be accessed by multiple threads.
    //    private volatile bool _shouldStop;
    //}

    //事件发生前转换类
    //public class StartFrameDataBtnClickEventArgsConverter : EventArgsConverterBase<RoutedEventArgs>
    //{
    //    //按钮文本常量
    //    public readonly string BUTTON_CONTENT_START = "开始"; 
    //    public readonly string BUTTON_CONTENT_STOP = "停止";


    //    protected override object Convert(RoutedEventArgs args)
    //    {
    //        GridControl frameDataGrid = null;
    //        ToggleButton startFrameData_btn = args.Source as ToggleButton;

    //        FrameworkElement root = LayoutHelper.GetTopLevelVisual(startFrameData_btn as DependencyObject);
    //        frameDataGrid = (GridControl)LayoutHelper.FindElementByName(root, "frameGrid");

    //        if (startFrameData_btn.IsChecked == true)
    //        {
    //            startFrameData_btn.Content = BUTTON_CONTENT_STOP;
    //            //frameDataGrid.ItemsSource = FrameTestCase.generateData();
    //        }
    //        else {
    //            startFrameData_btn.Content = BUTTON_CONTENT_START;
    //        }

    //        return frameDataGrid;
    //    }
    //}
}
