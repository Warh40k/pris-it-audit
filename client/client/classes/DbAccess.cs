using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Text;
using System.Windows;

namespace client
{
    public class DbAccess
    {
        public string conString;
        public OleDbConnection con;
        OleDbDataAdapter oda;

        public DbAccess(string conString)
        {
            this.conString = conString;
            con = new OleDbConnection(conString);
            oda = new OleDbDataAdapter();
        }


        public DataTable SelectQuery(string query)
        {
            con.Open();
            OleDbCommand command = new OleDbCommand(query, con);

            oda.SelectCommand = command;
            DataTable dt = new DataTable();
            oda.Fill(dt);
            con.Close();

            foreach (DataColumn column in dt.Columns)
                column.ColumnName.ToString();
            
            return dt;
            
        }
        public DataTable SelectQuery(OleDbCommand command)
        {

            DataTable dt = new DataTable();
            oda.SelectCommand = command;
            oda.SelectCommand.Connection = con;

            con.Open();
            oda.Fill(dt);
            con.Close();

            return dt;
        }
        public void Update(DataTable table, string query, List<OleDbParameter> parameters)
        {
            con.Open();
            
            OleDbCommand updateCommand = new OleDbCommand(query, con);
            oda.UpdateCommand = updateCommand;
            foreach (OleDbParameter parameter in parameters)
            {
                oda.UpdateCommand.Parameters.Add(parameter);
            }
            oda.UpdateCommand.ExecuteNonQuery();
            con.Close();
        }
        public void Insert(DataTable table, List<OleDbParameter> parameters)
        {
            StringBuilder fields = new StringBuilder(string.Format("INSERT INTO [{0}] ( ", table.TableName));
            StringBuilder values = new StringBuilder("VALUES ( ");
            foreach(OleDbParameter parameter in parameters)
            {
                fields.Append(parameter.ParameterName + ",");
                values.Append("?,");
            }
            fields.Remove(fields.Length - 1, 1);
            fields.Append(") ");
            values.Remove(values.Length - 1, 1);
            values.Append(");");
            string query = fields.Append(values).ToString();
            OleDbCommand insertCommand = new OleDbCommand(query, con);
            oda.InsertCommand = insertCommand;

            foreach (OleDbParameter parameter in parameters)
                oda.InsertCommand.Parameters.Add(parameter);

            con.Open();
            oda.InsertCommand.ExecuteNonQuery();
            con.Close();
        }
        public TreeView SetTree(string branch, System.Windows.Input.MouseButtonEventHandler click)
        {
            List<string> treeItems = new List<string>();
            TreeView tree = new TreeView();

            if (branch == "Запросы")
            {
                foreach (string key in MainWindow.queries.Keys)
                    treeItems.Add(key);
            }
            else
                treeItems = GetTables();
            TreeViewItem treeItem = new TreeViewItem() { Header = branch };

            foreach (string str in treeItems)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = str;
                item.MouseDoubleClick += click;
                treeItem.Items.Add(item);
            }
            tree.Items.Add(treeItem);

            return tree;
        }
        public List<string> GetTables()
        {
            List<string> tables = new List<string>();
            string[] restrictions = new string[4];
            restrictions[3] = "Table";

            con.Open();
            DataTable dt = con.GetSchema("Tables", restrictions);
            con.Close();
            
            foreach (DataRow row in dt.Rows)
                tables.Add(row["TABLE_NAME"].ToString());
            return tables;
        }
        
        public string[,] GetColumnNames(string tableName)
        {
 
            DataTable schemaTable = new DataTable();
            con.Open();
            DataTable schema = con.GetSchema("Columns");
            con.Close();
            DataRow[] rows = schema.Select("TABLE_NAME ='" + tableName + "'");
            string[,] columnNames = new string[rows.Count(),2];
            foreach (DataRow row in rows)
            {
                columnNames[int.Parse(row["ORDINAL_Position"].ToString()) - 1, 0] = row["COLUMN_NAME"].ToString();
                columnNames[int.Parse(row["ORDINAL_Position"].ToString()) - 1, 1] = row["DATA_TYPE"].ToString();
            }
            return columnNames;
        }
        public ComboBox GetForeignItems(DataTable foreignColumnValues, int columnId, DataTable table=null, string selectedItem = "")
        {
            ComboBox combo = new ComboBox();
            if(table != null)
            {
                combo = new ComboBox() { Text = table.Columns[columnId].Caption, Margin = new Thickness(5), MaxWidth = 120 };
            }
            else
            {
                combo = new ComboBox() { Margin = new Thickness(5), MaxWidth = 120 };
            }

            for (int j = 0; j < foreignColumnValues.Rows.Count; j++)
            {
                string foreignId = foreignColumnValues.Rows[j][0].ToString();
                string foreignValue = foreignColumnValues.Rows[j][1].ToString();

                if (foreignValue == selectedItem)
                    combo.SelectedIndex = j;
                combo.Items.Add(new ComboBoxItem() { Content = foreignValue, Uid = foreignId });
            }

            return combo;

        }
        public List<string> GetForeignItems(string tableName, string columnName)
        {
            List<string> collection = new List<string>();
            OleDbDataAdapter outerAdapter = new OleDbDataAdapter(string.Format("SELECT DISTINCT Код, [{0}] FROM [{1}]", columnName, tableName), con);
            DataTable foreignColumnValues = new DataTable();
            outerAdapter.Fill(foreignColumnValues);
            for (int j = 0; j < foreignColumnValues.Rows.Count; j++)
            {
                string foreignId = foreignColumnValues.Rows[j][0].ToString();
                string foreignValue = foreignColumnValues.Rows[j][1].ToString();
                collection.Add(foreignValue);
            }
            return collection;
        }
    }
}
