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
        public int currentId;

        public EditMode(DbAccess db, DataTable table, int currentId = 0)
        {
            InitializeComponent();

            this.db = db;
            this.table = table;
            this.currentId = currentId;
            ChangeId(currentId);

        }
        void ChangeId(int id)
        {
            wrapPanel.Children.Clear();
            DataRowCollection rows = table.Rows;

            columns = db.GetColumns(table);
            note_label.Content = rows[id][0];

            wrapPanel.Children.Add(new Label() { Content = columns[0], Margin = new Thickness(5), MaxWidth = 120 });
            wrapPanel.Children.Add(new TextBox() { Text = rows[id][0].ToString(), Margin = new Thickness(5), MaxWidth = 120, IsReadOnly = true });

            for (int i = 1; i < columns.Count; i++)
            {
                wrapPanel.Children.Add(new Label() { Content = columns[i], Margin = new Thickness(5), MaxWidth = 120 });
                wrapPanel.Children.Add(new TextBox() { Text = rows[id][i++].ToString(), Margin = new Thickness(5), MaxWidth = 120 });
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

        private void forward_button_Click(object sender, RoutedEventArgs e)
        {
            ChangeId(++currentId);
        }

        private void backward_button_Click(object sender, RoutedEventArgs e)
        {
            ChangeId(--currentId);
        }
    }
}
