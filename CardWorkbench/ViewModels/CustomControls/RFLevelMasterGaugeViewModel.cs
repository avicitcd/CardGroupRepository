
using DevExpress.Mvvm;
using DevExpress.Xpf.Gauges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CardWorkbench.ViewModels.CustomControls
{
    public class RFLevelMasterGaugeViewModel : ViewModelBase
    {

        //临时引入一个信号强度计时器
        private DispatcherTimer timer = new DispatcherTimer();

        //信号强度刻度条
        LinearScale linearScale;

        //信号强度进度条命令
        public ICommand masterMcLevelProgressBarCommand
        {
            get { return new DelegateCommand<RoutedEventArgs>(onMasterMcLevelProgressBarLoaded, x => { return true; }); }
        }

        private void onMasterMcLevelProgressBarLoaded(RoutedEventArgs e)
        {
            linearScale = e.Source as LinearScale;
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += new EventHandler(masterLevelRandomChange);
            timer.Start();

        }
        private void masterLevelRandomChange(object sender, EventArgs e)
        {
            Random rnd = new Random();
            double val = rnd.Next(-110, -9); // creates a number between 1 and 12
            linearScale.Ranges[0].EndValue = new RangeValue(val);
        }

    }
}
