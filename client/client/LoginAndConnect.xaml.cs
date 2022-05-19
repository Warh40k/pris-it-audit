using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
namespace client
{
    /// <summary>
    /// Логика взаимодействия для LoginAndConnect.xaml
    /// </summary>
    public partial class LoginAndConnect : Window
    {
        DbAccess db;
        Dictionary<string, string> credentials = new Dictionary<string, string>()
        {
            {"vladanis","Сотрудник" },
            {"galBatur", "Менеджер" },
            {"antichip", "Системный администратор" }
        };
        public string dbPath = @"C:\Users\Никита\source\repos\pris-it-audit\client\client\bin\Debug\DataBase.accdb";
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
                if (Directory.Exists("data") == false)
                    Directory.CreateDirectory("data");
                db = new DbAccess(connectionString);
                db.con.Open();
                File.Copy(path, @"data\DataBase.accdb", true);
                MessageBox.Show("Соединение успешно установлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                con_label.Content = "Соединение установлено";
                con_label.Foreground = Brushes.Green;
                login_stack.IsEnabled = true;

                //Заполнение combobox названиями должностей из таблицы Должность
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
            string selectedPosition = (string)position_combo.SelectedValue;
            //Если логин существует, а пароль равен наименованию должности сотрудника

            if (credentials.Keys.Contains(login_textbox.Text) && selectedPosition == credentials[login_textbox.Text] && passwordbox.Password == credentials[login_textbox.Text])
            {
                dbPath = path_textbox.Text;
                DialogResult = true;
            }
            else
                MessageBox.Show("Неверный логин или пароль","Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Указать путь до файл-сервера
        private void path_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.DefaultExt = "accdb";
            dialog.Filter = "Database |*.accdb";
            if (dialog.ShowDialog() == false)
                return;
            path_textbox.Text = dialog.FileName;

        }

        private void connect_button_Click(object sender, RoutedEventArgs e)
        {
            CheckConnection(path_textbox.Text);
        }
    }
}
