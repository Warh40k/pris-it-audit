using System.Windows;
using System.Threading;
using System.IO;
namespace client
{
    /// <summary>
    /// Логика взаимодействия для Wait.xaml
    /// </summary>
    public partial class Wait : Window
    {
        string lockFile;
        Thread childThread;
        Thread mainThread;

        public Wait(string lockFile)
        {
            InitializeComponent();
            this.lockFile = lockFile;
            
            childThread = new Thread(CheckLock);
            childThread.Name = "Child";
            childThread.IsBackground = true;
            
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            childThread.Start();
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
           
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            
        }
        void CheckLock()
        {
            while (true)
            {
                if (File.Exists(lockFile) == false && File.Exists(lockFile + "\\..\\DataBase.laccdb") == false)
                {
                    Dispatcher.Invoke(() => DialogResult = true);
                }
                Thread.Sleep(500);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            childThread.Abort();
            DialogResult = false;
        }
    }
}
