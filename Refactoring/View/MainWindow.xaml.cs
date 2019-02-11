using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Refactoring
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class ListItem
        {
            ImageSource Image { get; }
            public string Name { get;}
            public string Type { get; }
            public string ChangeData { get; }
            public uint Size { get; }

            public ListItem(string Name, string ChangeData)
            {
                this.Name = Name;
                Type = "TestFile";
                this.ChangeData = ChangeData;
                Size = 53225;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            ObservableCollection<ListItem> Hallo = new ObservableCollection<ListItem>();
            Hallo.Add(new ListItem("Bubble", "GUM"));
            Hallo.Add(new ListItem("Why", "A notwork"));
            Explorer.ItemsSource = Hallo;
        }
    }
}
