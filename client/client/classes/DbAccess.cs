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
                "FROM Employee LEFT JOIN Department ON Department.Id = Employee.Department ORDER BY Employee.Id;"},

            {"Default", "SELECT * FROM "},

            {"Position", "SELECT * FROM [Position] ORDER BY Id" },

            {"Infrastructure", "SELECT Infrastructure.Id AS Id, Inventory.Name AS Name, Infrastructure.DateRelease AS Released, Infrastructure.DatePurchase AS Purchased, Office.Name AS Office, Employee.Name AS Responsible, Infrastructure.Price AS Price FROM Employee RIGHT JOIN (Office RIGHT JOIN (Inventory RIGHT JOIN Infrastructure ON Inventory.[Id] = Infrastructure.[Name]) ON Office.[Id] = Infrastructure.[Office]) ON Employee.Id = Infrastructure.[Responsible] ORDER BY Infrastructure.Id;"},

            {"Office","SELECT Office.Id, Department.Name AS Department, Office.Name AS Name FROM Office LEFT JOIN Department ON Department.Id = Office.Department ORDER BY Office.Id;" }
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
        public void SetList(TreeView tree)
        {
            OleDbConnection con = new OleDbConnection(conString);

            con.Open();

            string[] restrictions = new string[4];
            restrictions[3] = "Table";

            DataTable dt = con.GetSchema("Tables", restrictions);
            con.Close();

            TreeViewItem treeTable = new TreeViewItem() { Header = "Таблица" };

            foreach (DataRow row in dt.Rows)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = row["TABLE_NAME"].ToString();
                item.MouseDoubleClick += Item_MouseDoubleClick;
                treeTable.Items.Add(item);
            }
            tree.Items.Add(treeTable);
        }

        private void Item_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            string content = item.Header.ToString();
            if (queries.ContainsKey(content))
                SetTable(queries[content], dg);
            else
                SetTable(queries["Default"] + content, dg);
        }

    }
}
