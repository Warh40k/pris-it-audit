using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Controls;

namespace client
{
    internal class DbAccess
    {
        static string conString = "Provider=Microsoft.ACE.OLEDB.16.0;Data Source=DataBase.accdb;";
        public DataGrid dg;
        Dictionary<string, string> queries = new Dictionary<string, string>()
        {
            {"Worker", "SELECT CodeWorker, Worker.Name As Name, Division.Name as Division, Type " +
                "FROM Worker LEFT JOIN Division ON Division.CodeDivision = Worker.Division;"},
            {"Default", "SELECT * FROM "},
            {"Infrastructure", "SELECT " +
                "Infrastructure.InventoryNumber, EquipmentSoftware.NameES, Infrastructure.ReleaseDate, " +
                "Infrastructure.PurchaseDate, Office.OfficeName, Worker.Name, Infrastructure.Price " +
                "FROM Worker " +
                "INNER JOIN (Office INNER JOIN (EquipmentSoftware " +
                "INNER JOIN Infrastructure " +
                "ON EquipmentSoftware.[CodeES] = Infrastructure.[NameInfr]) " +
                "ON Office.[CodeOffice] = Infrastructure.[Office]) " +
                "ON Worker.[CodeWorker] = Infrastructure.[ResponsiblePerson];" },
            {"Office","SELECT CodeOffice, Division.Name AS Division, OfficeName AS Name FROM Office LEFT JOIN Division ON Division.CodeDivision = Office.Division" }
        };
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
            if(queries.ContainsKey(content))
                SetTable(queries[content], dg);
            else
                SetTable(queries["Default"]+content, dg);
        }

    }
}
