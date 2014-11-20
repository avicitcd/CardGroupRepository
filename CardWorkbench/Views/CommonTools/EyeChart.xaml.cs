using DevExpress.Xpf.Charts;
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
using System.Windows.Threading;

namespace CardWorkbench.Views.CommonTools
{
    /// <summary>
    /// EyeChart.xaml 的交互逻辑
    /// </summary>
    public partial class EyeChart : UserControl
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private int number = 50;
        private int value = 1;
        private int interval = 1;
        public EyeChart()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(RefreshPlot);
            timer.IsEnabled = true;
        }

        private void RefreshPlot(object sender,EventArgs e)
        {
            if(value>number)
            {
                value = 1;
            }else
            {
                value+=2;
            }
            SeriesPoint p = new SeriesPoint(interval, value);
            series.Points.Add(p);
            interval++;
            if(series.Points.Count>number)
            {
                series.Points.RemoveAt(0);
            }
        }
    }
}
