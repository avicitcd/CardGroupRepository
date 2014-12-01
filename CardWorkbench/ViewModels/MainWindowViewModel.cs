using CardWorkbench.Views.CommonControls;
using CardWorkbench.Views.CommonTools;
using CardWorkbench.Utils;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.NavBar;
using DevExpress.Xpf.Ribbon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CardWorkbench.Models;
using DevExpress.Xpf.Editors;
using System.Windows.Threading;
using System.Windows.Media;
using DevExpress.Data;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Bars;
using CardWorkbench.Views.MenuControls;
using System.Threading;
using DevExpress.Xpf.Core;
using CardWorkbench.ViewModels.CommonTools;
using System.Collections.ObjectModel;
using CardWorkbench.ViewModels.MenuControls;
using System.Windows.Media.Imaging;
using System.Collections;
using CardWorkbench.AcroInterface;
using DevExpress.Xpf.PropertyGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CardWorkbench.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        //控件名称
        public static readonly string TOOLBOX_TEXTCONTROL_NAME = "toolbox_textCtrl"; //文本
        public static readonly string TOOLBOX_LINECONTROL_NAME = "toolbox_lineCtrl"; //二维曲线
        public static readonly string TOOLBOX_METERCONTROL_NAME = "toolbox_meterCtrl"; //仪表
        public static readonly string TOOLBOX_LIGHTCONTROL_NAME = "toolbox_lightCtrl"; //工作灯
        public static readonly string TOOLBOX_TIMECONTROL_NAME = "toolbox_timeCtrl"; //时间
        //工作区document group 名称
        public static readonly string DOCUMENTGROUP_NAME = "documentContainer";
        //工作区自定义控件Canvas名称
        public static readonly string CANVAS_CUSTOM_CONTROL_NAME = "workCanvas";
        //Ribbon标签栏及其组件名称
        public static readonly string RIBBONCONTROL_NAME = "ribbonControl";
        public static readonly string RIBBONPAGE_TOOLS_NAME = "toolsRibbonPage";   //工具ribbonPage
        public static readonly string RIBBONPAGE_SETUP_NAME = "setupRibbonPage";  //设备ribbonPage
        public static readonly string RIBBONBARITEM_RESETDEVICE_NAME = "resetDeviceButton";  //"重置设备"按钮名
        //原始帧panel名称、标题 
        public static readonly string PANEL_FRAMEDUMP_NAME = "frameDumpPanel";
        public static readonly string PANEL_FRAMEDUMP_CAPTION = "原始帧显示";
        //配置通道panel名称、标题 
        public static readonly string PANEL_CONFIG_CHANNEL_NAME = "configChannelPanel";
        public static readonly string PANEL_CONFIG_CHANNEL_CAPTION = "通道配置";
        //配置模拟器panel名称、标题 
        public static readonly string PANEL_CONFIG_SIMULATOR_NAME = "configSimulatorPanel";
        public static readonly string PANEL_CONFIG_SIMULATOR_CAPTION = "模拟器配置";
        //自定义控件panel和document panel名称
        public static readonly string PANEL_CUSTOMCONTROL_NAME = "mainControl";
        public static readonly string DOCUMENTPANEL_WORKSTATE_NAME = "document1";
        //硬件设置菜单栏对话框名称
        public static readonly string DIALOG_HARDWAR_RECOGNITION_NAME = "hardwareRecognitionDialog";
        //硬件菜单栏
        public static readonly string NAVBARCONTROL_MENU_NAME = "menuNavcontrol";  //菜单控件名称
        public static readonly string NAVBARGROUP_DEVICE_NAME_PREFIX = "deviceNavBar"; //设备菜单组名称前缀
        public static readonly string NAVBARITEM_CHANNEL_NAME_PREFIX = "channelNavItem"; //通道菜单项名称前缀  
        public static readonly string NAVBARITEM_SIMULATOR_NAME_PREFIX = "simulatorNavItem"; //模拟器菜单项名称前缀
        public static readonly string NAVBARITEM_STACKPANEL_NAME_SUBFIX = "stackPanel"; //通道菜单项下stackPanel的名称后缀 
        public static readonly string NAVBARGROUP_IMAGE_URI_PATH = "pack://application:,,,/Images/hardware_32.png"; //设备菜单组图标资源路径
        public static readonly string NAVBARITEM_CHANNEL_OFF_URI_PATH = "pack://application:,,,/Images/channel_off.png"; //通道菜单项状态"关闭"图标资源路径
        public static readonly string NAVBARITEM_CHANNEL_ON_URI_PATH = "pack://application:,,,/Images/channel_on.png"; //通道菜单项状态"运行"图标资源路径
        public static readonly int CHANNELSTATUS_BRUN_ON = 1; //通道“运行”状态
        public static readonly string LABEL_NAVBARITEM_ON = " (运行中)";    //菜单项文本状态"运行"
        public static readonly string LABEL_NAVBARITEM_OFF = " (停止)";   //菜单项文本状态"停止"
        public static readonly string LABEL_NAVBARITEM_SIMULATOR_RUNA = " (运行A模式)";   //模拟器菜单项文本状态"RUNA"
        public static readonly string LABEL_NAVBARITEM_SIMULATOR_RUNB = " (运行B模式)";   //模拟器菜单项文本状态"RUNB"
        public static readonly string LABEL_NAVBARITEM_SIMULATOR_INSERTFRAME = " (运行插入帧模式)";   //模拟器菜单项文本状态"INSERTFRAME"
        public static readonly string NAVBARITEM_SIMULATOR_OFF_URI_PATH = "pack://application:,,,/Images/simulator_off.png"; //模拟器菜单项状态"关闭"图标资源路径
        public static readonly string NAVBARITEM_SIMULATOR_ON_URI_PATH = "pack://application:,,,/Images/simulator_on.png"; //模拟器菜单项状态"关闭"图标资源路径


        //注册服务声明
        public IDialogService hardwareRecognitionDialogService { get { return GetService<IDialogService>(DIALOG_HARDWAR_RECOGNITION_NAME); } }  //获得硬件识别对话框服务
        public IOpenFileDialogService OpenFileDialogService { get { return GetService<IOpenFileDialogService>(); } }  //获得文件选择对话框服务
        public ISplashScreenService SplashScreenService { get { return GetService<ISplashScreenService>(); } } //获得splash screen服务
        public IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }


        //参数grid某行是否拖拽开始
        bool _dragStarted = false;
        //参数grid面板table view的名称
        public static readonly string PARAM_GRID_TABLEVIEW_NAME = "paramGridTabelView";

        public MainWindowViewModel()
        {

        }


        #region 顶部功能菜单命令绑定

        /// <summary>
        /// 硬件识别对话框命令
        /// </summary>
        public ICommand hardwareRecognitionCommand
        {
            get { return new DelegateCommand<LayoutPanel>(onHardwareRecognitionClick, x => { return true; }); }
        }

        private void onHardwareRecognitionClick(LayoutPanel cardMenuPanel)
        {
            UICommand okCommand = new UICommand()
            {
                Caption = "添加",
                IsCancel = false,
                IsDefault = true,
                Command = new DelegateCommand<CancelEventArgs>(
                   x => { },
                   x =>
                   {
                       if (HardwareRecognitionViewModel.grid != null
                           && HardwareRecognitionViewModel.grid.SelectedItems.Count != 0)
                       {
                           return true;
                       }
                       return false;
                   }
                ),
            };
            UICommand cancelCommand = new UICommand()
            {
                Id = MessageBoxResult.Cancel,
                Caption = "取消",
                IsCancel = true,
                IsDefault = false,
            };
            UICommand result = hardwareRecognitionDialogService.ShowDialog(
                dialogCommands: new List<UICommand>() { okCommand, cancelCommand },
                title: "设备识别",
                viewModel: null
            );

            if (result == okCommand)
            {
                IList list = null;
                if (HardwareRecognitionViewModel.grid != null)
                {
                    list = HardwareRecognitionViewModel.grid.SelectedItems as IList;
                }

                onSelectHardwareLoad(cardMenuPanel, list);
            }
        }

        /// <summary>
        /// 加载选中板卡显示配置菜单命令
        /// </summary>
        /// <param name="cardMenuPanel">配置菜单layout panel</param>
        /// <param name="devicelist">设备清单列表</param>
        private void onSelectHardwareLoad(LayoutPanel cardMenuPanel, IList devicelist)
        {
            CardMenuConfig cardMenu = new CardMenuConfig();
            NavBarControl menuNavBarControl = cardMenu.FindName(NAVBARCONTROL_MENU_NAME) as NavBarControl;
            if (devicelist != null)
            {
                CardMenuConfigViewModel cardMenuViewModel = new CardMenuConfigViewModel();  //绑定命令所在的viewmodel
                IList<Device> currentDevicesList = DevicesManager.getCurrentDeviceListInstance();  //查看当前的设备清单列表
                if (currentDevicesList != null)
                {
                    currentDevicesList.Clear();
                }
                else
                {
                    currentDevicesList = new List<Device>();
                }
                foreach (var obj in devicelist)
                {
                    Device device = obj as Device;
                    currentDevicesList.Add(device);

                    //1.构建设备菜单组
                    NavBarGroup deviceGroup = buildDeviceNavBarGroup(device);
                    //2.构建通道菜单项
                    if (device.channelList != null)
                    {
                        foreach (Channel channel in device.channelList)
                        {
                            NavBarItem channelItem = buildChannelNavBarItem(device, channel, cardMenuViewModel, menuNavBarControl);
                            deviceGroup.Items.Add(channelItem);
                        }
                    }
                    //3.构建模拟器菜单项
                    if (device.simulator != null)
                    {
                        NavBarItem simulatorItem = buildSimulatorNavBarItem(device, device.simulator, cardMenuViewModel, menuNavBarControl);
                        deviceGroup.Items.Add(simulatorItem);
                    }

                    menuNavBarControl.Groups.Add(deviceGroup);    //设置设备菜单组到菜单控件
                }
                DevicesManager.setCurrentDeviceList(currentDevicesList);  //设置到全局变量
            }
            cardMenuPanel.Content = cardMenu; //添加到页面
            FrameworkElement root = LayoutHelper.GetRoot(cardMenuPanel);
            //开启ribbon工具标签页
            RibbonControl ribbonControl = (RibbonControl)LayoutHelper.FindElementByName(root, RIBBONCONTROL_NAME);
            RibbonPage toolsRibbonPage = ribbonControl.Manager.FindName(RIBBONPAGE_TOOLS_NAME) as RibbonPage;
            toolsRibbonPage.IsEnabled = true;
            //开启“重置设备按钮”
            RibbonPage setupRibbonPage = ribbonControl.Manager.FindName(RIBBONPAGE_SETUP_NAME) as RibbonPage;
            BarButtonItem resetDeviceBtn = setupRibbonPage.FindName(RIBBONBARITEM_RESETDEVICE_NAME) as BarButtonItem;
            resetDeviceBtn.IsEnabled = true;

            //获取“输出”panel的文本框并启动获取设备状态线程
            if (Constants.isDebugMode)
            {
                return;
            }
            TextBox logTextBox = UIControlHelper.getLogTextBox(root, cardMenuPanel);
            DeviceStatusManageThread.initDeviceStatusManageThread(menuNavBarControl, logTextBox);
        }

        /// <summary>
        /// 构建设备菜单组
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <returns>设备菜单组</returns>
        private NavBarGroup buildDeviceNavBarGroup(Device device)
        {
            NavBarGroup deviceGroup = new NavBarGroup();
            deviceGroup.Tag = device.deviceID;
            deviceGroup.Name = NAVBARGROUP_DEVICE_NAME_PREFIX + device.deviceID;
            deviceGroup.Header = device.deviceModel;
            BitmapImage myBitmapImage = new BitmapImage(new Uri(NAVBARGROUP_IMAGE_URI_PATH));
            deviceGroup.ImageSource = myBitmapImage;
            ImageSettings deviceItemImageSetting = new ImageSettings();
            deviceItemImageSetting.Height = 32;
            deviceItemImageSetting.Width = 32;
            deviceGroup.ItemImageSettings = deviceItemImageSetting;
            return deviceGroup;
        }

        /// <summary>
        /// 构建通道菜单项
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <param name="channel">通道对象</param>
        /// <param name="cardMenuViewModel">设备菜单viewmodel</param>
        /// <param name="menuNavBarControl">设备菜单navControl</param>
        /// <returns></returns>
        private NavBarItem buildChannelNavBarItem(Device device, Channel channel, CardMenuConfigViewModel cardMenuViewModel, NavBarControl menuNavBarControl)
        {
            NavBarItem channelItem = new NavBarItem();
            string itemID = device.deviceID + "-" + channel.channelID;
            string itemName = NAVBARITEM_CHANNEL_NAME_PREFIX + device.deviceID + channel.channelID; //通道item名称
            string navbarItem_channel_image_uri_path = channel.channelStatus == null ?    //菜单项图标名称
                NAVBARITEM_CHANNEL_OFF_URI_PATH : (channel.channelStatus.bRun == CHANNELSTATUS_BRUN_ON ?
                NAVBARITEM_CHANNEL_ON_URI_PATH : NAVBARITEM_CHANNEL_OFF_URI_PATH);

            //设置菜单项属性
            setNavBarItemProp(channelItem, itemID, itemName, navbarItem_channel_image_uri_path,
                channel.channelName, cardMenuViewModel.cardMenuItemClickCommand, menuNavBarControl);
            return channelItem;
        }

        /// <summary>
        /// 构建模拟器菜单项
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <param name="simulator">模拟器对象</param>
        /// <param name="cardMenuViewModel">设备菜单viewmodel</param>
        /// <param name="menuNavBarControl">设备菜单navControl</param>
        /// <returns></returns>
        private NavBarItem buildSimulatorNavBarItem(Device device, Simulator simulator, CardMenuConfigViewModel cardMenuViewModel, NavBarControl menuNavBarControl)
        {
            NavBarItem simulatorItem = new NavBarItem();
            string itemID = device.simulator.simulatorID;
            string itemName = NAVBARITEM_SIMULATOR_NAME_PREFIX + device.simulator.simulatorID; //模拟器item名称
            //设置菜单项属性
            string navbarItem_simulator_image_uri_path = NAVBARITEM_SIMULATOR_OFF_URI_PATH;  //TODO 根据状态设置显示图标
            setNavBarItemProp(simulatorItem, itemID, itemName, navbarItem_simulator_image_uri_path,
                device.simulator.simulatorName, cardMenuViewModel.cardMenuItemClickCommand, menuNavBarControl);
            return simulatorItem;
        }
        /// <summary>
        /// 设置菜单项属性
        /// </summary>
        /// <param name="item">菜单项对象</param>
        /// <param name="item_Name">菜单项控件名称</param>
        /// <param name="item_image_path">菜单项图标资源路径</param>
        /// <param name="item_label">菜单项显示名</param>
        /// <param name="item_Bind_Command">菜单项点击命令</param>
        /// <param name="command_Paramter">菜单项点击命令参数</param>
        private void setNavBarItemProp(NavBarItem item, string item_ID, string item_Name, string item_image_path, string item_label, ICommand item_Bind_Command, object command_Paramter)
        {
            item.Tag = item_ID;
            item.Name = item_Name;
            item.Command = item_Bind_Command;
            item.CommandParameter = command_Paramter;
            StackPanel stackPanel = new StackPanel();
            stackPanel.Name = item_Name + NAVBARITEM_STACKPANEL_NAME_SUBFIX;
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            Image image = new Image();
            BitmapImage imageSource = new BitmapImage(new Uri(item_image_path));
            image.Source = imageSource;
            Label itemLabel = new Label();
            itemLabel.VerticalAlignment = VerticalAlignment.Center;
            itemLabel.Margin = new Thickness(5, 0, 0, 0);
            itemLabel.Content = item_label;
            stackPanel.Children.Add(image);
            stackPanel.Children.Add(itemLabel);
            //显示菜单项状态文本
            if (NAVBARITEM_CHANNEL_ON_URI_PATH.Equals(item_image_path) || NAVBARITEM_SIMULATOR_ON_URI_PATH.Equals(item_image_path))
            {
                //stackPanel.ToolTip = TOOLTIP_NAVBARITEM_CHANNEL_ON;
                itemLabel.Content += LABEL_NAVBARITEM_ON;
            }
            else if (NAVBARITEM_CHANNEL_OFF_URI_PATH.Equals(item_image_path) || NAVBARITEM_SIMULATOR_OFF_URI_PATH.Equals(item_image_path))
            {
                //stackPanel.ToolTip = TOOLTIP_NAVBARITEM_CHANNEL_OFF;
                itemLabel.Content += LABEL_NAVBARITEM_OFF;
            }
            item.Content = stackPanel;
        }


        /// <summary>
        /// 打开读取本地硬件配置文件对话框  TODO:应修改为读取通道配置文件信息
        /// </summary>
        public ICommand openHardwareConfigCommand
        {
            get { return new DelegateCommand<LayoutPanel>(onOpenHardwareConfigClick, x => { return true; }); }
        }

        private void onOpenHardwareConfigClick(LayoutPanel cardMenuPanel)
        {
            OpenFileDialogService.Filter = "配置文件|*.xml";
            OpenFileDialogService.FilterIndex = 1;
            bool DialogResult = OpenFileDialogService.ShowDialog();
            if (!DialogResult)
            {
                //ResultFileName = string.Empty;
            }
            else
            {
                //onSelectHardwareLoad(cardMenuPanel);

                //IFileInfo file = OpenFileDialogService.Files.First();
                //ResultFileName = file.Name;
                //using (var stream = file.OpenText())
                //{
                //    FileBody = stream.ReadToEnd();
                //}
            }
        }

        /// <summary>
        /// ”通道配置“ 按钮Command
        /// </summary>
        public ICommand configChannelCommand
        {
            get { return new DelegateCommand<DockLayoutManager>(onConfigChannelClick, x => { return true; }); }
        }

        private void onConfigChannelClick(DockLayoutManager dockManager)
        {
            UIControlHelper.createWorkDocumentPanel(dockManager, DOCUMENTGROUP_NAME, PANEL_CONFIG_CHANNEL_NAME, PANEL_CONFIG_CHANNEL_CAPTION, new McfsGui());
        }

        /// <summary>
        /// 更新ribbonPage上通道按钮的显隐状态
        /// </summary>
        /// <param name="selectChannel">所选通道对象</param>
        /// <param name="isRun">通道是否运行</param>
        private void updateChannelRibbonPageBtnDisplay(Channel selectChannel, RibbonPage channelRibbonPage, bool isRun) 
        {
            //重设按钮显隐状态
            if (selectChannel.channelStatus != null)
            {
                BarButtonItem startChannelButton = channelRibbonPage.FindName(CardMenuConfigViewModel.BUTTON_STARTCHANNEL_NAME) as BarButtonItem;
                BarButtonItem stopChannelButton = channelRibbonPage.FindName(CardMenuConfigViewModel.BUTTON_STOPCHANNEL_NAME) as BarButtonItem;
                stopChannelButton.IsEnabled = isRun;
                startChannelButton.IsEnabled = !isRun;
            }
        }

        /// <summary>
        /// ”运行通道“ 按钮Command
        /// </summary>
        public ICommand runChannelCommand
        {
            get { return new DelegateCommand<RibbonControl>(onRunChannelClick, x => { return true; }); }
        }

        private void onRunChannelClick(RibbonControl channelRibbonControl)
        {
            string[] deviceAndChannelID = MainWindowViewModel.getSelectChannelInfo(channelRibbonControl);
            try
            {
                //运行通道
                acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                acro1626P.runChannel(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]));
            }
            catch (Exception ex)
            {
                TextBox logTextBox = UIControlHelper.getLogTextBox(null, channelRibbonControl);
                if (logTextBox != null)
                {
                    logTextBox.AppendText(ex.Message + "\n");
                }
            }
            //更新界面按钮状态
            Channel selectChannel = DevicesManager.getChannelByID(deviceAndChannelID[0], deviceAndChannelID[1]);
            RibbonPage channelRibbonPage = channelRibbonControl.Manager.FindName(CardMenuConfigViewModel.RIBBONPAGE_CHANNEL_NAME) as RibbonPage;
            updateChannelRibbonPageBtnDisplay(selectChannel, channelRibbonPage, true);
        }

        /// <summary>
        /// ”停止通道“ 按钮Command
        /// </summary>
        public ICommand stopChannelCommand
        {
            get { return new DelegateCommand<RibbonControl>(onStopChannelClick, x => { return true; }); }
        }

        private void onStopChannelClick(RibbonControl channelRibbonControl)
        {
            string[] deviceAndChannelID = MainWindowViewModel.getSelectChannelInfo(channelRibbonControl);
            try
            {
                //停止通道
                acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                acro1626P.stopChannel(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]));
            }
            catch (Exception ex)
            {
                TextBox logTextBox = UIControlHelper.getLogTextBox(null, channelRibbonControl);
                if (logTextBox != null)
                {
                    logTextBox.AppendText(ex.Message + "\n");
                }
            }
            //更新界面按钮状态
            Channel selectChannel = DevicesManager.getChannelByID(deviceAndChannelID[0], deviceAndChannelID[1]);
            RibbonPage channelRibbonPage = channelRibbonControl.Manager.FindName(CardMenuConfigViewModel.RIBBONPAGE_CHANNEL_NAME) as RibbonPage;
            updateChannelRibbonPageBtnDisplay(selectChannel, channelRibbonPage, false);
        }

        /// <summary>
        /// ”重置通道“ 按钮Command
        /// </summary>
        public ICommand resetChannelCommand
        {
            get { return new DelegateCommand<RibbonControl>(onResetChannelClick, x => { return true; }); }
        }

        private void onResetChannelClick(RibbonControl channelRibbonControl)
        {
            string[] deviceAndChannelID = MainWindowViewModel.getSelectChannelInfo(channelRibbonControl);
            try
            {
                //重置通道操作
                acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                acro1626P.resetChannel(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]));
                //重新读取一次通道配置
                string json = acro1626P.getChannelSetup(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]));
                var str = JObject.Parse(json).SelectToken(typeof(McfsControlRegisters).Name).ToString();
                //如果主界面已打开propertyGrid，则需要重置数据到grid
                FrameworkElement root = LayoutHelper.GetTopLevelVisual(channelRibbonControl as DependencyObject);
                DockLayoutManager dockManager = (DockLayoutManager)LayoutHelper.FindElementByName(root, CardMenuConfigViewModel.DOCKLAYOUTMANAGER_NAME);
                DocumentGroup documentGroup = dockManager.GetItem(CardMenuConfigViewModel.DOCUMENTGROUP_NAME) as DocumentGroup;
                DocumentPanel docPanel = dockManager.GetItem(PANEL_CONFIG_CHANNEL_NAME) as DocumentPanel;
                if (docPanel != null)
                {
                    McfsGui mcfsGui = docPanel.Content as McfsGui;
                    PropertyGridControl propertyGrid = mcfsGui.registerPropertyGrid;
                    if (propertyGrid != null)
                    {
                        propertyGrid.SelectedObject = JsonConvert.DeserializeObject<McfsControlRegisters>(str);
                    }
                }
                MessageBoxService.Show(
                       messageBoxText: "重置通道成功!",
                       caption: "信息",
                       button: MessageBoxButton.OK,
                       icon: MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBoxService.Show(
                       messageBoxText: "重置通道出错!",
                       caption: "错误",
                       button: MessageBoxButton.OK,
                       icon: MessageBoxImage.Error);
                TextBox logTextBox = UIControlHelper.getLogTextBox(null, channelRibbonControl);
                if (logTextBox != null)
                {
                    logTextBox.AppendText(ex.Message + "\n");
                }
            }
            //更新界面按钮状态
            Channel selectChannel = DevicesManager.getChannelByID(deviceAndChannelID[0], deviceAndChannelID[1]);
            RibbonPage channelRibbonPage = channelRibbonControl.Manager.FindName(CardMenuConfigViewModel.RIBBONPAGE_CHANNEL_NAME) as RibbonPage;
            updateChannelRibbonPageBtnDisplay(selectChannel, channelRibbonPage, false);
        }

        /// <summary>
        /// ”重置设备“ 按钮Command
        /// </summary>
        public ICommand resetDeviceCommand
        {
            get { return new DelegateCommand<LayoutPanel>(onResetDeviceClick, x => { return true; }); }
        }

        private void onResetDeviceClick(LayoutPanel cardMenuLayoutPanel)
        {
            CardMenuConfig cardMenuConfig = cardMenuLayoutPanel.Content as CardMenuConfig;
            if (cardMenuConfig != null)
            {
                NavBarGroup selectNavGroup = cardMenuConfig.menuNavcontrol.SelectedGroup as NavBarGroup;
                if (selectNavGroup != null)
                {
                    string deviceID = selectNavGroup.Tag as string;
                    try
                    {
                        //重置设备操作
                        acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                        acro1626P.resetDevice(int.Parse(deviceID));
                        //如果主界面已打开propertyGrid所在panel，则需要关闭此panel
                        FrameworkElement root = LayoutHelper.GetTopLevelVisual(cardMenuLayoutPanel as DependencyObject);
                        DockLayoutManager dockManager = (DockLayoutManager)LayoutHelper.FindElementByName(root, CardMenuConfigViewModel.DOCKLAYOUTMANAGER_NAME);
                        DocumentGroup documentGroup = dockManager.GetItem(CardMenuConfigViewModel.DOCUMENTGROUP_NAME) as DocumentGroup;
                        DocumentPanel docPanel = dockManager.GetItem(PANEL_CONFIG_CHANNEL_NAME) as DocumentPanel;
                        if (docPanel != null)
                        {
                            docPanel.Closed = true;
                        }
                        MessageBoxService.Show(
                        messageBoxText: "重置设备成功!",
                        caption: "信息",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxService.Show(
                        messageBoxText: "重置设备失败!",
                        caption: "错误",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                        TextBox logTextBox = UIControlHelper.getLogTextBox(null, cardMenuLayoutPanel);
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText(ex.Message + "\n");
                        }
                    }
                }
                else {
                    MessageBoxService.Show(
                        messageBoxText: "请在左侧【硬件设备】菜单中先展开需要重置的【设备】!",
                        caption: "警告",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Warning);
                    return;
                }
                
            }
        }

        /// <summary>
        /// 通过依赖组件获得当前选择通道的设备ID和通道ID信息
        /// </summary>
        /// <param name="dependencyObj">依赖组件</param>
        /// <returns></returns>
        public static string[] getSelectChannelInfo(DependencyObject dependencyObj)
        {
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(dependencyObj as DependencyObject);
            NavBarControl navBarControl = (NavBarControl)LayoutHelper.FindElementByName(root, NAVBARCONTROL_MENU_NAME);
            NavBarItem selectItem = navBarControl.SelectedItem as NavBarItem;   //当前选中的NavBarItem
            if (selectItem.Name.Contains(NAVBARITEM_CHANNEL_NAME_PREFIX))    //选择项是通道item
            {
                string itemID = selectItem.Tag as string;
                string[] idStr = itemID.Split(new char[] { '-' });
                return idStr;
            }
            else    //选择项是模拟器
            {
                NavBarGroup navBarGroup = navBarControl.SelectedGroup as NavBarGroup;
                string[] idStr = new string[] { navBarControl.Tag as string, "" };  //放入设备id
                return idStr;
            }
        }

        /// <summary>
        /// 通过依赖组件获得当前设备对应的模拟器ID
        /// </summary>
        /// <param name="dependencyObj">依赖组件</param>
        /// <returns></returns>
        public static string getSimulatorID(DependencyObject dependencyObj)
        {
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(dependencyObj as DependencyObject);
            NavBarControl navBarControl = (NavBarControl)LayoutHelper.FindElementByName(root, NAVBARCONTROL_MENU_NAME);
            NavBarItem selectItem = navBarControl.SelectedItem as NavBarItem;   //当前选中的NavBarItem
            if (selectItem.Name.Contains(NAVBARITEM_SIMULATOR_NAME_PREFIX))    //选择项是模拟器item
            {
                string itemID = selectItem.Tag as string;
                return itemID;
            }
            else    //选择项是模拟器
            {
                return null;
            }
        }

        /// <summary>
        /// ”模拟器配置“ 按钮Command
        /// </summary>
        public ICommand configSimulatorCommand
        {
            get { return new DelegateCommand<DockLayoutManager>(onConfigSimulatorClick, x => { return true; }); }
        }

        private void onConfigSimulatorClick(DockLayoutManager dockManager)
        {
            UIControlHelper.createWorkDocumentPanel(dockManager, DOCUMENTGROUP_NAME, PANEL_CONFIG_SIMULATOR_NAME, PANEL_CONFIG_SIMULATOR_CAPTION, new SimulatorWizard());
        }
        
        /// <summary>
        /// ”运行模拟器“ 按钮Command
        /// </summary>
        public ICommand startSimulatorCommand
        {
            get { return new DelegateCommand<LayoutPanel>(onStartSimulatorClick, x => { return true; }); }
        }

        private void onStartSimulatorClick(LayoutPanel cardMenuLayoutPanel)
        {
            CardMenuConfig cardMenuConfig = cardMenuLayoutPanel.Content as CardMenuConfig;
            if (cardMenuConfig != null)
            {
                NavBarGroup selectNavGroup = cardMenuConfig.menuNavcontrol.SelectedGroup as NavBarGroup;
                if (selectNavGroup != null)
                {
                    string deviceID = selectNavGroup.Tag as string;
                    try
                    {
                        //开启模拟器
                        acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                        acro1626P.setSimulatorStatus(int.Parse(deviceID), (int)SimulatorStatus.SIMULATORSTATUS.RUNA);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxService.Show(
                        messageBoxText: "启动模拟器失败!",
                        caption: "错误",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                        TextBox logTextBox = UIControlHelper.getLogTextBox(null, cardMenuLayoutPanel);
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText(ex.Message + "\n");
                        }
                    }
                }
                else
                {
                    MessageBoxService.Show(
                        messageBoxText: "请在左侧【硬件设备】菜单中先展开模拟器所属的【设备】!",
                        caption: "警告",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Warning);
                    return;
                }

            }
            //更新界面按钮状态
            //Channel selectChannel = DevicesManager.getChannelByID(deviceAndChannelID[0], deviceAndChannelID[1]);
            //RibbonPage channelRibbonPage = channelRibbonControl.Manager.FindName(CardMenuConfigViewModel.RIBBONPAGE_CHANNEL_NAME) as RibbonPage;
            //updateChannelRibbonPageBtnDisplay(selectChannel, channelRibbonPage, true);
        }

        /// <summary>
        /// ”停止模拟器“ 按钮Command
        /// </summary>
        public ICommand stopSimulatorCommand
        {
            get { return new DelegateCommand<LayoutPanel>(onStopSimulatorClick, x => { return true; }); }
        }

        private void onStopSimulatorClick(LayoutPanel cardMenuLayoutPanel)
        {
            CardMenuConfig cardMenuConfig = cardMenuLayoutPanel.Content as CardMenuConfig;
            if (cardMenuConfig != null)
            {
                NavBarGroup selectNavGroup = cardMenuConfig.menuNavcontrol.SelectedGroup as NavBarGroup;
                if (selectNavGroup != null)
                {
                    string deviceID = selectNavGroup.Tag as string;
                    try
                    {
                        //关闭模拟器
                        acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                        acro1626P.setSimulatorStatus(int.Parse(deviceID), (int)SimulatorStatus.SIMULATORSTATUS.IDLE);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxService.Show(
                        messageBoxText: "停止模拟器失败!",
                        caption: "错误",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                        TextBox logTextBox = UIControlHelper.getLogTextBox(null, cardMenuLayoutPanel);
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText(ex.Message + "\n");
                        }
                    }
                }
                else
                {
                    MessageBoxService.Show(
                        messageBoxText: "请在左侧【硬件设备】菜单中先展开模拟器所属的【设备】!",
                        caption: "警告",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Warning);
                    return;
                }

            }
        }

        /// <summary>
        /// ”原始帧显示“ 按钮Command
        /// </summary>
        public ICommand frameDumpCommand
        {
            get { return new DelegateCommand<DockLayoutManager>(onFrameDumpClick, x => { return true; }); }
        }

        private void onFrameDumpClick(DockLayoutManager dockManager)
        {
            UIControlHelper.createWorkDocumentPanel(dockManager, DOCUMENTGROUP_NAME, PANEL_FRAMEDUMP_NAME, PANEL_FRAMEDUMP_CAPTION, new FrameDump());
        }


        /// <summary>
        /// "自定义控件" 按钮Command
        /// </summary>
        public ICommand customControlCommand
        {
            get { return new DelegateCommand<DockLayoutManager>(onCustomControlClick, x => { return true; }); }
        }
        private void onCustomControlClick(DockLayoutManager dockManager)
        {
            DocumentGroup documentGroup = dockManager.GetItem(DOCUMENTGROUP_NAME) as DocumentGroup;
            DocumentPanel docPanel = dockManager.GetItem(PANEL_CUSTOMCONTROL_NAME) as DocumentPanel;
            if (docPanel != null && docPanel.IsClosed)
            {
                dockManager.DockController.Restore(docPanel);
            }
            dockManager.DockController.Activate(docPanel);
        }


        #endregion

        #region 控件栏拖拽命令绑定
        public ICommand dropToolBoxCommand
        {
            get { return new DelegateCommand<DragEventArgs>(onDropToolBoxNavItem, x => { return true; }); }
        }


        //拖拽事件
        private void onDropToolBoxNavItem(DragEventArgs e)
        {
            object originalSource = e.OriginalSource;
            NavBarItemControl item = LayoutHelper.FindParentObject<NavBarItemControl>(originalSource as DependencyObject);
            NavBarGroupHeader header = LayoutHelper.FindParentObject<NavBarGroupHeader>(originalSource as DependencyObject);
            NavBarItem data = e.Data.GetData(NavBarDragDropHelper.FormatName) as NavBarItem;
            if (data != null && data.Name != null)
            {
                string navItemName = data.Name;
                FrameworkElement root = LayoutHelper.GetTopLevelVisual(originalSource as DependencyObject);
                Canvas workCanvas = (Canvas)LayoutHelper.FindElementByName(root, CANVAS_CUSTOM_CONTROL_NAME);
                UserControl commonControl = null;
                int maxZindex = UIControlHelper.getMaxZIndexOfContainer(workCanvas);
                if (TOOLBOX_TEXTCONTROL_NAME.Equals(navItemName))   //文本控件
                {
                    commonControl = new TextControl();

                }
                else if (TOOLBOX_LINECONTROL_NAME.Equals(navItemName)) //曲线控件
                {
                    commonControl = new ChartControl();
                }
                else if (TOOLBOX_LIGHTCONTROL_NAME.Equals(navItemName)) //工作灯控件
                {
                    commonControl = new LampControl();
                }
                else if (TOOLBOX_METERCONTROL_NAME.Equals(navItemName))  //仪表控件
                {
                    commonControl = new CirleControl();
                }
                else if (TOOLBOX_TIMECONTROL_NAME.Equals(navItemName)) //时间控件
                {
                    commonControl = new TimeControl();
                }
                if (commonControl != null)
                {
                    Canvas.SetZIndex(commonControl, maxZindex + 1);
                    workCanvas.Children.Add(commonControl);
                }

            }
        }
        #endregion

        #region 参数Grid面板命令绑定
        public ICommand ParamGridMouseDownCommand
        {
            get { return new DelegateCommand<MouseButtonEventArgs>(onParamGridMouseDown, x => { return true; }); }
        }

        private void onParamGridMouseDown(MouseButtonEventArgs e)
        {
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);
            TableView paramTableView = (TableView)LayoutHelper.FindElementByName(root, PARAM_GRID_TABLEVIEW_NAME);
            int rowHandle = paramTableView.GetRowHandleByMouseEventArgs(e);
            if (rowHandle != GridDataController.InvalidRow)
                _dragStarted = true;
        }

        // 从grid上拖动数据到自定义panel控件上的命令
        //public ICommand ParamGridMouseMoveCommand
        //{
        //    get { return new DelegateCommand<MouseEventArgs>(onParamGridMouseMove, x => { return true; }); }
        //}

        //private void onParamGridMouseMove(MouseEventArgs e)
        //{
        //    FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);
        //    TableView paramTableView = (TableView)LayoutHelper.FindElementByName(root, PARAM_GRID_TABLEVIEW_NAME);
        //    //int rowHandle = paramTableView.GetRowHandleByMouseEventArgs(e);
        //    //RowControl rowControl = paramTableView.GetRowElementByRowHandle(rowHandle) as RowControl;

        //    //此处必须新建控件的dipatcher线程做处理，否则在拖动参数时会出现界面假死的情况
        //    paramTableView.Dispatcher.BeginInvoke(new Action(() =>
        //    {
        //        if (_dragStarted)
        //        {
        //            //Console.WriteLine("start..............");
        //            FrameworkElement element = paramTableView.GetRowElementByMouseEventArgs(e);
        //            RowData rowData = null;
        //            if (element != null)
        //            {
        //                rowData = element.DataContext as RowData;
        //                if (rowData == null)
        //                {
        //                    return;
        //                }
        //                DataObject data = CreateDataObject(rowData);
        //                DragDrop.DoDragDrop(element, data, DragDropEffects.Move | DragDropEffects.Copy);
        //            }
        //            _dragStarted = false;
        //            //Console.WriteLine("end..............");
        //        }
        //    }));

        //}

        //private DataObject CreateDataObject(RowData rowData)
        //{
        //    DataObject data = new DataObject();
        //    //data.SetData(typeof(int), rowHandle);
        //    data.SetData(typeof(RowData), rowData);
        //    return data;
        //}

        #endregion

        #region 自定义控件Panel按钮命令绑定
        public ICommand resetCommonControlsPanelCommand
        {
            get { return new DelegateCommand<Canvas>(onResetCommonControlsBtnClick, x => { return true; }); }
        }

        private void onResetCommonControlsBtnClick(Canvas commonControlsCanvas)
        {
            if (commonControlsCanvas != null)
            {
                commonControlsCanvas.Children.Clear();
            }
        }

        /// <summary>
        /// 控件键盘按键响应事件命令
        /// </summary>
        public ICommand CanvasDeleteKeyDownCommand
        {
            get { return new DelegateCommand<KeyEventArgs>(onCanvasDeleteKeyDown, x => { return true; }); }
        }

        private void onCanvasDeleteKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)  //删除按键
            {
                Canvas workCanvas = e.Source as Canvas;
                if (workCanvas == null)
                {
                    FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);
                    workCanvas = (Canvas)LayoutHelper.FindElementByName(root, CANVAS_CUSTOM_CONTROL_NAME);

                }
                var maxZ = UIControlHelper.getMaxZIndexOfContainer(workCanvas); //当前最顶层的控件

                if (workCanvas.Children.Count != 0)
                {
                    foreach (FrameworkElement childElement in workCanvas.Children)
                    {
                        if (Canvas.GetZIndex(childElement) == maxZ)
                        {
                            workCanvas.Children.Remove(childElement); //删除
                            break;
                        }

                    }

                }
            }
        }
        #endregion

        #region 输出Panel的按钮命令
        /// <summary>
        /// 清空日志panel的内容
        /// </summary>
        public ICommand clearOutputPanelCommand
        {
            get { return new DelegateCommand<DockLayoutManager>(onClearOutputPanelBtnClick, x => { return true; }); }
        }

        private void onClearOutputPanelBtnClick(DockLayoutManager dockLayoutManager)
        {
            if (dockLayoutManager != null)
            {
                TextBox logTextBox = UIControlHelper.getLogTextBox(null, dockLayoutManager);
                logTextBox.Clear();
            }
        }

        #endregion
    }
}
