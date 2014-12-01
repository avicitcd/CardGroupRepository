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
using CardWorkbench.Views.CommonTools;
using CardWorkbench.Views.CommonControls;
using CardWorkbench.Views.MenuControls;
using DevExpress.Xpf.NavBar;
using DevExpress.Xpf.Ribbon;
using CardWorkbench.Utils;

namespace CardWorkbench
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : DXRibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            //new NavBarDragDropHelper(toolBoxNavBar);  //注册右侧导航工具栏可拖动

        }

    }
}
