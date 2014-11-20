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
using DevExpress.Xpf.NavBar;
using DevExpress.Xpf.Docking;

namespace CardWorkbench.Views.CommonTools
{
    /// <summary>
    /// WareTreeControl.xaml 的交互逻辑
    /// </summary>
    public partial class WareTreeControl : UserControl
    {
        public AutoHideGroup TargetGroup { get; set; }
        //public ReceiverControlPanel ReceiverPanel{get;set;}
        public BitSyncControlPanel BitPanel { get; set; }
        public WareTreeControl(AutoHideGroup targetGroup)
        {
            InitializeComponent();
            this.TargetGroup = targetGroup;
            //this.ReceiverPanel = new ReceiverControlPanel();
            this.BitPanel = new BitSyncControlPanel();
        }

        private void NavBarGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NavBarGroup selectGroup = (NavBarGroup)this.navcontrol.View.GetNavBarGroup(e);
            NavBarItem item = (NavBarItem)this.navcontrol.View.GetNavBarItem(e);
            if(item==null||item.Content==null)
            {
                return;
            }
            string value = (string)item.Content;
            if ("接收机".Equals(value))
            {
                if(this.TargetGroup.Items.Count>0)
                {
                    LayoutPanel panel = (LayoutPanel)this.TargetGroup.Items[0];
                    panel.Visibility = System.Windows.Visibility.Hidden;
                }
                this.TargetGroup.Items.Clear();
                //this.TargetGroup.Items.Add(this.ReceiverPanel.LayoutPanel);
                //this.ReceiverPanel.LayoutPanel.Visibility = System.Windows.Visibility.Visible;
            }else if("位同步".Equals(value))
            {
                if (this.TargetGroup.Items.Count > 0)
                {
                    LayoutPanel panel = (LayoutPanel)this.TargetGroup.Items[0];
                    panel.Visibility = System.Windows.Visibility.Hidden;
                }
                this.TargetGroup.Items.Clear();
                this.TargetGroup.Items.Add(this.BitPanel.LayoutPanel);
                this.BitPanel.LayoutPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

    }
}
