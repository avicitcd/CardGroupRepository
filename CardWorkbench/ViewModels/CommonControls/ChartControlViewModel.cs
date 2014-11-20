using CardWorkbench.Models;
using CardWorkbench.Utils;
using DevExpress.Mvvm;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CardWorkbench.ViewModels.CommonControls
{
    /// <summary>
    ///  自定义控件曲线ViewModel类
    /// </summary>
    public class ChartControlViewModel : CommonControlViewModel
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private int number = 50;
        private int value = 1;
        private int interval = 1;
        private LineSeries2D series2D = null;

        public ICommand ChartControlDragEnterCommand
        {
            get { return new DelegateCommand<DragEventArgs>(onChartControlDragEnter, x => { return true; }); }
        }

        private void onChartControlDragEnter(DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        public ICommand ChartControlDropCommand
        {
            get { return new DelegateCommand<DragEventArgs>(onChartControlDrop, x => { return true; }); }
        }

        private void onChartControlDrop(DragEventArgs e)
        {
            //int rowHandle = (int)e.Data.GetData(typeof(int));
            //MyObject myObject = (MyObject)grid1.GetRow(rowHandle);
            //MessageBox.Show("Hello"+rowHandle);
            RowData rowData = (RowData)e.Data.GetData(typeof(RowData));
            if (rowData != null)
            {
              //FrameworkElement root = LayoutHelper.GetTopLevelVisual(e.Source as DependencyObject);

              Pane chartPane = e.Source as Pane;
              XYDiagram2D xyDiagram2D = LayoutHelper.FindParentObject<XYDiagram2D>(chartPane as DependencyObject);
              ChartControl parentChartControl = LayoutHelper.FindParentObject<ChartControl>(chartPane as DependencyObject);

              if (parentChartControl != null)
              {
                 Title lineChartTile = parentChartControl.Titles[0] as Title ;
                 Param rowParam = rowData.Row as Param;
                 string titleContent = lineChartTile.Content as string;
                 if (titleContent == null || "".Equals(titleContent))
                 {
                    lineChartTile.Content = rowParam.paramChineseName + "曲线";
                 }
              }
               
              if (series2D == null && xyDiagram2D != null)
              {
                  series2D = xyDiagram2D.Series[0] as LineSeries2D;
                  timer.Interval = TimeSpan.FromMilliseconds(50);
                  timer.Tick += new EventHandler(RefreshPlot);
                  timer.IsEnabled = true;
              }
            }
            //MessageBox.Show(((Param)rowData.Row).paramName);
        }

        private void RefreshPlot(object sender, EventArgs e)
        {
            if (value > number)
            {
                value = 1;
            }
            else
            {
                value += 2;
            }
            SeriesPoint p = new SeriesPoint(interval, value);
            if (series2D != null)
            {
                series2D.Points.Add(p);
                interval++;
                if (series2D.Points.Count > number)
                {
                    series2D.Points.RemoveAt(0);
                }
            }
        }

       

    }
}
