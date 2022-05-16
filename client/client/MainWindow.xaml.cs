using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Data;
using System.Data.OleDb;
using System;
using System.Text;
using System.IO;

namespace client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DbAccess db;
        DataTable currentTable;
        string serverFile = "";
        string clientFile = "data\\DataBase.accdb";
        public delegate void GridUpdate(string tableName);

        Dictionary<string, string> tableJoins = new Dictionary<string, string>()
        {
            {"Сотрудник", "SELECT Сотрудник.Код, Сотрудник.Название, Должность.Название, Подразделение.Название, Сотрудник.Удалить " +
                "FROM Подразделение RIGHT JOIN (Должность RIGHT JOIN Сотрудник ON [Должность].[Код] = Сотрудник.[Должность]) ON Подразделение.[Код] = Сотрудник.[Подразделение] ORDER BY Сотрудник.Код;"},

            {"Default", "SELECT * FROM "},

            {"Инфраструктура", "SELECT Инфраструктура.Код, Оборудование.Название, Инфраструктура.ДатаИзготов, Инфраструктура.ДатаПриобр, Инфраструктура.Цена, Инфраструктура.Количество, [Кабинет].Название, Сотрудник.Название, Инфраструктура.Удалить FROM Сотрудник INNER JOIN (Оборудование INNER JOIN (Кабинет INNER JOIN Инфраструктура ON Кабинет.[Код] = Инфраструктура.[Кабинет]) ON Оборудование.[Код] = Инфраструктура.[Оборудование]) ON Сотрудник.[Код] = Инфраструктура.[Сотрудник] ORDER BY Инфраструктура.[Код];" },

            {"Кабинет","SELECT Кабинет.Код, Подразделение.Название, Кабинет.Название AS Название, Кабинет.Удалить FROM Кабинет LEFT JOIN Подразделение ON Подразделение.Код = Кабинет.Подразделение ORDER BY Кабинет.Код;" }
        };

        public static Dictionary<string, string> queries = new Dictionary<string, string>()
        {
            {"Местоположение", "SELECT Инфраструктура.Код, Оборудование.Название, Кабинет.Название, Подразделение.Название FROM (Подразделение INNER JOIN Кабинет ON Подразделение.[Код] = Кабинет.[Подразделение]) INNER JOIN (Оборудование INNER JOIN Инфраструктура ON Оборудование.[Код] = Инфраструктура.[Оборудование]) ON Кабинет.[Код] = Инфраструктура.[Кабинет] WHERE Оборудование.Название=?" },
            {"По подразделениям", "SELECT Подразделение.Название, Оборудование.Название, Sum(Инфраструктура.Количество) AS [Sum-Количество], Sum(Инфраструктура.Цена) AS [Sum-Цена] FROM Оборудование INNER JOIN ((Подразделение INNER JOIN Кабинет ON Подразделение.Код = Кабинет.Подразделение) INNER JOIN Инфраструктура ON Кабинет.Код = Инфраструктура.Кабинет) ON Оборудование.Код = Инфраструктура.Оборудование GROUP BY Подразделение.Название, Оборудование.Название HAVING Подразделение.Название=?;"},
            {"По ответственному", "SELECT Сотрудник.Название, Кабинет.Код, Оборудование.Название, Инфраструктура.ДатаИзготов, Инфраструктура.ДатаПриобр, Инфраструктура.Количество, Инфраструктура.Цена FROM Сотрудник RIGHT JOIN (Оборудование INNER JOIN (Кабинет INNER JOIN Инфраструктура ON Кабинет.Код = Инфраструктура.Кабинет) ON Оборудование.Код = Инфраструктура.Оборудование) ON Сотрудник.Код = Инфраструктура.Сотрудник WHERE Сотрудник.Название=?;"},
            {"Стоимость инфраструктуры по подразделению", "SELECT Подразделение.Название, Sum(Инфраструктура.Цена) AS Сумма FROM (Подразделение INNER JOIN Кабинет ON Подразделение.Код = Кабинет.Подразделение) INNER JOIN Инфраструктура ON Кабинет.Код = Инфраструктура.Кабинет GROUP BY Подразделение.Название ORDER BY Sum(Инфраструктура.Цена) DESC;" }
        };

        public MainWindow()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            db = new DbAccess(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + clientFile);
            LoginAndConnect loginWindow = new LoginAndConnect();
            bool? dialogResult = loginWindow.ShowDialog();

            if (dialogResult == false)
                Application.Current.Shutdown();
            else
            {
                InitializeComponent();
                serverFile = loginWindow.dbPath;
                System.Windows.Input.MouseButtonEventHandler clickEvent;
                clickEvent = Item_MouseDoubleClick;
                TreeView tablesView = db.SetTree("Таблицы", clickEvent);
                TreeView queriesView = db.SetTree("Запросы", clickEvent);
                treeStack.Children.Add(tablesView);
                treeStack.Children.Add(queriesView);
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GridUpdate updateGrid = UpdateGrid;
            var selected = dataGrid.SelectedIndex;
            if (selected == -1)
                selected = 0;
            EditMode em = new EditMode(db, currentTable, updateGrid, selected);
            em.ChangeItem(selected);
            em.ShowDialog();
        }
        public void Item_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            string tableName = item.Header.ToString();
            UpdateGrid(tableName);
        }
        public void UpdateGrid(string tableName)
        {
            string[,] columns = db.GetColumnNames(tableName);
            ParameterInput paramWindow = new ParameterInput();

            if (tableJoins.ContainsKey(tableName))
            {
                currentTable = db.SelectQuery(tableJoins[tableName]);
                button_grid.IsEnabled = true;
            }
            else if (queries.ContainsKey(tableName))
            {
                button_grid.IsEnabled = false;
                OleDbCommand command = new OleDbCommand(queries[tableName]);
                switch (tableName)
                {
                    case "Местоположение":
                        paramWindow.param_name.Content = "Название оборудования";
                        paramWindow.param_combo.ItemsSource = db.GetForeignItems("Оборудование", "Название");
                        break;
                    case "По подразделениям":
                        paramWindow.param_name.Content = "Название подразделения";
                        paramWindow.param_combo.ItemsSource = db.GetForeignItems("Подразделение", "Название");
                        break;
                    case "По ответственному":
                        paramWindow.param_name.Content = "Имя ответственного";
                        paramWindow.param_combo.ItemsSource = db.GetForeignItems("Сотрудник", "Название");
                        break;
                }
                if (tableName != "Стоимость инфраструктуры по подразделению")
                { 
                    paramWindow.ShowDialog();
                    if (paramWindow.DialogResult == false)
                        return;
                    command.Parameters.AddWithValue("param", paramWindow.param_combo.Text);
                }
                currentTable = db.SelectQuery(command);
            }  
            else
            {
                currentTable = db.SelectQuery(tableJoins["Default"] + tableName + " ORDER BY Код");
                button_grid.IsEnabled = true;
            }

            for (int i = 0; i < currentTable.Columns.Count; i++)
            {
                currentTable.Columns[i].Caption = currentTable.Columns[i].ColumnName.Replace('.', '_');
                if (columns.Length == 0)
                    currentTable.Columns[i].ColumnName = currentTable.Columns[i].Caption;
                else
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
            em.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string query;
            if (dataGrid.SelectedIndex != -1)
            {
                //query = string.Format("DELETE FROM [{0}] WHERE {1} = ?", currentTable.TableName, currentTable.Columns[0].ColumnName);
                bool newState = !(bool)currentTable.Rows[dataGrid.SelectedIndex][dataGrid.Columns.Count - 1];
                query = string.Format("UPDATE [{0}] SET Удалить = ? WHERE {1} = {2}", currentTable.TableName, currentTable.Columns[0].ColumnName, currentTable.Rows[dataGrid.SelectedIndex][0]);
                List<OleDbParameter> parameters = new List<OleDbParameter>();
                parameters.Add(new OleDbParameter(currentTable.Columns[dataGrid.Columns.Count - 1].ColumnName, Convert.ToInt32(newState)));
                db.Update(currentTable, query, parameters);
                UpdateGrid(currentTable.TableName);
            }

        }

        private async void export_button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder csv = new StringBuilder();
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = "csv";
            string exportFile = "";

            if (saveDialog.ShowDialog() == false)
                return;
            
            exportFile = saveDialog.FileName;
            //Заполнение строки данными таблицы в формате csv
            foreach (DataColumn column in currentTable.Columns)
                csv.Append(column.ColumnName + ";");
            csv.Remove(csv.Length - 1, 1);
            csv.Append('\n');

            foreach(DataRow row in currentTable.Rows)
            {
                foreach(var item in row.ItemArray)
                {
                    csv.Append(item + ";");
                }
                csv.Remove(csv.Length - 1, 1);
                csv.Append("\n");
            }
            csv.Remove(csv.Length - 1, 1);
            try
            {
                using (FileStream writer = new FileStream(exportFile, FileMode.Create))
                {
                    byte[] buffer = Encoding.GetEncoding(1251).GetBytes(csv.ToString());
                    await writer.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            catch(IOException)
            {
                MessageBox.Show("Экспорт не удался. Проверьте подключение и попробуйте ещё раз", "Беда", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            MessageBox.Show("Файл записан", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            System.Diagnostics.Process.Start(exportFile);
        }
        private void sync_button_Click(object sender, RoutedEventArgs e)
        {
            db.Sync(serverFile, clientFile);
            if (currentTable != null)
                UpdateGrid(currentTable.TableName);
        }
    }
}
