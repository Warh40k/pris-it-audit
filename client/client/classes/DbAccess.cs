using System;
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

        public DbAccess(string conString)
        {
            this.conString = conString;
        }

        public DataTable MakeQuery(string query)
        {
            OleDbConnection con = new OleDbConnection(conString);

            con.Open();
            OleDbCommand command = new OleDbCommand(query, con);
            OleDbDataAdapter oda = new OleDbDataAdapter(command);

            DataTable dt = new DataTable();
            oda.Fill(dt);
            con.Close();
            return dt;
            
        }
        public TreeView SetTree(string branch, System.Windows.Input.MouseButtonEventHandler click)
        {
            TreeView tree = new TreeView();

            tables = GetTables(conString);
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
        public List<string> GetTables(string conString)
        {
            List<string> tables = new List<string>();
            string[] restrictions = new string[4];
            restrictions[3] = "Table";
            OleDbConnection con = new OleDbConnection(conString);

            con.Open();
            DataTable dt = con.GetSchema("Tables", restrictions);
            con.Close();
            
            foreach (DataRow row in dt.Rows)
                tables.Add(row["TABLE_NAME"].ToString());
            return tables;
        }
        public List<string> GetColumns(DataTable table)
        {
            List<string> columns = new List<string>();
            foreach (DataColumn column in table.Columns)
                columns.Add(column.ColumnName);
            return columns;
        }
    }
}
