using Oveger.XAMLS;
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

namespace Oveger.Scripts
{
    /// <summary>
    /// Lógica interna para groupsWindow.xaml
    /// </summary>
    public partial class groupsWindow : Window
    {
        public bool removeMode = false;
        public string pathToAdd = "";
        public MainWindow mainWindow;
        public static RoutedCommand MyCommand = new RoutedCommand();
        public groupsWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow.Hide();
            Topmost = true;
            Focus();
            StartLabels();
            MyCommand.InputGestures.Add(new KeyGesture(Key.Escape));
        }

        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e) => Close();
        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            var groups = groupsBlock.Text.Split(';');
            ConfigManager.AddGroups(groups);
            groupsMngr.Children.Clear();
            StartLabels();
            groupsBlock.Text = string.Empty;
        }

        void StartLabels()
        {
            var groups = ConfigManager.GetGroups();
            foreach (var group in groups)
            {
                // Pill-chip button for each group
                Button chip = new Button()
                {
                    Margin  = new Thickness(4),
                    Cursor  = Cursors.Hand,
                    Tag     = group
                };

                Border chipBorder = new Border()
                {
                    CornerRadius    = new CornerRadius(20),
                    Padding         = new Thickness(14, 6, 14, 6),
                    BorderThickness = new Thickness(1)
                };
                chipBorder.Background  = new SolidColorBrush(Color.FromArgb(180, 26, 23, 48));
                chipBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(60, 124, 58, 237));

                StackPanel chipContent = new StackPanel { Orientation = Orientation.Horizontal };
                TextBlock  chipText   = new TextBlock()
                {
                    Text       = group,
                    Foreground = new SolidColorBrush(Color.FromRgb(167, 139, 250)),
                    FontSize   = 13,
                    FontWeight = FontWeights.Medium,
                    VerticalAlignment = VerticalAlignment.Center
                };
                TextBlock chipX = new TextBlock()
                {
                    Text       = "  ✕",
                    Foreground = new SolidColorBrush(Color.FromArgb(140, 252, 165, 165)),
                    FontSize   = 11,
                    VerticalAlignment = VerticalAlignment.Center
                };
                chipContent.Children.Add(chipText);
                chipContent.Children.Add(chipX);
                chipBorder.Child = chipContent;

                // Set the styled Border as the button's Content with a passthrough template
                chip.Content = chipBorder;
                chip.Template = new ControlTemplate(typeof(Button))
                {
                    VisualTree = PassthroughFactory()
                };

                // Hover glow effect
                chip.MouseEnter += (s, e) =>
                {
                    chipBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(124, 58, 237));
                    chipBorder.Effect = new System.Windows.Media.Effects.DropShadowEffect
                    { Color = Color.FromRgb(124, 58, 237), BlurRadius = 14, ShadowDepth = 0, Opacity = 0.6 };
                };
                chip.MouseLeave += (s, e) =>
                {
                    chipBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(60, 124, 58, 237));
                    chipBorder.Effect = null;
                };

                string capturedGroup = group;
                chip.Click += (s, e) =>
                {
                    if (pathToAdd.Equals("") && !removeMode)
                    {
                        var v = MessageBox.Show("Tem certeza que deseja excluir essa categoria?",
                                                "Excluir grupo?", MessageBoxButton.YesNo);
                        if (v == MessageBoxResult.Yes)
                        {
                            ConfigManager.RemoveGroup(capturedGroup);
                            groupsMngr.Children.Remove(chip);
                            mainWindow.Show();
                            mainWindow.Reload();
                        }
                    }
                    else if (!pathToAdd.Equals("") && !removeMode)
                    {
                        ConfigManager.AddPathOnGroup(pathToAdd, capturedGroup);
                        mainWindow.Show();
                        mainWindow.Reload();
                        Close();
                    }
                    else
                    {
                        ConfigManager.RemovePathOnGroup(pathToAdd, capturedGroup);
                        mainWindow.Show();
                        mainWindow?.Reload();
                        Close();
                    }
                };

                groupsMngr.Children.Add(chip);
            }
        }

        /// <summary>
        /// Simple passthrough template: renders Button.Content directly (no extra wrapping Border).
        /// The styling is applied to the chip.Content (a Border) already.
        /// </summary>
        private FrameworkElementFactory PassthroughFactory()
        {
            FrameworkElementFactory cp = new FrameworkElementFactory(typeof(ContentPresenter));
            cp.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            cp.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            return cp;
        }
    }
}