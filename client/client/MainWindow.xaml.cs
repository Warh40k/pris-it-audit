using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Data;
using System.Data.OleDb;

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
            {"Сотрудник", "SELECT Сотрудник.Код, Сотрудник.Название, Должность.Название, Подразделение.Название, Сотрудник.Удалить " +
                "FROM Подразделение RIGHT JOIN (Должность RIGHT JOIN Сотрудник ON [Должность].[Код] = Сотрудник.[Должность]) ON Подразделение.[Код] = Сотрудник.[Подразделение];"},

            {"Default", "SELECT * FROM "},

            {"Инфраструктура", "SELECT Инфраструктура.Код, Оборудование.Название, Инфраструктура.ДатаИзготов, Инфраструктура.ДатаПриобр, Инфраструктура.Цена, Инфраструктура.Количество, [Кабинет].Название, Сотрудник.Название, Инфраструктура.Удалить FROM Сотрудник INNER JOIN (Оборудование INNER JOIN (Кабинет INNER JOIN Инфраструктура ON Кабинет.[Код] = Инфраструктура.[Кабинет]) ON Оборудование.[Код] = Инфраструктура.[Оборудование]) ON Сотрудник.[Код] = Инфраструктура.[Сотрудник];" },

            {"Кабинет","SELECT Кабинет.Код, Подразделение.Название, Кабинет.Название AS Название, Кабинет.Удалить FROM Кабинет LEFT JOIN Подразделение ON Подразделение.Код = Кабинет.Подразделение ORDER BY Кабинет.Код;" }
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
            GridUpdate updateGrid = UpdateGrid;
            var selected = dataGrid.SelectedIndex;
            if (selected == -1)
                selected = 0;
            EditMode em = new EditMode(db, currentTable, updateGrid, selected);
            em.ChangeItem(selected);
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

            string[,] columns = db.GetColumnNames(tableName);

            for (int i = 0; i < currentTable.Columns.Count; i++)
            {
                currentTable.Columns[i].Caption = currentTable.Columns[i].ColumnName.Replace('.', '_');
                currentTable.Columns[i].ColumnName = columns[i, 0];
            }

            dataGrid.ItemsSource = currentTable.DefaultView;
            currentTable.TableName = tableName;
            tableLabel.Content = tableName;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid(currentTable.TableName);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GridUpdate updateGrid = UpdateGrid;
            EditMode em = new EditMode(db,currentTable, updateGrid, currentTable.Rows.Count - 1);
            em.AddItem();
            em.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string query;
            if (dataGrid.SelectedIndex != -1)
            {
                query = string.Format("DELETE FROM [{0}] WHERE {1} = ?", currentTable.TableName, currentTable.Columns[0].ColumnName);
                List<OleDbParameter> parameters = new List<OleDbParameter>();
                parameters.Add(new OleDbParameter(currentTable.Columns[0].ColumnName, currentTable.Rows[dataGrid.SelectedIndex][0]));
                db.Update(currentTable, query, parameters);
                UpdateGrid(currentTable.TableName);
            }

        }
    }
}
