﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Controls;

namespace client
{
    public class DbAccess
    {
        public string conString;
        public List<string> tables;
        OleDbConnection con;
        OleDbDataAdapter oda;
        public DbAccess(string conString)
        {
            this.conString = conString;
            con = new OleDbConnection(conString);
            oda = new OleDbDataAdapter();
        }

        public DataTable SelectQuery(string query)
        {
            con.Open();
            OleDbCommand command = new OleDbCommand(query, con);
            oda.SelectCommand = command;

            DataTable dt = new DataTable(); 
            oda.Fill(dt);
            con.Close();
            return dt;
            
        }
        public void InsertQuery(string query, List<OleDbParameter> parameters)
        {
            con.Open();

            OleDbCommand command = new OleDbCommand(query, con);
            //OleDbCommand command = new OleDbCommand("UPDATE [Employee] SET Name = @Name WHERE Employee.Id = 1", con);
            //OleDbCommand command = new OleDbCommand(string.Format("UPDATE {0} SET Name = @Name WHERE {), con);

            foreach(OleDbParameter parameter in parameters)
                command.Parameters.Add(parameter);

            //command.Parameters.Add(new OleDbParameter("@Id", 1));
            //command.Parameters.Add(new OleDbParameter("@table", "Employee"));
            //command.Parameters.Add(new OleDbParameter("@Name", "Дима"));
            command.ExecuteNonQuery();
            con.Close();
        }
        public void Update(DataTable table, string query, List<OleDbParameter> parameters)
        {
            con.Open();

            //OleDbCommand selectCommand = new OleDbCommand("SELECT Infrastructure.Id AS Id, Inventory.Name AS Name, Infrastructure.DateRelease AS Released, Infrastructure.DatePurchase AS Purchased, Office.Name AS Office, Employee.Name AS Responsible, Infrastructure.Price AS Price FROM Employee RIGHT JOIN (Office RIGHT JOIN (Inventory RIGHT JOIN Infrastructure ON Inventory.[Id] = Infrastructure.[Name]) ON Office.[Id] = Infrastructure.[Office]) ON Employee.Id = Infrastructure.[Responsible] ORDER BY Infrastructure.Id;", con);
            OleDbCommand updateCommand = new OleDbCommand(query, con);
            //OleDbCommand updateCommand = new OleDbCommand("UPDATE [Infrastructure] SET Price = 200 WHERE Infrastructure.Id = 1", con);
            oda.UpdateCommand = updateCommand;
            foreach (OleDbParameter parameter in parameters)
            {
                oda.UpdateCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
            }
            // oda.AcceptChangesDuringUpdate = true;
            //oda.UpdateCommand.ExecuteNonQuery();
            //table.Rows[0]["Price"] = 5000;
            oda.Update(table);
            con.Close();
        }
        public TreeView SetTree(string branch, System.Windows.Input.MouseButtonEventHandler click)
        {
            TreeView tree = new TreeView();

            tables = GetTables(conString);
            TreeViewItem treeItem = new TreeViewItem() { Header = branch };

            foreach (string str in tables)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = str;
                item.MouseDoubleClick += click;
                treeItem.Items.Add(item);
            }
            tree.Items.Add(treeItem);

            return tree;
        }
        public List<string> GetTables(string conString)
        {
            List<string> tables = new List<string>();
            string[] restrictions = new string[4];
            restrictions[3] = "Table";

            con.Open();
            DataTable dt = con.GetSchema("Tables", restrictions);
            con.Close();
            
            foreach (DataRow row in dt.Rows)
                tables.Add(row["TABLE_NAME"].ToString());
            return tables;
        }
        public List<string> GetColumns(DataTable table)
        {
            List<string> columns = new List<string>();
            foreach (DataColumn column in table.Columns)
                columns.Add(column.ColumnName);
            return columns;
        }
    }
}
