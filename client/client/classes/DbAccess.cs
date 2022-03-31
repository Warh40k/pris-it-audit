using System.Data;
using System.Data.OleDb;
using System.Windows.Controls;

namespace client
{
    internal class DbAccess
    {
        public static string conString = "Provider=Microsoft.ACE.OLEDB.16.0;Data Source=DataBase.accdb;";
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
            string query;

            if (content == "Worker")
                query = "SELECT CodeWorker, Worker.Name As Name, Division.Name as Division, Type FROM Worker LEFT JOIN Division ON Division.CodeDivision = Worker.Division;";
            else
                query = string.Format("SELECT * FROM {0}", content);
            SetTable(query, dg);
        }

    }
}
