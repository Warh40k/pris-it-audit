using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;


namespace client
{
    /// <summary>
    /// Логика взаимодействия для EditMode.xaml
    /// </summary>
    public partial class EditMode : Window
    {
        public DbAccess db;
        public DataTable table;
        List<string> columns;

        public EditMode(DbAccess db, DataTable table)
        {
            InitializeComponent();
            this.db = db;
            this.table = table;
            columns = db.GetColumns(table);
            DataRowCollection row = table.Rows;
            foreach(string column in columns)
            {
                wrapPanel.Children.Add(new Label() { Content = column, Margin = new Thickness(5), MaxWidth = 120});
                wrapPanel.Children.Add(new TextBox() {Margin = new Thickness(5), MaxWidth = 120});
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
