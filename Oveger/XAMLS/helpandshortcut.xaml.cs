using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Lógica interna para helpandshortcut.xaml
    /// </summary>
    public partial class Helpandshortcut : Window
    {
        public Helpandshortcut()
        {
            InitializeComponent();
        }

        public static RoutedCommand MyCommand = new RoutedCommand();
        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            MyCommand.InputGestures.Add(new KeyGesture(Key.Escape));

            string key = Enum.GetName(typeof(System.Windows.Forms.Keys), ConfigManager.GetKey());
            string modkey1 = Enum.GetName(typeof(MainWindow.Modifiers), ConfigManager.GetMODKey(0));
            string modkey2 = Enum.GetName(typeof(MainWindow.Modifiers), ConfigManager.GetMODKey(1));
            infokey.Text = $"Atalho atual: {modkey1} {modkey2} {key}\nTecla para minimizar o programa: ESC";

            for(int i = 0; i < Enum.GetNames(typeof(MainWindow.Modifiers)).Length; i++)
            {
                mod1box.Items.Add(Enum.GetNames(typeof(MainWindow.Modifiers))[i]);
                mod2box.Items.Add(Enum.GetNames(typeof(MainWindow.Modifiers))[i]);
            }
            for(int i = 0; i < Enum.GetNames(typeof(System.Windows.Forms.Keys)).Length; i++)
                keybox.Items.Add(Enum.GetNames(typeof(System.Windows.Forms.Keys))[i]);

            mod1box.SelectedIndex = 0;
            mod2box.SelectedIndex = 0;
            keybox.SelectedIndex = 48;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.ChangeHotkeys((System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), keybox.Text), (MainWindow.Modifiers)Enum.Parse(typeof(MainWindow.Modifiers), mod1box.Text), (MainWindow.Modifiers)Enum.Parse(typeof(MainWindow.Modifiers), mod2box.Text));
            Close();
            Thread.Sleep(300);
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
