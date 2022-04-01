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
            {"Employee", "SELECT Employee.Id As Id, Employee.Name As Name, Department.Name as Department, Type " +
                "FROM Employee LEFT JOIN Department ON Department.Id = Employee.Department;"},

            {"Default", "SELECT * FROM "},

            {"Position", "SELECT * FROM [Position]" },

            {"Infrastructure", "SELECT Infrastructure.Id AS Id, Inventory.Name AS Name, Infrastructure.DateRelease AS Released, Infrastructure.DatePurchase AS Purchased, Office.Name AS Office, Employee.Name AS Responsible, Infrastructure.Price AS Price FROM Employee RIGHT JOIN (Office RIGHT JOIN (Inventory RIGHT JOIN Infrastructure ON Inventory.[Id] = Infrastructure.[Name]) ON Office.[Id] = Infrastructure.[Office]) ON Employee.Id = Infrastructure.[Responsible];"},

            {"Office","SELECT Office.Id, Department.Name AS Department, Office.Name AS Name FROM Office LEFT JOIN Department ON Department.Id = Office.Department" }
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
