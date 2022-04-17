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
        public delegate void GridUpdate(string tableName);

        Dictionary<string, string> queries = new Dictionary<string, string>()
        {
            {"Employee", "SELECT Employee.Id, Employee.Name AS Name, Employee.Surname, [Position].Name, Department.Name " +
                "FROM Department RIGHT JOIN ([Position] RIGHT JOIN Employee ON [Position].[Id] = Employee.[Position]) ON Department.[Id] = Employee.[Department];"},

            {"Default", "SELECT * FROM "},

            {"Position", "SELECT * FROM [Position] ORDER BY Id"},

            {"Infrastructure", "SELECT Infrastructure.Id, Inventory.Name, Infrastructure.Released, Infrastructure.Purchased,  Infrastructure.Price, Infrastructure.Units, Office.Name, Employee.Name FROM Employee RIGHT JOIN (Office RIGHT JOIN (Inventory RIGHT JOIN Infrastructure ON Inventory.[Id] = Infrastructure.[Name]) ON Office.[Id] = Infrastructure.[Office]) ON Employee.Id = Infrastructure.[Responsible] ORDER BY Infrastructure.Id;"},

            {"Office","SELECT Office.Id, Department.Name, Office.Name AS Name FROM Office LEFT JOIN Department ON Department.Id = Office.Department ORDER BY Office.Id;" }
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
            string tableName = item.Header.ToString();
            UpdateGrid(tableName);
        }
        public void UpdateGrid(string tableName)
        {
            if (queries.ContainsKey(tableName))
                currentTable = db.SelectQuery(queries[tableName]);
            else
                currentTable = db.SelectQuery(queries["Default"] + tableName);

            string[] columns = db.GetColumnNames(tableName);
            for (int i = 0; i < currentTable.Columns.Count; i++)
            {
                currentTable.Columns[i].Caption = currentTable.Columns[i].ColumnName.Replace('.', '_');
                currentTable.Columns[i].ColumnName = columns[i];
            }

            dataGrid.ItemsSource = currentTable.DefaultView;
            currentTable.TableName = tableName;
            tableLabel.Content = tableName;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid(currentTable.TableName);
        }
    }
}
