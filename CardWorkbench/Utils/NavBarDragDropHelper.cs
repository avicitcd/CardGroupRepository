using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.NavBar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CardWorkbench.Utils
{
    /// <summary>
    /// 导航栏拖放工具类
    /// </summary>
    class NavBarDragDropHelper
    {
        private System.Windows.Point _MouseDownLocation;
        private readonly NavBarControl _NavBar;
        public const string FormatName = "NavBarDragFormat";  //定义一个drag动作临时DataObject名称



        public NavBarDragDropHelper(NavBarControl navBar)
        {
            _NavBar = navBar;
            _NavBar.PreviewMouseLeftButtonDown += _NavBar_PreviewMouseLeftButtonDown;   //左键按下方法
            _NavBar.PreviewMouseMove += _NavBar_PreviewMouseMove;   //鼠标移动方法
            _NavBar.AllowDrop = false;  //工具导航条本身不允许放置
            //_NavBar.DragOver += _NavBar_DragOver;
            //_NavBar.Drop += _NavBar_Drop;
        }




        void _NavBar_DragOver(object sender, DragEventArgs e)
        {
            //e.Effects = DragDropEffects.Copy;
            if (!e.Data.GetDataPresent(FormatName))
            {
                e.Effects = DragDropEffects.None;
            }
        }

        void _NavBar_Drop(object sender, DragEventArgs e)
        {
            object originalSource = e.OriginalSource;
            NavBarItemControl item = LayoutHelper.FindParentObject<NavBarItemControl>(originalSource as DependencyObject);
            NavBarGroupHeader header = LayoutHelper.FindParentObject<NavBarGroupHeader>(originalSource as DependencyObject);
            NavBarItem data = e.Data.GetData(FormatName) as NavBarItem;
            if (data != null)
            {
                NavBarItem navItem = item == null ? null : item.DataContext as NavBarItem;
                NavBarGroup group = navItem == null ? header.DataContext as NavBarGroup : navItem.Group;
                Console.WriteLine(navItem);

                OnDragDrop(navItem, group, data);
            }
        }

        void _NavBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _MouseDownLocation = e.GetPosition(null);
        }

        void _NavBar_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && CheckDiff(_MouseDownLocation - e.GetPosition(null)))
            {
                NavBarItemControl item = e.OriginalSource as NavBarItemControl;
                if (item == null)
                {
                    return;
                }
                NavBarItem navBarItem = item.DataContext as NavBarItem;
                DataObject dragData = new DataObject(FormatName, navBarItem);
                DragDrop.DoDragDrop(_NavBar, dragData, DragDropEffects.Copy);
            }
        }

        private static bool CheckDiff(Vector diff)
        {
            return (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                                           Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance);
        }

        private void OnDragDrop(NavBarItem targetItem, NavBarGroup targetGroup, NavBarItem data)
        {
            DataRowView dataRowView = data.DataContext as DataRowView;
            DataTable sourceTable = dataRowView.DataView.Table;
            DataTable targetTable = (targetGroup.NavBar.ItemsSource as DataView).Table;
            DataRowView targetDataRow = targetItem == null ? null : targetItem.DataContext as DataRowView;
            int targetIndex = targetItem == null ? targetTable.Rows.Count : targetTable.Rows.IndexOf(targetDataRow.Row);
            DataRow newRow = targetTable.NewRow();
            newRow["Group"] = targetGroup.DataContext.ToString();
            newRow["Item"] = dataRowView["Item"];
            targetTable.Rows.InsertAt(newRow, targetIndex);
            sourceTable.Rows.Remove(dataRowView.Row);
            dataRowView.Delete();
            sourceTable.AcceptChanges();
        }
    }
}
