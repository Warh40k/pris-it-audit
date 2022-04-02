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
using System.Windows.Shapes;

namespace client
{
    /// <summary>
    /// Логика взаимодействия для EditMode.xaml
    /// </summary>
    public partial class EditMode : Window
    {
        public EditMode()
        {
            InitializeComponent();
            for(int i = 0; i < 10; i++)
            {
                wrapPanel.Children.Add(new Label() { Content = string.Format("Поле" + i), Margin = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Center });
                wrapPanel.Children.Add(new Button() { Content = string.Format("Кнопка1" + i), Margin = new Thickness(5) });
            }
        }
    }
}
