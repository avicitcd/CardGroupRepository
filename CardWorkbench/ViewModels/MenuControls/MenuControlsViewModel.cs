using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CardWorkbench.ViewModels.MenuControls
{
    public abstract class MenuControlsViewModel : ViewModelBase
    {
        public ICommand stopLoadingSplashCommand
        {
            get { return new DelegateCommand<object>(onstopLoadingSplashLoaded, x => { return true; }); }
        }

        private void onstopLoadingSplashLoaded(object context)
        {
            DXSplashScreen.Close();
        }
    }
}
