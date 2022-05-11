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
            path_textbox.Text = dbPath;
        }

        private void CheckConnection(string path)
        {
            string connectionString = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};", path);
            try
            {
                db = new DbAccess(connectionString);
                db.con.Open();
                MessageBox.Show("Соединение успешно установлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                con_label.Content = "Соединение установлено";
                con_label.Foreground = Brushes.Green;
                login_stack.IsEnabled = true;
                OleDbDataAdapter outerAdapter = new OleDbDataAdapter(string.Format("SELECT DISTINCT Код, Название FROM Должность"), db.con);
                DataTable foreignColumnValues = new DataTable();
                outerAdapter.Fill(foreignColumnValues);
                db.con.Close();
                ItemCollection items = db.GetForeignItems(foreignColumnValues, 0).Items;
                foreach (ComboBoxItem item in items)
                    position_combo.Items.Add(item.Content);
            }
            catch (OleDbException)
            {
                MessageBox.Show(string.Format("База данных {0} не найдена. Попробуйте задать путь самостоятельно.", path), "Ошибка пути", MessageBoxButton.OK, MessageBoxImage.Error);
                con_label.Content = "Соединение не установлено";
                login_stack.IsEnabled = false;
                con_label.Foreground = Brushes.Red;
            }
            finally
            {
                db.con.Close();
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void path_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.DefaultExt = "accdb";
            dialog.Filter = "Database |*.accdb";
            dialog.ShowDialog();
            path_textbox.Text = dialog.FileName;

        }

        private void connect_button_Click(object sender, RoutedEventArgs e)
        {
            CheckConnection(path_textbox.Text);
        }
    }
}
