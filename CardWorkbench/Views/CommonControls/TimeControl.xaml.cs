﻿using System;
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

namespace CardWorkbench.Views.CommonControls
{
    /// <summary>
    /// TimeControl.xaml 的交互逻辑
    /// </summary>
    public partial class TimeControl : UserControl
    {
        public static string ModelShowValue = "00:00:00:00";
        public TimeControl()
        {
            InitializeComponent();
        }
    }
}
