using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace client
{
    static internal class BdAccess
    {
        public static string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DataBase.mdb;";

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
        }
        public static void SetList(ListBox list)
        {
            OleDbConnection con = new OleDbConnection(conString);

            con.Open();

            string[] restrictions = new string[4];
            restrictions[3] = "Table";

            DataTable dt = con.GetSchema("Tables", restrictions);

            foreach (DataRow row in dt.Rows)
            {
                list.Items.Add(row["TABLE_NAME"]);
            }

        }
    }
}
