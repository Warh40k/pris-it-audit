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
    /// Логика взаимодействия для LoginAndConnect.xaml
    /// </summary>
    public partial class LoginAndConnect : Window
    {
        string dbPath = "X:\\DataBase.accdb";
        DbAccess db;
        public LoginAndConnect()
        {
            InitializeComponent();
            string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};", dbPath);
            try
            {
                db = new DbAccess(connectionString);
                db.con.Open();
            }
            catch(System.Data.OleDb.OleDbException)
            {
                MessageBox.Show(string.Format("База данных {0} не найдена. Попробуйте задать путь самостоятельно.", dbPath), "Ошибка пути", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
            finally
            {
                db.con.Close();
            }
            con_label.Content = "Соединение установлено";
            con_label.Foreground = Brushes.Green;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
