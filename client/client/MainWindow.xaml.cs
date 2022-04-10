using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Data;

namespace client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbAccess db;
        DataTable currentTable;
        public delegate void MouseDClick(object sender, System.Windows.Input.MouseButtonEventArgs e);
        Dictionary<string, string> queries = new Dictionary<string, string>()
        {
            {"Employee", "SELECT Employee.Id As Id, Employee.Name As Name, Department.Name as Department, Type " +
                "FROM Employee LEFT JOIN Department ON Department.Id = Employee.Department ORDER BY Employee.Id;"},

            {"Default", "SELECT * FROM "},

            {"Position", "SELECT * FROM [Position] ORDER BY Id"},

            {"Infrastructure", "SELECT Infrastructure.Id AS Id, Inventory.Name AS Name, Infrastructure.DateRelease AS Released, Infrastructure.DatePurchase AS Purchased, Office.Name AS Office, Employee.Name AS Responsible, Infrastructure.Price AS Price FROM Employee RIGHT JOIN (Office RIGHT JOIN (Inventory RIGHT JOIN Infrastructure ON Inventory.[Id] = Infrastructure.[Name]) ON Office.[Id] = Infrastructure.[Office]) ON Employee.Id = Infrastructure.[Responsible] ORDER BY Infrastructure.Id;"},

            {"Office","SELECT Office.Id, Department.Name AS Department, Office.Name AS Name FROM Office LEFT JOIN Department ON Department.Id = Office.Department ORDER BY Office.Id;" }
        };
        public MainWindow()
        {
            InitializeComponent();
            db = new DbAccess("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=DataBase.accdb;");

            System.Windows.Input.MouseButtonEventHandler clickEvent;
            clickEvent = Item_MouseDoubleClick;
            TreeView treeView = db.SetTree("Tables", clickEvent);
            treeStack.Children.Add(treeView);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditMode em = new EditMode(db, currentTable);
            em.Show();
        }
        public void Item_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            string content = item.Header.ToString();
            if (queries.ContainsKey(content))
            {
                currentTable = db.SelectQuery(queries[content]);
                dataGrid.ItemsSource = currentTable.DefaultView;
            }
            else
            {
                currentTable = db.SelectQuery(queries["Default"] + content);
                dataGrid.ItemsSource = currentTable.DefaultView;
            }
            currentTable.TableName = content;
            tableLabel.Content = content;
        }
    }
}
