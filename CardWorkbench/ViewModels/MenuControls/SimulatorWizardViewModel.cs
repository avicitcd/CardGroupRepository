using CardWorkbench.AcroInterface;
using CardWorkbench.Models;
using CardWorkbench.Utils;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.PropertyGrid;
using DynamicBuilder;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace CardWorkbench.ViewModels.MenuControls
{
    public class SimulatorWizardViewModel : MenuControlsViewModel
    {

        public static readonly string PROPERTYGRID_SIMULATOR_NAME = "simulatorSettingPropGrid";

        public SimulatorSetup simulatorSetup { get; set; }

        public IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }

        public ISaveFileDialogService SaveFileDialogService { get { return GetService<ISaveFileDialogService>(); } }

        public IOpenFileDialogService OpenFileDialogService { get { return GetService<IOpenFileDialogService>(); } }

        public static readonly string DEFAULTEXT = "xml";

        public static readonly string DEFAULTFILENAME = "simSetupFile";

        public static readonly string FILTER = "Sim Setup Files|*.xml";

        public static readonly int FILTERINDEX = 1;
        public virtual bool DialogResult { get; protected set; }

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
            List<FixedWord> fixedWordList = new List<FixedWord>();
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
        /// 读取模拟器配置参数指令
        /// </summary>
        /// <returns></returns>
        public ICommand readSimulatorSetupCommand
        {
            get { return new DelegateCommand<PropertyGridControl>(onReadSimulatorSetupBtnClick); }
        }
        private void onReadSimulatorSetupBtnClick(PropertyGridControl propertyGrid) 
        {
            OpenFileDialogService.Filter = FILTER;
            OpenFileDialogService.FilterIndex = FILTERINDEX;
            DialogResult = OpenFileDialogService.ShowDialog();
            if (DialogResult)
            {
                var serializer = new XmlSerializer(typeof(SimulatorSetup));
                using (var reader = System.Xml.XmlReader.Create(OpenFileDialogService.GetFullFileName()))
                {
                   TextBox logTextBox = UIControlHelper.getLogTextBox(null, propertyGrid); //日志面板textbox
                    try
                    {
                        simulatorSetup = (SimulatorSetup)serializer.Deserialize(reader);
                        if (simulatorSetup != null)
                        {
                            propertyGrid.SelectedObject = simulatorSetup; //加载数据到模拟器配置grid
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxService.Show(
                          messageBoxText: "读取模拟器配置文件信息出错，请检查!",
                          caption: "错误",
                          button: MessageBoxButton.OK,
                          icon: MessageBoxImage.Error);
                        if (logTextBox != null)
                        {
                            logTextBox.AppendText(ex.Message + "\n");
                        }
                        return;
                    }
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText("读取模拟器配置成功!" + "\n");
                    }
                }
                
            }
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
        
        /// <summary>
        /// 另存为模拟器配置参数指令
        /// </summary>
        public ICommand saveSimulatorSetupCommand
        {
            get { return new DelegateCommand<PropertyGridControl>(onSaveSimulatorSetupBtnClick); }
        }
        private void onSaveSimulatorSetupBtnClick(PropertyGridControl propertyGrid)
        {
            SaveFileDialogService.DefaultExt = DEFAULTEXT;
            SaveFileDialogService.DefaultFileName = DEFAULTFILENAME;
            SaveFileDialogService.Filter = FILTER;
            SaveFileDialogService.FilterIndex = FILTERINDEX;
            DialogResult = SaveFileDialogService.ShowDialog();

            if (DialogResult)
            {
                TextBox logTextBox = UIControlHelper.getLogTextBox(null, propertyGrid); //日志面板textbox
                try
                {
                    DXSplashScreen.Show<SplashScreenView>(); //显示loading框
                    //保存xml到指定文件
                    string setupStr = buildSimulatorSetupFileStr();
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.LoadXml(setupStr);
                    doc.Save(SaveFileDialogService.GetFullFileName());
                    if (DXSplashScreen.IsActive)
                    {
                        DXSplashScreen.Close();
                    }
                }
                catch (Exception ex)
                {
                    if (DXSplashScreen.IsActive)
                    {
                        DXSplashScreen.Close();
                    }
                    if (logTextBox != null)
                    {
                        logTextBox.AppendText("另存为配置文件出错！" + ex.Message + "\n");
                    }
                    return;
                }
                if (logTextBox != null)
                {
                    logTextBox.AppendText("成功保存配置文件！" + SaveFileDialogService.GetFullFileName() + "\n");
                }
            }
            
        }

        /// <summary>
        /// 构建另存为模拟器配置字符串
        /// </summary>
        /// <returns></returns>
        private string buildSimulatorSetupFileStr() 
        {
            dynamic xml = new DynamicBuilder.Xml();
            xml.Declaration();
            xml.SimulatorSetup(Xml.Fragment(setup =>
            {
                //帧格式
                setup.FormatCreate(Xml.Fragment(fc =>
                {
                    fc.wordsize(simulatorSetup.formatCreate.wordsize);
                    fc.numberofwords(simulatorSetup.formatCreate.numberofwords);
                    fc.syncpattern16(simulatorSetup.formatCreate.syncpattern16);
                    fc.bitrate(simulatorSetup.formatCreate.bitrate);
                    fc.CODE_TYPE(simulatorSetup.formatCreate.CODE_TYPE.ToString());
                    fc.ORIENTATION(simulatorSetup.formatCreate._ORIENTATION.ToString());
                }));
                //固定字
                setup.fixedWordList(Xml.Fragment(fwl =>
                {
                    foreach (FixedWord fixword in simulatorSetup.fixedWordList)
                    {
                        setup.FixedWord(Xml.Fragment(fw =>
                        {
                            fw.valueNonFormat(fixword.valueNonFormat);
                            fw.wordnumber(fixword.wordnumber);
                            fw.wordinterval(fixword.wordinterval);
                        }));
                    }
                }));

                //波形器
                setup.waveformList(Xml.Fragment(wfl =>
                {
                    foreach (Waveform waveform in simulatorSetup.waveformList)
                    {
                        setup.Waveform(Xml.Fragment(wf =>
                        {
                            wf.isEnableXml(waveform.isEnable == true ? 1 : 0);
                            wf.waveformnumber(waveform.waveformnumber);
                            wf.wordnumber(waveform.wordnumber);
                            wf.wordinterval(waveform.wordinterval);
                            wf.rate(waveform.rate);
                            wf.WAVE_FORM(waveform.WAVE_FORM.ToString());
                            wf.DATATYPE(waveform._DATATYPE.ToString());
                        }));
                    }
                }));
                //计数器
                setup.counterList(Xml.Fragment(ctl =>
                {
                    foreach (Counter counter in simulatorSetup.counterList)
                    {
                        setup.Counter(Xml.Fragment(ct =>
                        {
                            ct.isEnableXml(counter.isEnable == true ? 1: 0);
                            ct.counternumber(counter.counternumber);
                            ct.wordnumber(counter.wordnumber);
                            ct.wordinterval(counter.wordinterval);
                            ct.preset(counter.preset);
                            ct.limit(counter.limit);
                        }));
                    }
                }));
            }));

            return xml.ToString(true);
        }

    }


    /// <summary>
    /// 固定字List新增item实例初始化类
    /// </summary>
    public class FixWordItemInitializer : IInstanceInitializer
    {
        public FixWordItemInitializer()
        {
        }

        object IInstanceInitializer.CreateInstance(TypeInfo type)
        {
            var item = new FixedWord();
            item.valueNonFormat = "1";
            item.wordnumber = 1;
            item.wordinterval = 0;
            return item;
        }


        IEnumerable<TypeInfo> IInstanceInitializer.Types
        {
            get
            {
                return new List<TypeInfo>() {
                        new TypeInfo(typeof(FixedWord), "New FixedWord"),
                    };
            }
        }
    }
}
