﻿using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Data;
using System.Data.OleDb;
using System;

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
            {"Местоположение", "SELECT Инфраструктура.Код, Оборудование.Название, Кабинет.Название, Подразделение.Название FROM (Подразделение INNER JOIN Кабинет ON Подразделение.[Код] = Кабинет.[Подразделение]) INNER JOIN (Оборудование INNER JOIN Инфраструктура ON Оборудование.[Код] = Инфраструктура.[Оборудование]) ON Кабинет.[Код] = Инфраструктура.[Кабинет] WHERE Оборудование.Название=\"Коммутатор Juniper QFX10002-72Q\"" },
            {"По подразделениям", "SELECT Подразделение.Название, Оборудование.Название, Sum(Инфраструктура.Количество) AS [Sum-Количество], Sum(Инфраструктура.Цена) AS [Sum-Цена] FROM Оборудование INNER JOIN ((Подразделение INNER JOIN Кабинет ON Подразделение.Код = Кабинет.Подразделение) INNER JOIN Инфраструктура ON Кабинет.Код = Инфраструктура.Кабинет) ON Оборудование.Код = Инфраструктура.Оборудование GROUP BY Подразделение.Название, Оборудование.Название HAVING (((Подразделение.Название) Like \"*\" & ? & \"*\"));"},
            {"По ответственному", "SELECT Подразделение.Название, Оборудование.Название, Sum(Инфраструктура.Количество) AS [Sum-Количество], Sum(Инфраструктура.Цена) AS [Sum-Цена] FROM Оборудование INNER JOIN ((Подразделение INNER JOIN Кабинет ON Подразделение.Код = Кабинет.Подразделение) INNER JOIN Инфраструктура ON Кабинет.Код = Инфраструктура.Кабинет) ON Оборудование.Код = Инфраструктура.Оборудование GROUP BY Подразделение.Название, Оборудование.Название HAVING (((Подразделение.Название) Like \"*\" & ? & \"*\"));" },
            {"Стоимость инфраструктуры по подразделению", "SELECT Подразделение.Название, Sum(Инфраструктура.Цена) AS Сумма FROM (Подразделение INNER JOIN Кабинет ON Подразделение.Код = Кабинет.Подразделение) INNER JOIN Инфраструктура ON Кабинет.Код = Инфраструктура.Кабинет GROUP BY Подразделение.Название ORDER BY Sum(Инфраструктура.Цена) DESC;" }
        };

        public MainWindow()
        {
            db = new DbAccess(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=data\DataBase.accdb");
            LoginAndConnect loginWindow = new LoginAndConnect();
            bool? dialogResult = loginWindow.ShowDialog();

            if (dialogResult == false)
                Application.Current.Shutdown();
            else
            {
                InitializeComponent();
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
            if (tableJoins.ContainsKey(tableName))
                currentTable = db.SelectQuery(tableJoins[tableName]);
            else if (queries.ContainsKey(tableName))
                currentTable = db.SelectQuery(queries[tableName], tableName);
            else
                currentTable = db.SelectQuery(tableJoins["Default"] + tableName + " ORDER BY Код");   

            for (int i = 0; i < currentTable.Columns.Count; i++)
            {
                currentTable.Columns[i].Caption = currentTable.Columns[i].ColumnName.Replace('.', '_');
                //currentTable.Columns[i].ColumnName = columns[i, 0];
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
    }
}
