using System.Windows;
using System.Windows.Controls;


namespace client
{
    /// <summary>
    /// Логика взаимодействия для EditMode.xaml
    /// </summary>
    public partial class EditMode : Window
    {
        public EditMode()
        {
            InitializeComponent();
            for(int i = 0; i < 25; i++)
            {
                wrapPanel.Children.Add(new Label() {Content = string.Format("Поле" + i), Margin = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Left });
                wrapPanel.Children.Add(new TextBox() {Text = string.Format("Значение" + i), MinWidth = 140, Margin = new Thickness(5,5,5,5), HorizontalAlignment = HorizontalAlignment.Left});
                DbAccess db = new DbAccess {conString = "Provider=Microsoft.ACE.OLEDB.16.0;Data Source=DataBase.accdb;"};
                db.MakeQuery();
            }
        }
    }
}
