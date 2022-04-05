using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Controls;

namespace client
{
    internal class DbAccess
    {
        public string conString;
        
        public DataView MakeQuery(string query)
        {
            OleDbConnection con = new OleDbConnection(conString);

            con.Open();
            OleDbCommand command = new OleDbCommand(query, con);
            OleDbDataAdapter oda = new OleDbDataAdapter(command);

            DataTable dt = new DataTable();
            oda.Fill(dt);
            con.Close();
            DataView dview = new DataView(dt);
            return dview;
            
        }
        public TreeView SetTree(string branch, System.Windows.Input.MouseButtonEventHandler click)
        {
            TreeView tree = new TreeView();

            List<string> columns = GetColumns(conString);
            TreeViewItem treeItem = new TreeViewItem() { Header = branch };

            foreach (string str in columns)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = str;
                item.MouseDoubleClick += click;
                treeItem.Items.Add(item);
            }
            tree.Items.Add(treeItem);

            return tree;
        }
        public List<string> GetColumns(string conString)
        {
            List<string> columns = new List<string>();
            string[] restrictions = new string[4];
            restrictions[3] = "Table";
            OleDbConnection con = new OleDbConnection(conString);

            con.Open();
            DataTable dt = con.GetSchema("Tables", restrictions);
            con.Close();
            
            foreach (DataRow row in dt.Rows)
                columns.Add(row["TABLE_NAME"].ToString());
            return columns;
        }
    }
}
