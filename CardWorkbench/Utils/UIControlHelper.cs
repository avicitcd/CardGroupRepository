using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CardWorkbench.Utils
{
    /// <summary>
    /// 界面UI控件工具类
    /// </summary>
    public class UIControlHelper
    {
        //主界面dockManager名称
        public static readonly string DOCKMANAGER_NAME = "mainDockManager";
        //“输出”panel名称
        public static readonly string LAYOUTPANEL_LOG_NAME = "logPanel";

        /// <summary>
        /// 使点击的控件在容器中置顶
        /// </summary>
        /// <param name="e">鼠标点击event参数</param>
        /// <param name="containerName">容器名称</param>
        public static void setClickedUserControl2TopIndex(MouseButtonEventArgs e,string containerName)
        {
            UserControl parentControl = LayoutHelper.FindParentObject<UserControl>(e.Source as DependencyObject);
            FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);
            Canvas workCanvas = (Canvas)LayoutHelper.FindElementByName(root, containerName);
            BringToFront(workCanvas as FrameworkElement, parentControl);
        }
        /// <summary>
        /// 获得容器中顶层控件的zindex值
        /// </summary>
        /// <param name="element">容器</param>
        /// <returns></returns>
        public static int getMaxZIndexOfContainer(FrameworkElement element)
        {
            if (element == null) return -1;

            Canvas canvas = element as Canvas;
            if (canvas == null) return -1;

            if (canvas.Children == null || canvas.Children.Count == 0)
            {
                return 0;
            }

            var maxZ = canvas.Children.OfType<UIElement>()
                //.Where(x => x != pane)
             .Select(x => Canvas.GetZIndex(x))
             .Max();

            return maxZ;
        }

        /// <summary>
        /// 在容器中置顶指定控件
        /// </summary>
        /// <param name="element">容器</param>
        /// <param name="pane">需要置顶的控件</param>
        public static void BringToFront(FrameworkElement element, Control pane)
        {
            int maxZ = getMaxZIndexOfContainer(element);
            if (maxZ == -1)
            {
                return;
            }
            Canvas.SetZIndex(pane, maxZ + 1);
        }

        /// <summary>
        /// 在dock工作区document group创建新的document Panel
        /// </summary>
        /// <param name="dockManager">dock布局管理器</param>
        /// <param name="documentGroupName">documentGroup名称</param>
        /// <param name="addDocPanelName">新增Document panel名称</param>
        /// <param name="addDocPanelCaption">新增Document Panel显示标题</param>
        /// <param name="panelContent">新增Document Panel的内容元素</param>
        /// <returns>创建的document panel对象</returns>
        public static DocumentPanel createWorkDocumentPanel(DockLayoutManager dockManager, string documentGroupName, string addDocPanelName, string addDocPanelCaption, object panelContent)
        {
            DocumentGroup documentGroup = dockManager.GetItem(documentGroupName) as DocumentGroup;
            DocumentPanel docPanel = dockManager.GetItem(addDocPanelName) as DocumentPanel;
            if (docPanel == null)
            {
                DXSplashScreen.Show<SplashScreenView>(); //显示loading框
                docPanel = dockManager.DockController.AddDocumentPanel(documentGroup);
                docPanel.Caption = addDocPanelCaption;
                docPanel.Content = panelContent;
                docPanel.Name = addDocPanelName;
            }
            else if (docPanel.IsClosed)
            {
                dockManager.DockController.Restore(docPanel);
            }
            dockManager.DockController.Activate(docPanel);

            return docPanel;
        }
        /// <summary>
        /// 在dock工作区document group创建新的floating Panel
        /// </summary>
        /// <param name="dockManager">dock布局管理器</param>
        /// <param name="floatGroupName">floatGroupName名称</param>
        /// <param name="addFloatingPanelName">新增addFloatingPanel名称</param>
        /// <param name="addFloatingPanelCaption">新增addFloatingPanel显示标题</param>
        /// <param name="panelContent">新增Floating Panel的内容元素</param>
        /// <returns>创建的document panel对象</returns>
        public static LayoutPanel createFloatingPanel(DockLayoutManager dockManager, string floatGroupName, string addFloatingPanelName, string addFloatingPanelCaption, object panelContent, Point location, Size panelSize)
        {
            FloatGroup floatGroup = dockManager.GetItem(floatGroupName) as FloatGroup;
            LayoutPanel floatPanel = dockManager.GetItem(addFloatingPanelName) as LayoutPanel;
            if (floatPanel == null)
            {
                //DXSplashScreen.Show<SplashScreenView>(); //显示loading框
                floatPanel = dockManager.DockController.AddPanel(location, panelSize);
                floatPanel.Caption = addFloatingPanelCaption;
                floatPanel.Content = panelContent;
                floatPanel.Name = addFloatingPanelName;
            }
            else if (floatPanel.IsClosed)
            {
                dockManager.DockController.Restore(floatPanel);
            }
            dockManager.DockController.Activate(floatPanel);

            return floatPanel;
        }
        /// <summary>
        /// 根据依赖组件查找主界面的输出面板下的textbox日志输出控件
        /// </summary>
        /// <param name="root">顶层组件</param>
        /// <param name="dependencyObj">依赖组件</param>
        /// <returns></returns>
        public static TextBox getLogTextBox(FrameworkElement root, DependencyObject dependencyObj)
        {
            if (root == null)
            {
                root = LayoutHelper.FindLayoutOrVisualParentObject<MainWindow>(dependencyObj as DependencyObject, true);
            }
            DockLayoutManager dockManager = LayoutHelper.FindElementByName(root, DOCKMANAGER_NAME) as DockLayoutManager;
            AutoHideGroup autoGroup = dockManager.AutoHideGroups.GetItems()[0] as AutoHideGroup;
            LayoutPanel layoutpanel = dockManager.GetItem(LAYOUTPANEL_LOG_NAME) as LayoutPanel;
            TextBox logTextBox = layoutpanel.Content as TextBox;
            return logTextBox;
        }
    }
}
