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

namespace client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DataBase.mdb;";
        public MainWindow()
        {
            InitializeComponent();
            

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OleDbConnection dbConnection = new OleDbConnection(connectionString);

            dbConnection.Open();
            string query = "SELECT * FROM Worker";
            OleDbCommand command = new OleDbCommand(query, dbConnection);
            OleDbDataReader reader = command.ExecuteReader();

            List<BdAccess> workers = new List<BdAccess>();
            while (reader.Read())
            {
                workers.Add(new BdAccess()
                {
                    Id = int.Parse(reader[0].ToString()),
                    Name = reader[1].ToString(),
                    Surname = reader[2].ToString(),
                    Position = reader[3].ToString(),
                    Department = reader[4].ToString()
                });
            }

            dataGrid.ItemsSource = workers;
        }
    }
}
