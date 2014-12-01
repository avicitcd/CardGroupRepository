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
    /// ReceiverUC.xaml 的交互逻辑
    /// </summary>
    public partial class ReceiverUC : UserControl
    {
        //Grid增加border边线
        public void AddGridBorder()
          {
              int rows = MainGrid.RowDefinitions.Count;
              int columns = MainGrid.ColumnDefinitions.Count;
              //TagButton btnTag = new TagButton();
              for (int i = 0; i < rows; i++)
             {
                 if (i != rows - 1)
                 {
                     #region
 
                     for (int j = 0; j < columns; j++)
                     {
                         Border border = null;
                         if (j == columns - 1)
                         {
                             border = new Border()
                             {
                                 BorderBrush = new SolidColorBrush(Colors.Gray),
                                 BorderThickness = new Thickness(1, 1, 1, 0)
                             };
                         }
                         else
                         {
                             border = new Border()
                             {
                                 BorderBrush = new SolidColorBrush(Colors.Gray),
                                 BorderThickness = new Thickness(1, 1, 0, 0)                                
                             };
                         }
                         Grid.SetRow(border, i);
                         Grid.SetColumn(border, j);
                         MainGrid.Children.Add(border);
                     }
                     #endregion
                 }
                 else
                 {
                     for (int j = 0; j < columns; j++)
                     {
                         Border border = null;
                         if (j + 1 != columns)
                         {
                             border = new Border
                             {
                                 BorderBrush = new SolidColorBrush(Colors.Gray),
                                 BorderThickness = new Thickness(1, 1, 0, 1)
                             };
                         }
                         else
                         {
                             border = new Border
                             {
                                 BorderBrush = new SolidColorBrush(Colors.Gray),
                                BorderThickness = new Thickness(1, 1, 1, 1)
                             };
                         }
                         Grid.SetRow(border, i);
                         Grid.SetColumn(border, j);
                         MainGrid.Children.Add(border);
                     }
                 }
             }
         }

        public ReceiverUC()
        {
            InitializeComponent();
            AddGridBorder();
        }
    }
}
