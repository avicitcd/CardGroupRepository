using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Docking;

namespace CardWorkbench.Views.CommonTools
{
    /// <summary>
    /// BitSyncControlPanel.xaml 的交互逻辑
    /// </summary>
    public partial class BitSyncControlPanel : UserControl
    {
        public BitSyncControlPanel()
        {
            InitializeComponent();
            this.LayoutPanel.Caption = "位同步控件";
        }
    }
}
