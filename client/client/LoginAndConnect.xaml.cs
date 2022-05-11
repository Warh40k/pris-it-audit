using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
                con_label.Content = "Соединение установлено";
                con_label.Foreground = Brushes.Green;
                login_stack.IsEnabled = true;
                OleDbDataAdapter outerAdapter = new OleDbDataAdapter(string.Format("SELECT DISTINCT Код, Название FROM Должность"), db.con);
                DataTable foreignColumnValues = new DataTable();
                outerAdapter.Fill(foreignColumnValues);
                db.con.Close();
                var items = db.GetForeignItems(foreignColumnValues, 0).Items;
                position_combo.ItemsSource = items;  
            }
            catch(System.Data.OleDb.OleDbException)
            {
                MessageBox.Show(string.Format("База данных {0} не найдена. Попробуйте задать путь самостоятельно.", dbPath), "Ошибка пути", MessageBoxButton.OK, MessageBoxImage.Error);
                con_label.Content = "Соединение не установлено";
                con_label.Foreground = Brushes.Red;
            }
            finally
            {
                db.con.Close();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
