using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Oveger.XAMLS
{
    /// <summary>
    /// Lógica interna para ChangePathForm.xaml
    /// </summary>
    public partial class ChangePathForm : Window
    {
        public ChangePathForm()
        {
            InitializeComponent();
        }
        public string Oldpath { get; set; }
        public string Newpath { get; set; }

        public static RoutedCommand MyCommand = new RoutedCommand();
        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            MyCommand.InputGestures.Add(new KeyGesture(Key.Escape));

            BitmapImage image = new BitmapImage(new Uri("https://github.com/ChickChuck2/Oveger/blob/master/Oveger/Resources/findpath.png?raw=true", UriKind.Absolute));
            choosePath.Style = new Style()
            {
                Setters =
                {
                    new Setter()
                    {
                        Property = BackgroundProperty, Value = new ImageBrush
                        {
                            ImageSource = image
                        }
                    }
                }
            };
        }
        private void ChoosePath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.ShowDialog();

            if (dialog.SelectedPath.Length > 0)
                newPathBox.Text = System.IO.Path.Combine(dialog.SelectedPath, System.IO.Path.GetFileName(oldPathBox.Text));
        }
    }
}