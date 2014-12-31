using CardWorkbench.Models;
using CardWorkbench.Utils;
using CardWorkbench.Views.MenuControls;
using DevExpress.Mvvm;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.NavBar;
using DevExpress.Xpf.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CardWorkbench.ViewModels.MenuControls
{
    public class CardMenuConfigViewModel
    {
        //layoutManager
        public static readonly string DOCKLAYOUTMANAGER_NAME = "mainDockManager"; 
        //工作区document group 名称
        public static readonly string DOCUMENTGROUP_NAME = "documentContainer";
        //Ribbon标签栏及其组件名称
        public static readonly string RIBBONCONTROL_NAME = "ribbonControl";
        public static readonly string RIBBONPAGE_TOOLS_NAME = "toolsRibbonPage";  //监控设置页
        public static readonly string RIBBONPAGE_CHANNEL_NAME = "channelRibbonPage"; //通道设置页
        public static readonly string RIBBONPAGEGROUP_CHANNEL_DATA_HANDLE_NAME = "channelDataHandleRibbonGroup"; //数据处理设置组
        public static readonly string BAREDITITEM_CHANNEL_NAME = "channelNameEditItem"; //通道显示编辑框名称
        public static readonly string RIBBONPAGE_PLAYBACK_NAME = "playBackRibbonPage"; //回放设置页
        public static readonly string RIBBONPAGE_CONFIG_SIMULATOR_NAME = "simulatorRibbonPage"; //模拟器设置页
        public static readonly string BUTTON_STARTCHANNEL_NAME = "startChannelButton";   //通道开始按钮名称
        public static readonly string BUTTON_STOPCHANNEL_NAME = "stopChannelButton";   //通道停止按钮名称
        public static readonly string BUTTON_OPENCHANNELSETUP_NAME = "biOpen";   //加载通道配置按钮名称
        public static readonly string BUTTON_SAVECHANNELSETUP_NAME = "biSave";   //保存通道配置按钮名称
        
        #region 硬件设置对话框命令绑定

        /// <summary>
        /// 菜单双击命令
        /// </summary>
        public ICommand cardMenuDoubleClickCommand
        {
            get { return new DelegateCommand<MouseButtonEventArgs>(onCardMenuDoubleClick, x => { return true; }); }
        }

        private void onCardMenuDoubleClick(MouseButtonEventArgs e)
        {
            NavBarItem navBarItem = findNavBarItemByEventArgs(e);
            if (navBarItem != null)
            {
                FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);
                DockLayoutManager dockManager = (DockLayoutManager)LayoutHelper.FindElementByName(root, DOCKLAYOUTMANAGER_NAME);
                if (navBarItem.Name.Contains(MainWindowViewModel.NAVBARITEM_CHANNEL_NAME_PREFIX))   //如果是通道菜单项
                {
                    //通道设置Panel
                    UIControlHelper.createWorkDocumentPanel(dockManager, DOCUMENTGROUP_NAME, MainWindowViewModel.PANEL_CONFIG_CHANNEL_NAME, MainWindowViewModel.PANEL_CONFIG_CHANNEL_CAPTION, new McfsGui());
                    setChannelSetupButtonsEnabled(root);
                }
                else {
                    //模拟器设置Panel
                    UIControlHelper.createWorkDocumentPanel(dockManager, DOCUMENTGROUP_NAME, MainWindowViewModel.PANEL_CONFIG_SIMULATOR_NAME, MainWindowViewModel.PANEL_CONFIG_SIMULATOR_CAPTION, new SimulatorWizard());
                }
            }
            
        }

        /// <summary>
        /// 设置加载和另存为通道配置文件按钮开启
        /// </summary>
        /// <param name="root"></param>
        public static void  setChannelSetupButtonsEnabled(FrameworkElement root)
        {
            RibbonControl ribbonControl = (RibbonControl)LayoutHelper.FindElementByName(root, RIBBONCONTROL_NAME);
            RibbonPageGroup channelSetupRibbonPageGroup = ribbonControl.FindName(RIBBONPAGEGROUP_CHANNEL_DATA_HANDLE_NAME) as RibbonPageGroup;
            BarButtonItem openChannelSetupButton = channelSetupRibbonPageGroup.FindName(BUTTON_OPENCHANNELSETUP_NAME) as BarButtonItem;
            BarButtonItem saveChannelSetupButton = channelSetupRibbonPageGroup.FindName(BUTTON_SAVECHANNELSETUP_NAME) as BarButtonItem;
            openChannelSetupButton.IsEnabled = true;
            saveChannelSetupButton.IsEnabled = true;
        }

        /// <summary>
        /// 获取点击所在的NavBarItem
        /// </summary>
        /// <param name="e">点击事件</param>
        /// <returns>NavBarItem</returns>
        private NavBarItem findNavBarItemByEventArgs(MouseButtonEventArgs e) {
            NavBarControl navbarControl = e.Source as NavBarControl;
            //直接通过菜单栏根据点击事件获取菜单项
            NavBarItem navBarItem = navbarControl.View.GetNavBarItem(e);    
            //如果获取失败，尝试通过点击所在控件向上查找父亲控件类型是NavBarItem的菜单项
            if (navBarItem == null)
            {
                navBarItem = LayoutHelper.FindLayoutOrVisualParentObject<NavBarItem>(e.OriginalSource as DependencyObject, true);
            }
            return navBarItem;
        }

        /// <summary>
        /// 菜单点击命令
        /// </summary>
        public ICommand cardMenuItemClickCommand
        {
            get { return new DelegateCommand<NavBarControl>(onCardMenuItemClick, x => { return true; }); }
        }

        private void onCardMenuItemClick(NavBarControl navBarControl)
        {
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(navBarControl as DependencyObject);
            RibbonControl ribbonControl = (RibbonControl)LayoutHelper.FindElementByName(root, RIBBONCONTROL_NAME);
            RibbonPage channelRibbonPage = ribbonControl.Manager.FindName(RIBBONPAGE_CHANNEL_NAME) as RibbonPage;
            RibbonPage playBackRibbonPage = ribbonControl.Manager.FindName(RIBBONPAGE_PLAYBACK_NAME) as RibbonPage;
            RibbonPage configSimulatorRibbonPage = ribbonControl.Manager.FindName(RIBBONPAGE_CONFIG_SIMULATOR_NAME) as RibbonPage;
            NavBarItem selectItem = navBarControl.SelectedItem as NavBarItem;   //当前选中的NavBarItem
            if (selectItem.Name.Contains(MainWindowViewModel.NAVBARITEM_CHANNEL_NAME_PREFIX))    //选择项是通道item
            {
                string itemID = selectItem.Tag as string;
                string[] idStr = itemID.Split(new char[]{'-'});
                Channel selectChannel = null;
                if (idStr != null && idStr.Length > 1)
                {
                    selectChannel = DevicesManager.getChannelByID(idStr[0], idStr[1]);
                }
                if (selectChannel != null)
                {
                    //开启通道和回放的设置页，并获取焦点
                    playBackRibbonPage.IsEnabled = true;
                    channelRibbonPage.IsEnabled = true;
                    if (!channelRibbonPage.IsSelected)
                    {
                        channelRibbonPage.IsSelected = true;
                    }
                    RibbonPageGroup channelSetupRibbonPageGroup = ribbonControl.FindName(RIBBONPAGEGROUP_CHANNEL_DATA_HANDLE_NAME) as RibbonPageGroup;
                    BarEditItem taskBarEditItem = channelSetupRibbonPageGroup.FindName(BAREDITITEM_CHANNEL_NAME) as BarEditItem;
                    taskBarEditItem.EditValue = selectChannel.channelName;  //设置页上显示通道名称
                    //根据状态控制运行和停止通道按钮的显隐
                    if (selectChannel.channelStatus != null)
                    {
                       BarButtonItem startChannelButton = channelSetupRibbonPageGroup.FindName(BUTTON_STARTCHANNEL_NAME) as BarButtonItem; 
                       BarButtonItem stopChannelButton = channelSetupRibbonPageGroup.FindName(BUTTON_STOPCHANNEL_NAME) as BarButtonItem;
                       stopChannelButton.IsEnabled = selectChannel.channelStatus.bRun == MainWindowViewModel.CHANNELSTATUS_BRUN_ON ? true : false;
                       startChannelButton.IsEnabled = selectChannel.channelStatus.bRun == MainWindowViewModel.CHANNELSTATUS_BRUN_ON ? false : true;
                    }
                }
                
            }
            else if (selectItem.Name.Contains(MainWindowViewModel.NAVBARITEM_SIMULATOR_NAME_PREFIX)) //选择项是模拟器item
            {
                string simulatorID = selectItem.Tag as string;
                Simulator selectSimulaotr = DevicesManager.getSimulatorByDeviceID(simulatorID);
                if (selectSimulaotr != null)
                {
                    //开启模拟器设置页，并获取焦点
                    configSimulatorRibbonPage.IsEnabled = true;
                    channelRibbonPage.IsEnabled = false;
                    if (!configSimulatorRibbonPage.IsSelected)
                    {
                        configSimulatorRibbonPage.IsSelected = true;
                    }
                }
            }
           
        }

        #endregion
    }
}
