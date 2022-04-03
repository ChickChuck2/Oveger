using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
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
        public string oldpath { get; set; }
        public string newpath { get; set; }

        private void Window_Initialized(object sender, EventArgs e)
        {
            BitmapImage image = new BitmapImage(new Uri("", UriKind.Relative));
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
        private void choosePath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.ShowDialog();

            if (dialog.SelectedPath.Length > 0)
                newPathBox.Text = System.IO.Path.Combine(dialog.SelectedPath, System.IO.Path.GetFileName(oldPathBox.Text));
        }
    }
}