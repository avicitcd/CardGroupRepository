using CardWorkbench.AcroInterface;
using CardWorkbench.Models;
using CardWorkbench.test;
using CardWorkbench.Utils;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.NavBar;
using DevExpress.Xpf.PropertyGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class McfsGuiViewModel : MenuControlsViewModel
    {
        public static readonly string NAVBARCONTROL_MENU_NAME = "menuNavcontrol";   //硬件设备菜单控件名称

        public McfsControlRegisters mcfsControlRegisters { get; set; }
        public InitializeWordProperties initializeWordProperties { get; set; }
        public ModifyWordProperty modifyWordProperties { get; set; }
        public IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }

        private bool isPropertyGridValid;  //配置项属性grid数据是否合法
        public McfsGuiViewModel()
        {
            isPropertyGridValid = true;
            initializeWordProperties = new InitializeWordProperties()
            {
                FrameLength = 32,
                MCFS_WPM_WORD_SIZE = MCFSWPMWORDSIZE.McfsWordSize16Bits,
                IDPosition =  1
            };
            modifyWordProperties = new ModifyWordProperty()
            {
                WordStart = 0,
                WordEnd = 1
            };
        }

        //syncPatternRegisters.McfsSyncPattern1 = Convert.ToInt32(syncPatternRegisters.McfsSyncPattern1, 16).ToString();
        //syncPatternRegisters.McfsSyncPattern1 = Convert.ToString(int.Parse(syncPatternRegisters.McfsSyncPattern1), 16);
        public ICommand enterInvalidCellExceptionCommand {
            get { return new DelegateCommand<InvalidCellExceptionEventArgs>(x => { isPropertyGridValid = false; }); }
        }
        public ICommand enterValidateCellCommand
        {
            get { return new DelegateCommand<ValidateCellEventArgs>(x => { isPropertyGridValid = true; }); }
        }
        
        /// <summary>
        /// 默认扩展属性命令
        /// </summary>
        public ICommand customExpandCommand
        {
            get { return new DelegateCommand<CustomExpandEventArgs>(onCustomExpand, x => { return true; }); }
        }

        private void onCustomExpand(CustomExpandEventArgs args)
        {
            args.IsExpanded = true;
            args.Handled = true;
        }

        /// <summary>
        /// 读取板卡配置参数指令
        /// </summary>
        public ICommand readChannelRegistersCommand
        {
            get { return new DelegateCommand<PropertyGridControl>(onReadChannelRegistersClick); }
        }
        private void onReadChannelRegistersClick(PropertyGridControl propertyGrid)
        {
            //获取通道注册的json值
            string json = "";
            string[] deviceAndChannelID = MainWindowViewModel.getSelectChannelInfo(propertyGrid);
            if (deviceAndChannelID == null || String.IsNullOrEmpty(deviceAndChannelID[deviceAndChannelID.Length-1]))   //没找到选择的设备和通道ID
            {
                MessageBoxService.Show(
                       messageBoxText: "无法读取配置信息，请在左侧【硬件设备】菜单中选择要读取的【通道】!",
                       caption: "警告",
                       button: MessageBoxButton.OK,
                       icon: MessageBoxImage.Warning);
                return;
            }
            if (Constants.isDebugMode)
            {
                json = ChannelRegisterSetupSimTest.getJsonStr(); //测试用的json字符串
            }
            else
            {
                try
                {
                    //从板卡接口获取当前的通道设置数据
                    acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                    json = acro1626P.getChannelSetup(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]));
                }
                catch (Exception ex)
                {
                    TextBox logTextBox = UIControlHelper.getLogTextBox(null, propertyGrid);
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText(ex.Message + "\n");
                    }
                }
            }

            var str = JObject.Parse(json).SelectToken(typeof(McfsControlRegisters).Name).ToString();
            mcfsControlRegisters = JsonConvert.DeserializeObject<McfsControlRegisters>(str);
            //设置数据到grid
            propertyGrid.SelectedObject = mcfsControlRegisters;

        }

        /// <summary>
        /// 写入板卡配置参数指令
        /// </summary>
        public ICommand writeChannelRegistersCommand
        {
            get { return new DelegateCommand<PropertyGridControl>(onWriteChannelRegistersClick); }
        }
        private void onWriteChannelRegistersClick(PropertyGridControl propertyGrid)
        {
            string json = "";
            if (mcfsControlRegisters != null)
            {
                string[] deviceAndChannelID = MainWindowViewModel.getSelectChannelInfo(propertyGrid);
                if (deviceAndChannelID == null || deviceAndChannelID.Length != 2)   //没找到选择的设备和通道ID
                {
                    MessageBoxService.Show(
                        messageBoxText: "无法写入配置信息，请在左侧【硬件设备】菜单中选择要写入的【通道】!",
                        caption:"警告",
                        button:MessageBoxButton.OK,
                        icon: MessageBoxImage.Warning);
                    return;
                }
                if (!isPropertyGridValid)
                {
                     MessageBoxService.Show(
                        messageBoxText: "无法写入配置信息，请检查填写的配置项是否符合要求!",
                        caption:"警告",
                        button:MessageBoxButton.OK,
                        icon: MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    json = JsonConvert.SerializeObject(mcfsControlRegisters, Formatting.Indented);  //序列化获得要设置的json数据
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    sb.Append("\"");
                    sb.Append(typeof(McfsControlRegisters).Name);
                    sb.Append("\"");
                    sb.Append(": ");
                    sb.Append(json);
                    sb.Append("}");                 

                    //调用板卡接口设置通道配置数据
                    acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                    acro1626P.setChannelSetup(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]), sb.ToString());
                    MessageBoxService.Show(
                             messageBoxText: "写入配置成功!",
                             caption: "提示",
                             button: MessageBoxButton.OK,
                             icon: MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBoxService.Show(
                        messageBoxText: "写入配置出错!",
                        caption: "错误",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    TextBox logTextBox = UIControlHelper.getLogTextBox(null, propertyGrid); //日志面板textbox
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText(ex.Message + "\n");
                    }
                }
            }
        }

        
        /// <summary>
        /// 初始化字属性指令
        /// </summary>
        public ICommand initWordPropertiesCommand
        {
            get { return new DelegateCommand<GroupBox>(onInitWordPropertiesClick); }
        }
        private void onInitWordPropertiesClick(GroupBox initWordPropertiesGroupBox)
        {
            string json = "";
            if (initializeWordProperties != null)
            {
                string[] deviceAndChannelID = MainWindowViewModel.getSelectChannelInfo(initWordPropertiesGroupBox);
                if (deviceAndChannelID == null || String.IsNullOrEmpty(deviceAndChannelID[deviceAndChannelID.Length-1]))   //没找到选择的设备和通道ID
                {
                    MessageBoxService.Show(
                        messageBoxText: "无法初始化字属性，请先在左侧【硬件设备】菜单中选择【通道】!",
                        caption:"警告",
                        button:MessageBoxButton.OK,
                        icon: MessageBoxImage.Warning);
                    return;
                }
                if (initializeWordProperties.FrameLength < 1 || initializeWordProperties.FrameLength > 65536)
                {
                     MessageBoxService.Show(
                        messageBoxText: "无法初始化字属性，【帧长】的设定必须是在1到65536之间!",
                        caption:"警告",
                        button:MessageBoxButton.OK,
                        icon: MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    initializeWordProperties.deviceID = deviceAndChannelID[0]; //关联设备id
                    initializeWordProperties.channelID = deviceAndChannelID[1]; //关联通道id
                    json = JsonConvert.SerializeObject(initializeWordProperties, Formatting.Indented);  //序列化获得要设置的json数据
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    sb.Append("\"");
                    sb.Append(typeof(InitializeWordProperties).Name);
                    sb.Append("\"");
                    sb.Append(": ");
                    sb.Append(json);
                    sb.Append("}");                 

                    //调用板卡接口初始化字属性数据
                    acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                    acro1626P.initWordPropertyMem(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]), sb.ToString());
                    MessageBoxService.Show(
                             messageBoxText: "初始化字属性成功!",
                             caption: "提示",
                             button: MessageBoxButton.OK,
                             icon: MessageBoxImage.Information);

                    WordPropertiesManager.addCurrentWordProperties2Dictionary(initializeWordProperties);  //放入全局字属性管理变量
                }
                catch (Exception ex)
                {
                    MessageBoxService.Show(
                        messageBoxText: "初始化字属性出错!",
                        caption: "错误",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    TextBox logTextBox = UIControlHelper.getLogTextBox(null, initWordPropertiesGroupBox); //日志面板textbox
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText(ex.Message + "\n");
                    }
                }
            }
        }

        
        /// <summary>
        /// 编辑字属性指令
        /// </summary>
        public ICommand modifyWordPropertiesCommand
        {
            get { return new DelegateCommand<GroupBox>(onModifyWordPropertiesClick); }
        }
        private void onModifyWordPropertiesClick(GroupBox modifyWordPropertiesGroupBox)
        {
            string json = "";
            if (modifyWordProperties != null)
            {
                string[] deviceAndChannelID = MainWindowViewModel.getSelectChannelInfo(modifyWordPropertiesGroupBox);
                if (deviceAndChannelID == null || String.IsNullOrEmpty(deviceAndChannelID[deviceAndChannelID.Length-1]))   //没找到选择的设备和通道ID
                {
                    MessageBoxService.Show(
                        messageBoxText: "无法初始化字属性，请先在左侧【硬件设备】菜单中选择【通道】!",
                        caption:"警告",
                        button:MessageBoxButton.OK,
                        icon: MessageBoxImage.Warning);
                    return;
                }
               
                try
                {
                    json = JsonConvert.SerializeObject(modifyWordProperties, Formatting.Indented);  //序列化获得要设置的json数据
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    sb.Append("\"");
                    sb.Append(typeof(ModifyWordProperty).Name);
                    sb.Append("\"");
                    sb.Append(": ");
                    sb.Append(json);
                    sb.Append("}");                 

                    //调用板卡接口更新字属性数据
                    acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                    acro1626P.setWordPropertiesMem(int.Parse(deviceAndChannelID[0]), int.Parse(deviceAndChannelID[1]), sb.ToString());
                    MessageBoxService.Show(
                             messageBoxText: "更新字属性成功!",
                             caption: "提示",
                             button: MessageBoxButton.OK,
                             icon: MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBoxService.Show(
                        messageBoxText: "更新字属性出错!",
                        caption: "错误",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    TextBox logTextBox = UIControlHelper.getLogTextBox(null, modifyWordPropertiesGroupBox); //日志面板textbox
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText(ex.Message + "\n");
                    }
                }
            }
        }
        

        
    }
}
