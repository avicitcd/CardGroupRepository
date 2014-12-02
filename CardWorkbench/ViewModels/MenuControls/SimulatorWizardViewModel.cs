using CardWorkbench.AcroInterface;
using CardWorkbench.Models;
using CardWorkbench.Utils;
using DevExpress.Mvvm;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.PropertyGrid;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CardWorkbench.ViewModels.MenuControls
{
    public class SimulatorWizardViewModel : MenuControlsViewModel
    {

        public static readonly string PROPERTYGRID_SIMULATOR_NAME = "simulatorSettingPropGrid";

        public SimulatorSetup simulatorSetup { get; set; }

        public IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }

        private bool isPropertyGridValid = true;  //配置项属性grid数据是否合法

        public SimulatorWizardViewModel() {
            
        }
        public ICommand enterInvalidCellExceptionCommand
        {
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
        /// 配置项十进制值转16进制显示命令
        /// </summary>
        public ICommand transHexDisplayCommand
        {
            get { return new DelegateCommand<ItemClickEventArgs>(onTransHexDisplayBtnClick); }
        }

        private void onTransHexDisplayBtnClick(ItemClickEventArgs args)
        {
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(args.Item.Parent as DependencyObject);
            PropertyGridControl propertyGrid = (PropertyGridControl)LayoutHelper.FindElementByName(root, PROPERTYGRID_SIMULATOR_NAME);
            
            //checkItem按钮状态变更
            BarCheckItem hexCheckItem = args.Item as BarCheckItem;
            if (hexCheckItem.IsChecked == true)
            {
                //转16进制
                try
                {
                    long value = long.Parse(propertyGrid.SelectedPropertyValue as string);
                    string hexValue = Convert.ToString(value, 16);
                    propertyGrid.SetRowValueByRowPath(propertyGrid.SelectedPropertyPath, hexValue);
                }
                catch (Exception)
                {
                    MessageBoxService.Show(
                           messageBoxText: "当前输入值已经是16进制或非法值，无法转换显示，请检查输入的值!",
                           caption: "警告",
                           button: MessageBoxButton.OK,
                           icon: MessageBoxImage.Warning);
                    hexCheckItem.IsChecked = false; //重置checkItem状态
                    return;
                }
            }
            else
            {
                //转10进制
                try
                {
                    long value = Convert.ToInt64(propertyGrid.SelectedPropertyValue as string, 16);
                    propertyGrid.SetRowValueByRowPath(propertyGrid.SelectedPropertyPath, value);
                }
                catch (Exception)
                {
                    MessageBoxService.Show(
                           messageBoxText: "当前输入值不合法，无法转换显示，请检查输入的值!",
                           caption: "警告",
                           button: MessageBoxButton.OK,
                           icon: MessageBoxImage.Warning);
                    hexCheckItem.IsChecked = false; //重置checkItem状态
                    return;
                }
            }
            //BarManager barManager = args.Item.Parent as BarManager;
            //args.Item.IsEnabled = false; //所点击的按钮置灰
            //BarCheckItem hexCheckItem = barManager.Items[PROPERTYGRID_MENUBUTTON_FIXEDWORD_NORMAL_NAME] as BarCheckItem;
            //hexCheckItem.IsChecked = false;
            //hexCheckItem.IsEnabled = true; 
        }

        /// <summary>
        /// 波形频率值下拉项改变命令
        /// </summary>
        public ICommand waveformValueChangedCommand
        {
            get { return new DelegateCommand<EditValueChangedEventArgs>(onWaveformValueChanged); }
        }
        private void onWaveformValueChanged(EditValueChangedEventArgs e)
        {
           ComboBoxEdit valueComboxEdit = e.Source as ComboBoxEdit;
           CellEditorPresenter cellEditorPresenter = LayoutHelper.FindElementByType<CellEditorPresenter>(LayoutHelper.GetParent(valueComboxEdit) as StackPanel);
          // string waveformObjFullPath = cellEditorPresenter.RowData.Parent.FullPath;
           Waveform editingWaveform = cellEditorPresenter.RowData.Parent.Value as Waveform;
           editingWaveform.RATE_UNITS = (RATEUNITS)e.NewValue;
        }

        /// <summary>
        /// 帧格式比特率下拉项改变命令
        /// </summary>
        public ICommand bitRateValueChangedCommand
        {
            get { return new DelegateCommand<EditValueChangedEventArgs>(onBitRateValueChanged); }
        }
        private void onBitRateValueChanged(EditValueChangedEventArgs e)
        {
            ComboBoxEdit valueComboxEdit = e.Source as ComboBoxEdit;
            CellEditorPresenter cellEditorPresenter = LayoutHelper.FindElementByType<CellEditorPresenter>(LayoutHelper.GetParent(valueComboxEdit) as StackPanel);
            FormatCreate editformatCreate = cellEditorPresenter.RowData.Parent.Value as FormatCreate;
            editformatCreate.BITRATE_UNITS = (BITRATEUNITS)e.NewValue;
        }

        /// <summary>
        /// 加载模拟器配置项指令
        /// </summary>
        public ICommand loadSimulatorSetupCommand
        {
            get { return new DelegateCommand<PropertyGridControl>(onLoadSimulatorSetup); }
        }
        private void onLoadSimulatorSetup(PropertyGridControl propertyGrid)
        {
            //1.帧格式
            FormatCreate frameFormat = new FormatCreate()
            {
                wordsize = 16,
                numberofwords = 32,
                syncpattern16 = "eb90",
                bitrate = 2,
                BITRATE_UNITS = BITRATEUNITS.MHz,
                CODE_TYPE = CODETYPE.SsNRZL,
                _ORIENTATION = ORIENTATION.SsMSBWOrientation
            };
            //2.固定字
            IList<FixedWord> fixedWordList = new List<FixedWord>();
            //3.波形
            Waveform[] waveformArray = new Waveform[2];
            Waveform waveform1 = new Waveform()
            {
                isEnable = false,
                waveformnumber = 1,
                wordnumber = 13,
                wordinterval = 0,
                rate = 0.5,
                RATE_UNITS = RATEUNITS.Hz,
                WAVE_FORM = WAVEFORM.SsRamp,
                _DATATYPE = DATATYPE.SsOnesComp
            };
            Waveform waveform2 = new Waveform()
            {
                isEnable = false,
                waveformnumber = 2,
                wordnumber = 14,
                wordinterval = 0,
                rate = 0.5,
                RATE_UNITS = RATEUNITS.Hz,
                WAVE_FORM = WAVEFORM.SsRamp,
                _DATATYPE = DATATYPE.SsOnesComp
            };
            waveformArray[0] = waveform1;
            waveformArray[1] = waveform2;
            //4.计数器
            Counter[] counterArray = new Counter[2];
            Counter counter1 = new Counter()
            {
                isEnable = false,
                counternumber = 1,
                wordnumber = 11,
                wordinterval = 0,
                preset = 0,
                limit = 10
            };
            Counter counter2 = new Counter()
            {
                isEnable = false,
                counternumber = 2,
                wordnumber = 12,
                wordinterval = 0,
                preset = 0,
                limit = 10
            };
            counterArray[0] = counter1;
            counterArray[1] = counter2;
            //5.初始化propetyGrid
            SimulatorSetup simulatorInitial = new SimulatorSetup()
            {
                formatCreate = frameFormat,
                fixedWordList = fixedWordList,
                waveformList = waveformArray,
                counterList = counterArray
            };
            simulatorSetup = simulatorInitial;
            propertyGrid.SelectedObject = simulatorInitial;
        }

        /// <summary>
        /// 写入模拟器配置参数指令
        /// </summary>
        public ICommand writeSimulatorSetupCommand
        {
            get { return new DelegateCommand<PropertyGridControl>(onWriteSimulatorSetupBtnClick); }
        }
        private void onWriteSimulatorSetupBtnClick(PropertyGridControl propertyGrid)
        {
           //object obj = propertyGrid.GetRowValueByRowPath("waveformList.[1].rate");
            string json = "";
            if (simulatorSetup != null)
            {
                string simulatorID = MainWindowViewModel.getSimulatorID(propertyGrid);
                if (String.IsNullOrEmpty(simulatorID))   //没找到选择的设备
                {
                    MessageBoxService.Show(
                        messageBoxText: "无法写入配置信息，请在左侧【硬件设备】菜单中选择要写入的设备模拟器!",
                        caption: "警告",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Warning);
                    return;
                }
                if (!isPropertyGridValid)
                {
                    MessageBoxService.Show(
                       messageBoxText: "无法写入配置信息，请检查填写的配置项是否符合要求!",
                       caption: "警告",
                       button: MessageBoxButton.OK,
                       icon: MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    SimulatorSetup simulatorSetupCopy = simulatorSetup.DeepClone();
                    //1.对FormatCreate中的同步字转换成10进制传输
                    simulatorSetupCopy.formatCreate.syncpattern = Convert.ToInt32(simulatorSetup.formatCreate.syncpattern16, 16).ToString();
                    //校验比特率的范围
                    if (simulatorSetupCopy.formatCreate.BITRATE_UNITS == BITRATEUNITS.KHz)
                    {
                        simulatorSetupCopy.formatCreate.bitrate = simulatorSetupCopy.formatCreate.bitrate * 1000;
                    }
                    else if (simulatorSetupCopy.formatCreate.BITRATE_UNITS == BITRATEUNITS.MHz)
                    {
                        simulatorSetupCopy.formatCreate.bitrate = simulatorSetupCopy.formatCreate.bitrate * 1000 * 1000;

                    }
                    if (simulatorSetupCopy.formatCreate.bitrate < 0 || simulatorSetupCopy.formatCreate.bitrate > 128000000)
                    {
                        MessageBoxService.Show(
                          messageBoxText: "无法写入配置信息，请检查填写的帧位速率是否在0到128000000之间!",
                          caption: "警告",
                          button: MessageBoxButton.OK,
                          icon: MessageBoxImage.Warning);
                        return;
                    }
                    //2.对FixedWord中的值做转换，默认用户填的是16进制值，转成10进制进行传输
                    if (simulatorSetupCopy.fixedWordList != null)
                    {
                        foreach (FixedWord fixedWord in simulatorSetupCopy.fixedWordList)
                        {
                            try
                            {
                                //fixedWord.value = Convert.ToInt32(fixedWord.value16, 16);
                                fixedWord.value = long.Parse(fixedWord.valueNonFormat);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    //尝试转16进制
                                    fixedWord.value = Convert.ToInt32(fixedWord.valueNonFormat, 16);
                                }
                                catch (Exception)
                                {
                                     MessageBoxService.Show(
                                     messageBoxText: "无法写入配置信息，请检查填写的固定字的值是否符合要求!",
                                     caption: "警告",
                                     button: MessageBoxButton.OK,
                                     icon: MessageBoxImage.Warning);
                                    return;
                                }
                               
                            }
                        }
                    }
                    //3.对于没有开启enable的waveform和counter做筛选
                    Waveform[] waveforms = simulatorSetupCopy.waveformList;
                    IEnumerable<Waveform> waveformQuery = waveforms.Where(x => x.isEnable);
                    if (waveformQuery != null && waveformQuery.Count() != 0)
                    {
                        simulatorSetupCopy.waveformList = waveformQuery.ToArray();
                        //频率单位转换并校验范围
                        foreach (Waveform waveform in simulatorSetupCopy.waveformList)
                        {
                            if (waveform.RATE_UNITS == RATEUNITS.KHz)
                            {
                                waveform.rate = waveform.rate * 1000;
                            }
                            if (waveform.rate < 0 || waveform.rate > 12203)
                            {
                                MessageBoxService.Show(
                                  messageBoxText: "无法写入配置信息，请检查填写的波形器的频率值是否在0到12203之间!",
                                  caption: "警告",
                                  button: MessageBoxButton.OK,
                                  icon: MessageBoxImage.Warning);
                                return;
                            }
                        }
                    }
                    else {
                        simulatorSetupCopy.waveformList = null;
                    }
                    Counter[] counters = simulatorSetupCopy.counterList;
                    IEnumerable<Counter> counterQuery = counters.Where(x => x.isEnable);
                    if (counterQuery != null && counterQuery.Count() != 0)
                    {
                        simulatorSetupCopy.counterList = counterQuery.ToArray();
                    }
                    else
                    {
                        simulatorSetupCopy.counterList = null;
                    }

                    json = JsonConvert.SerializeObject(simulatorSetupCopy, Formatting.Indented);  //序列化获得要设置的json数据
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    sb.Append("\"");
                    sb.Append(typeof(SimulatorSetup).Name);
                    sb.Append("\"");
                    sb.Append(": ");
                    sb.Append(json);
                    sb.Append("}");

                    //调用板卡接口设置模拟器配置数据
                    acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();
                    acro1626P.setSimulator(int.Parse(simulatorID), sb.ToString());
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
    }
}
