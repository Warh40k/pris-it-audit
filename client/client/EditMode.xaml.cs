using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System;


namespace client
{

    public partial class EditMode : Window
    {
        public DbAccess db;
        public DataTable table;
        List<string> columns;
        public int currentId = 0;

        public EditMode(DbAccess db, DataTable table)
        {
            InitializeComponent();

            this.db = db;
            this.table = table;
            ChangeId(currentId);
        }
        void ChangeId(int id)
        {
            wrapPanel.Children.Clear();
            DataRowCollection rows = table.Rows;

            string[] columns = db.GetColumnNames(table.TableName);
            note_label.Content = string.Format("Запись {0}", id + 1);

            wrapPanel.Children.Add(new Label() { Content = columns[0], Margin = new Thickness(5), MaxWidth = 120 });
            wrapPanel.Children.Add(new TextBox() { Text = rows[id][0].ToString(), Margin = new Thickness(5), MaxWidth = 120, IsEnabled = false });

            for (int i = 1; i < columns.Length; i++)
            {
                wrapPanel.Children.Add(new Label() { Content = columns[i], Margin = new Thickness(5), MaxWidth = 120 });
                string columnName = table.Columns[i].Caption;
                string[] columnSplit = columnName.Split('_');
                bool notJoined = true;
                if (columnSplit.Length > 1)
                {
                    int selectedIndex = 0;
                    ComboBox combo = new ComboBox() { Text = columnName, Margin = new Thickness(5), MaxWidth = 120 };

                    OleDbDataAdapter outerAdapter = new OleDbDataAdapter(string.Format("SELECT DISTINCT Id,[{0}] FROM [{1}]", columnSplit[1], columnSplit[0]), db.con);
                    DataTable outerColumnValues = new DataTable();
                    db.con.Open();
                    outerAdapter.Fill(outerColumnValues);
                    db.con.Close();
                    for (int j = 0; j < outerColumnValues.Rows.Count;j++)
                    {
                        string value = outerColumnValues.Rows[j][1].ToString();
                        if (value == rows[id][i].ToString())
                            selectedIndex = j;
                        combo.Items.Add(new ComboBoxItem() { Content = value }) ;
                    }

                    combo.SelectedIndex = selectedIndex;
                    wrapPanel.Children.Add(combo);
                    notJoined = false;
                }
                else
                    wrapPanel.Children.Add(new TextBox() { Text = rows[id][i].ToString(), Margin = new Thickness(5), MaxWidth = 120, IsEnabled = notJoined });
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string value, field;
            StringBuilder query = new StringBuilder(string.Format("UPDATE [{0}] SET ", table.TableName));
            bool isEmpty = true;
            List<OleDbParameter> parameters = new List<OleDbParameter>();
            for (int i = 1; i < table.Columns.Count; i++)
            {
                var item = wrapPanel.Children[2 * i + 1];
                string type = item.GetType().ToString();
                if (type == "System.Windows.Controls.TextBox")
                    value = ((TextBox)item).Text;
                else
                    value = (((ComboBox)item).SelectedIndex + 1).ToString();

                field = table.Columns[i].ColumnName;

                if (type == "System.Windows.Controls.ComboBox")
                {
                    parameters.Add(new OleDbParameter(field, value) { OleDbType = OleDbType.BigInt });
                    query.Append(table.TableName + "." + field + "= ?,");
                    isEmpty = false;
                }
                else if (value != table.Rows[currentId][field].ToString())
                {
                    parameters.Add(new OleDbParameter(field, value));
                    query.Append(table.TableName + "." + field + "= ?,");
                    isEmpty = false;
                }
            }
            if(isEmpty == false)
            {
                query.Remove(query.Length - 1, 1);
                query.Append(string.Format(" WHERE {0}.Id = {1}", table.TableName, table.Rows[currentId][0]));
                db.Update(table, query.ToString(), parameters);
            }
            
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
