using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Oveger.XAMLS
{
    /// <summary>
    /// Lógica interna para LocalRenameWindow.xaml
    /// </summary>
    public partial class LocalRenameWindow : Window
    {

        public static RoutedCommand MyCommand = new RoutedCommand();
        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        public LocalRenameWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            MyCommand.InputGestures.Add(new KeyGesture(Key.Escape));
        }
    }
}
