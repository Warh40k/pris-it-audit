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
            OleDbConnection con = new OleDbConnection(conString);
            string[] restrictions = new string[4];
            restrictions[3] = "Table";

            con.Open();
            DataTable dt = con.GetSchema(branch , restrictions);
            con.Close();
            
            TreeViewItem treeItem = new TreeViewItem() { Header = branch };

            foreach (DataRow row in dt.Rows)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = row["TABLE_NAME"].ToString();
                item.MouseDoubleClick += click;
                treeItem.Items.Add(item);
            }
            tree.Items.Add(treeItem);

            return tree;
        }

    }
}
