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
            for(int i = 0; i < 25; i++)
                wrapPanel.Children.Add(new Button() { Content = string.Format("Проверка" + i), Margin = new Thickness(5)});
        }
    }
}
