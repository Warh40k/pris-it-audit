﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DbAccess bd = new DbAccess() {dg = dataGrid};
            bd.SetList(treeView);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditMode em = new EditMode();
            em.Show();
        }
    }
}
