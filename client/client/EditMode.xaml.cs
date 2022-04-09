using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text;
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
            note_label.Content = "Запись" + rows[id][0];

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
            StringBuilder query = new StringBuilder(string.Format("INSERT INTO @table VALUES ("));
            List<OleDbParameter> parameters = new List<OleDbParameter>() {new OleDbParameter("@table", table.TableName)};
            for (int i = 0; i < wrapPanel.Children.Count; i=i+2)
            {
                string strValue = ((TextBox)wrapPanel.Children[i + 1]).Text;
                double value;
                string field = "@" + ((Label)wrapPanel.Children[i]).Content;
                bool isInt = double.TryParse(strValue, out value);
                if (isInt == true)
                    parameters.Add(new OleDbParameter(field, value));
                else
                    parameters.Add(new OleDbParameter(field, strValue));

                query.Append(field + ",");
            }
            query.Remove(query.Length - 1, 1);
            query.Append(");");

            db.InsertQuery(query.ToString(), parameters);

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
