using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls;
using System;

namespace client
{

    public partial class EditMode : Window
    {
        public DbAccess db;
        public DataTable table;
        public int currentId = 0;
        MainWindow.GridUpdate UpdateGrid;

        public EditMode(DbAccess db, DataTable table, MainWindow.GridUpdate UpdateGrid = null, int currentId = 0)
        {
            InitializeComponent();

            this.db = db;
            this.table = table;
            this.UpdateGrid = UpdateGrid;
            this.currentId = currentId;
        }
        public void ChangeItem(int id)
        {
            DataRowCollection rows = table.Rows;
            string[,] columns = db.GetColumnNames(table.TableName);

            if (id > table.Rows.Count)
            {
                currentId--;
                return;
            }
            else if (id < 0)
            {
                currentId = 0;
                return;
            }
            else
            {
                wrapPanel.Children.Clear();
                
                note_label.Content = string.Format("Запись {0}", id + 1);

                wrapPanel.Children.Add(new Label() { Content = columns[0,0], Margin = new Thickness(5), MaxWidth = 120 });
                wrapPanel.Children.Add(new TextBox() { Text = rows[id][0].ToString(), Margin = new Thickness(5), MaxWidth = 120, IsEnabled = false });

                for (int i = 1; i < columns.GetLength(0) - 1; i++)
                {
                    wrapPanel.Children.Add(new Label() { Content = columns[i,0], Margin = new Thickness(5), MaxWidth = 120 });
                    
                    string[] columnSplit = table.Columns[i].Caption.Split('_');
                    bool notJoined = true;
                    if (columnSplit.Length > 1)
                    {
                        OleDbDataAdapter outerAdapter = new OleDbDataAdapter(string.Format("SELECT DISTINCT Код,[{0}] FROM [{1}]", columnSplit[1], columnSplit[0]), db.con);
                        DataTable foreignColumnValues = new DataTable();
                        db.con.Open();
                        outerAdapter.Fill(foreignColumnValues);
                        db.con.Close();
                        ComboBox combo = db.GetForeignItems(foreignColumnValues, i, table, rows[id][i].ToString());
                        
                        wrapPanel.Children.Add(combo);
                        notJoined = false;
                    }
                    else if (columns[i, 1] == "7")
                    {
                        DatePicker datePicker = new DatePicker();
                        datePicker.SelectedDate = (DateTime)table.Rows[id][i];
                        wrapPanel.Children.Add(datePicker);
                    }
                    else
                        wrapPanel.Children.Add(new TextBox() { Text = rows[id][i].ToString(), Margin = new Thickness(5), MaxWidth = 120, IsEnabled = notJoined });
                }
            }
        }
        public void AddItem()
        {
            DataRowCollection rows = table.Rows;
            int id = rows.Count - 1;
            string[,] columns = db.GetColumnNames(table.TableName);

            note_label.Content = string.Format("Запись {0}", id + 2);

            wrapPanel.Children.Add(new Label() { Content = columns[0,0], Margin = new Thickness(5), MaxWidth = 120 });
            wrapPanel.Children.Add(new TextBox() { Text = (id+2).ToString(), Margin = new Thickness(5), MaxWidth = 120, IsEnabled = false });

            for (int i = 1; i < columns.GetLength(0) - 1; i++)
            {
                wrapPanel.Children.Add(new Label() { Content = columns[i,0], Margin = new Thickness(5), MaxWidth = 120 });

                string[] columnSplit = table.Columns[i].Caption.Split('_');
                bool notJoined = true;
                if (columnSplit.Length > 1)
                {
                    OleDbDataAdapter outerAdapter = new OleDbDataAdapter(string.Format("SELECT DISTINCT Код,[{0}] FROM [{1}]", columnSplit[1], columnSplit[0]), db.con);
                    DataTable foreignColumnValues = new DataTable();
                    db.con.Open();
                    outerAdapter.Fill(foreignColumnValues);
                    db.con.Close();
                    ComboBox combo = db.GetForeignItems(foreignColumnValues, i, table);

                    wrapPanel.Children.Add(combo);
                    notJoined = false;
                }
                else if (columns[i,1] == "7")
                {
                    DatePicker datePicker = new DatePicker();
                    wrapPanel.Children.Add(datePicker);
                }
                else
                    wrapPanel.Children.Add(new TextBox() { Margin = new Thickness(5), MaxWidth = 120, IsEnabled = notJoined });
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentId == table.Rows.Count - 1)
                InsertItem();
            else
                UpdateItem();

           
            UpdateGrid(table.TableName);
            DialogResult = true;
        }

        private void UpdateItem()
        {
            object value;
            string field;
            StringBuilder query = new StringBuilder(string.Format("UPDATE [{0}] SET ", table.TableName));
            bool isEmpty = true;
            List<OleDbParameter> parameters = new List<OleDbParameter>();
            for (int i = 1; i < table.Columns.Count - 1; i++)
            {
                var item = wrapPanel.Children[2 * i + 1];
                string type = item.GetType().ToString();
                if (type == "System.Windows.Controls.TextBox")
                    value = ((TextBox)item).Text;
                else if (type == "System.Windows.Controls.DatePicker")
                {
                    object dpitem = ((DatePicker)item).SelectedDate;
                    value = (DateTime)dpitem;
                }
                else
                {
                    object cbitem = ((ComboBox)item).SelectedItem;
                    value = ((ComboBoxItem)cbitem).Uid.ToString();
                }

                field = table.Columns[i].ColumnName;

                if (type == "System.Windows.Controls.ComboBox" || type == "System.Windows.Controls.DatePicker")
                {
                    parameters.Add(new OleDbParameter(field, value));
                    query.Append(table.TableName + "." + field + "= ?,");
                    isEmpty = false;
                }
                else if ((string)value != table.Rows[currentId][field].ToString() && (string)value != "")
                {
                    parameters.Add(new OleDbParameter(field, value));
                    query.Append(table.TableName + "." + field + "= ?,");
                    isEmpty = false;
                }
            }
            if (isEmpty == false)
            {
                query.Remove(query.Length - 1, 1);
                query.Append(string.Format(" WHERE {0}.{1} = {2}", table.TableName, table.Columns[0].ColumnName, table.Rows[currentId][0]));
                db.Update(table, query.ToString(), parameters);
            }

        }
        private void InsertItem()
        {
            string field;
            object value;
            string[,] columns = db.GetColumnNames(table.TableName);

            bool isEmpty = true;
            List<OleDbParameter> parameters = new List<OleDbParameter>();
            for (int i = 1; i < table.Columns.Count - 1; i++)
            {
                var item = wrapPanel.Children[2 * i + 1];
                string type = item.GetType().ToString();
                if (type == "System.Windows.Controls.TextBox")
                    value = ((TextBox)item).Text;
                else if (type == "System.Windows.Controls.DatePicker")
                {
                    object dpitem = ((DatePicker)item).SelectedDate;
                    value = (DateTime)dpitem;
                }
                else
                {
                    object cbitem = ((ComboBox)item).SelectedItem;
                    value = ((ComboBoxItem)cbitem).Uid.ToString();
                }

                field = table.Columns[i].ColumnName;

                if (type == "System.Windows.Controls.ComboBox" || type == "System.Windows.Controls.DatePicker")
                {
                    OleDbParameter parameter = new OleDbParameter();
                    parameter.ParameterName = field;
                    parameter.OleDbType = (OleDbType)(int.Parse(columns[i,1]));
                    parameter.Value = value;
                    parameters.Add(parameter);
                    isEmpty = false;
                }
                else if ((string)value != "")
                {
                    parameters.Add(new OleDbParameter(field, value));
                    isEmpty = false;
                }
            }
            if (isEmpty == false)
                db.Insert(table, parameters);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void forward_button_Click(object sender, RoutedEventArgs e)
        {
            ChangeItem(++currentId);
        }

        private void backward_button_Click(object sender, RoutedEventArgs e)
        {
            ChangeItem(--currentId);
        }
    }
}
