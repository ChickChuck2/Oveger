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
using System.Windows.Threading;

namespace Oveger.XAMLS
{
    /// <summary>
    /// Lógica interna para DownloadProgress.xaml
    /// </summary>
    public partial class DownloadProgress : Window
    {
        public DownloadProgress()
        {
            InitializeComponent();
            this.Topmost = true;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }
        public void SetText(string WhatisLove)
        {
            TitleBlock.Text = $"Baixando {WhatisLove}";
        }
        private delegate void UpdateTextBox();
        public void setSeconds(int seconds)
        {
            UpdateTextBox updateDownbox = () => TitleBlock.Text = "Download Concluido!";
            TitleBlock.Dispatcher.Invoke(updateDownbox);
            for (int i = seconds; i <= seconds; i--)
            {
                UpdateTextBox updateTextBox = () => textcount.Text = $"Fechando em ({i})";
                textcount.Dispatcher.Invoke(updateTextBox);
                DoEvents();
                Thread.Sleep(1000);
                if (i <= 0)
                    break;
            }
            Close();
        }
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke
            (
            DispatcherPriority.Background,
            (SendOrPostCallback)delegate (object arg)
            {
                var f = arg as DispatcherFrame;
                f.Continue = false;
            },
            frame
            );
            Dispatcher.PushFrame(frame);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
