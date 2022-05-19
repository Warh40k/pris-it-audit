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
    /// Главное окно приложения. Просмотр таблиц и запросов
    /// </summary>
    public partial class MainWindow : Window
    {
        DbAccess db;

        DataTable currentTable; // Таблица, отображаемая в текущий момент
        string serverFile = ""; // Путь к файлу сервера
        string clientFile = "data\\DataBase.accdb"; // Расположение локальной копии базы
        public delegate void GridUpdate(string tableName);

        // SQL-запросы таблиц
        Dictionary<string, string> tableJoins = new Dictionary<string, string>()
        {
            {"Сотрудник", "SELECT Сотрудник.Код, Сотрудник.Название, Должность.Название, Подразделение.Название, Сотрудник.Удалить " +
                "FROM Подразделение RIGHT JOIN (Должность RIGHT JOIN Сотрудник ON [Должность].[Код] = Сотрудник.[Должность]) ON Подразделение.[Код] = Сотрудник.[Подразделение] ORDER BY Сотрудник.Код;"},

            {"Default", "SELECT * FROM "},

            {"Инфраструктура", "SELECT Инфраструктура.Код, Оборудование.Название, Инфраструктура.ДатаИзготов, Инфраструктура.ДатаПриобр, Инфраструктура.Цена, Инфраструктура.Количество, [Кабинет].Название, Сотрудник.Название, Инфраструктура.Удалить FROM Сотрудник INNER JOIN (Оборудование INNER JOIN (Кабинет INNER JOIN Инфраструктура ON Кабинет.[Код] = Инфраструктура.[Кабинет]) ON Оборудование.[Код] = Инфраструктура.[Оборудование]) ON Сотрудник.[Код] = Инфраструктура.[Сотрудник] ORDER BY Инфраструктура.[Код];" },

            {"Кабинет","SELECT Кабинет.Код, Подразделение.Название, Кабинет.Название AS Название, Кабинет.Удалить FROM Кабинет LEFT JOIN Подразделение ON Подразделение.Код = Кабинет.Подразделение ORDER BY Кабинет.Код;" }
        };

        // SQL для запросов
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
            bool? dialogResult = loginWindow.ShowDialog(); // Открывает окно авторизации

            if (dialogResult == false)
                Application.Current.Shutdown();
            else
            {
                InitializeComponent();
                serverFile = loginWindow.dbPath;
                System.Windows.Input.MouseButtonEventHandler clickEvent;
                clickEvent = Item_MouseDoubleClick;
                TreeView tablesView = db.SetTree("Таблицы", clickEvent); // Получение дерева таблиц и запросов
                TreeView queriesView = db.SetTree("Запросы", clickEvent);
                treeStack.Children.Add(tablesView);
                treeStack.Children.Add(queriesView);
            }
            
        }

        private void change_button_Click(object sender, RoutedEventArgs e)
        {
            GridUpdate updateGrid = UpdateGrid;
            var selected = dataGrid.SelectedIndex; // Номер выделенной строки в таблице
            if (selected == -1) // Если никакой элемент не выбран
                selected = 0;

            if (File.Exists(serverFile + "\\..\\locked_db") || File.Exists(serverFile + "\\..\\DataBase.laccdb")) // Проверка блокировки базы данных
            {
                Wait waitWindow = new Wait(serverFile + "\\..\\locked_db");
                waitWindow.ShowDialog(); // Вызов окна ожидания
                if (waitWindow.DialogResult == false)
                    return;
            }
            db.Sync(serverFile, clientFile);

            EditMode em = new EditMode(db, currentTable, updateGrid, selected);
            File.Create(serverFile + "\\..\\locked_db").Close(); // Заблокировать базу для других пользователей

            em.ChangeItem(selected); // Открыть окно изменение выбранного элемента
            em.ShowDialog();
            if (em.DialogResult == true)
                db.Sync(clientFile, serverFile);
            File.Delete(serverFile + "\\..\\locked_db"); // Разблокировка
        }

        // Двойное нажатие по элементу дерева таблиц
        public void Item_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender; 
            string tableName = item.Header.ToString();
            UpdateGrid(tableName);
        }
        
        // Функция обновления DataGridView
        public void UpdateGrid(string tableName)
        {
            string[,] columns = db.GetColumnNames(tableName);
            ParameterInput paramWindow = new ParameterInput(); // Окно ввода параметра запроса

            if (tableJoins.ContainsKey(tableName)) // Если это таблица и для неё явно указан SQL
            {
                currentTable = db.SelectQuery(tableJoins[tableName]);
                button_grid.IsEnabled = true;
            }
            else if (queries.ContainsKey(tableName)) // Если это запрос и есть SQL для него
            {
                button_grid.IsEnabled = false;
                OleDbCommand command = new OleDbCommand(queries[tableName]);
                switch (tableName)
                {
                    case "Местоположение":
                        paramWindow.param_name.Content = "Название оборудования"; // Заголовок параметра
                        paramWindow.param_combo.ItemsSource = db.GetForeignItems("Оборудование", "Название"); // Данные для подстановки
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
                if (tableName != "Стоимость инфраструктуры по подразделению") // Для этого запроса не нужны параметры
                { 
                    paramWindow.ShowDialog(); // Показываем окно ввода параметра
                    if (paramWindow.DialogResult == false)
                        return;
                    command.Parameters.AddWithValue("param", paramWindow.param_combo.Text); // Добавление параметра с выбранным значением
                }
                currentTable = db.SelectQuery(command);
            }  
            else // Если явно не указан SQL
            {
                currentTable = db.SelectQuery(tableJoins["Default"] + tableName + " ORDER BY Код"); // В остальных случаях выводит всё, что есть
                button_grid.IsEnabled = true;
            }

            for (int i = 0; i < currentTable.Columns.Count; i++) // Переименовывание названия столбцов (нужно для правильного отображения данных и возможности обновления)
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

        private void add_button_Click(object sender, RoutedEventArgs e)
        {
            GridUpdate updateGrid = UpdateGrid;
            if (File.Exists(serverFile + "\\..\\locked_db") || File.Exists(serverFile + "\\..\\DataBase.laccdb"))
            {
                Wait waitWindow = new Wait(serverFile + "\\..\\locked_db");
                waitWindow.ShowDialog();
                if (waitWindow.DialogResult == false)
                    return;
            }

            db.Sync(serverFile, clientFile);

            File.Create(serverFile + "\\..\\locked_db").Close();

            EditMode em = new EditMode(db,currentTable, updateGrid, currentTable.Rows.Count - 1);
            em.AddItem();
            em.ShowDialog();

            if (em.DialogResult == true)
                db.Sync(clientFile, serverFile);
            File.Delete(serverFile + "\\..\\locked_db");
        }

        // Отметка для удаления
        private void delete_button_Click(object sender, RoutedEventArgs e)
        {
            string query;
            if (dataGrid.SelectedIndex != -1)
            {
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
            //строка с содержимым таблицы
            StringBuilder csv = new StringBuilder();

            // выбор пути сохранения
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = "csv";
            string exportFile = "";

            if (saveDialog.ShowDialog() == false)
                return;
            
            exportFile = saveDialog.FileName;

            //Шапка таблицы
            foreach (DataColumn column in currentTable.Columns)
                csv.Append(column.ColumnName + ";");
            csv.Remove(csv.Length - 1, 1);
            csv.Append('\n');

            //Заполнение строки данными таблицы в формате csv (столбцы разделять символом ";", а ряды - "\n")
            foreach (DataRow row in currentTable.Rows)
            {
                foreach(var item in row.ItemArray)
                {
                    csv.Append(item + ";");
                }
                csv.Remove(csv.Length - 1, 1);
                csv.Append("\n");
            }
            csv.Remove(csv.Length - 1, 1);

            //Запись в файл
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
            
            // Просмотр в программе по умолчанию (Excel)
            System.Diagnostics.Process.Start(exportFile)    ;
        }
        private void sync_button_Click(object sender, RoutedEventArgs e)
        {
            db.Sync(serverFile, clientFile);
            if (currentTable != null)
                UpdateGrid(currentTable.TableName);
        }
    }
}
