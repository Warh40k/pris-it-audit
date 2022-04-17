using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Controls;

namespace client
{
    public class DbAccess
    {
        public string conString;
        public List<string> tables;
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
            foreach (DataColumn column in dt.Columns)
                column.ColumnName.ToString();
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
        public TreeView SetTree(string branch, System.Windows.Input.MouseButtonEventHandler click)
        {
            TreeView tree = new TreeView();

            tables = GetTables();
            TreeViewItem treeItem = new TreeViewItem() { Header = branch };

            foreach (string str in tables)
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
        
        public string[] GetColumnNames(string tableName)
        {
 
            DataTable schemaTable = new DataTable();
            con.Open();
            DataTable schema = con.GetSchema("Columns");
            con.Close();
            DataRow[] rows = schema.Select("TABLE_NAME ='" + tableName + "'");
            string[] columnNames = new string[rows.Count()];
            foreach (DataRow row in rows)
                columnNames[int.Parse(row["ORDINAL_POSITION"].ToString()) - 1] = row["COLUMN_NAME"].ToString();
            return columnNames;
        }
    }
}
