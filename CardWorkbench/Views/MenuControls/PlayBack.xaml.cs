using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CardWorkbench.Views.MenuControls
{
    /// <summary>
    /// PlayBack.xaml 的交互逻辑
    /// </summary>
    public partial class PlayBack : UserControl
    {
        public PlayBack()
        {
            InitializeComponent();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            ButtonEdit be = LayoutHelper.FindLayoutOrVisualParentObject<ButtonEdit>(sender as DependencyObject, true);
            if (be == null)
                return;
            System.Windows.Forms.FileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                be.EditValue = dialog.FileName;
            }
        }
    }
}
