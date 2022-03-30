using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace client
{
    internal class DbAccess
    {
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DataBase.mdb;";
        public DataGrid dg;
        public static void SetTable(string query, DataGrid dg)
        {
            OleDbConnection con = new OleDbConnection(conString);

            con.Open();
            OleDbCommand command = new OleDbCommand(query, con);
            OleDbDataAdapter oda = new OleDbDataAdapter(command);

            DataTable dt = new DataTable();
            oda.Fill(dt);
            con.Close();
            dg.ItemsSource = dt.DefaultView;
            //dg.Columns[0].SortDirection = ListSortDirection.Ascending;
        }
        public void SetList(ListBox list)
        {
            OleDbConnection con = new OleDbConnection(conString);

            con.Open();

            string[] restrictions = new string[4];
            restrictions[3] = "Table";

            DataTable dt = con.GetSchema("Tables", restrictions);
            con.Close();

            foreach (DataRow row in dt.Rows)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = row["TABLE_NAME"].ToString();
                item.Selected += Item_Selected;
                
                list.Items.Add(item);
            }

        }

        private void Item_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)e.Source;
            string content = item.Content.ToString();
            string query = string.Format("SELECT * FROM {0}", content);
            DbAccess.SetTable(query, dg);
        }
    }

}
