using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.OleDb;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.Collections;
using System.Data;

namespace client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DbAccess bd = new DbAccess() {dg = dataGrid};
            bd.SetList(elementList);

        }

        private void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string query = "SELECT * FROM Worker";
            DbAccess.SetTable(query, dataGrid);
        }
      
    }
}
