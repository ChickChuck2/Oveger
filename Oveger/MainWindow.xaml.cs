using Microsoft.Win32;
using NReco.VideoConverter;
using Oveger.Scripts;
using Oveger.XAMLS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Oveger
{
    /// <summary>
    /// Lógica principal da MainWindow — Oveger Overlay
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG", "JPGE" };

        public static RoutedCommand MyCommand = new RoutedCommand();

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [Flags]
        public enum Modifiers { NoMod = 0x0000, Alt = 0x0001, Ctrl = 0x0002, Shift = 0x0004, Win = 0x0008 }
        private enum TypeofGet { GetIconEXT, GetIcon }

        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        readonly System.Windows.Forms.ContextMenuStrip context = new System.Windows.Forms.ContextMenuStrip();
        readonly System.Windows.Forms.ToolStripMenuItem close            = new System.Windows.Forms.ToolStripMenuItem { Text = "Fechar" };
        readonly System.Windows.Forms.ToolStripMenuItem show             = new System.Windows.Forms.ToolStripMenuItem { Text = "Mostrar" };
        readonly System.Windows.Forms.ToolStripMenuItem StartWithWindows = new System.Windows.Forms.ToolStripMenuItem { Text = "Iniciar com Windows" };
        readonly System.Windows.Forms.ToolStripMenuItem changehotkey     = new System.Windows.Forms.ToolStripMenuItem { Text = "Atalhos e ajuda" };
        readonly System.Windows.Forms.ToolStripMenuItem groupsTray       = new System.Windows.Forms.ToolStripMenuItem { Text = "Gerenciar Grupos" };

        public MainWindow() => InitializeComponent();

        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Hide();
            inprogram = false;
        }

        private void Window1_Closed(object sender, EventArgs e) => notifyIcon.Visible = false;

        // ────────────────────────────────────────────────────────
        // ADD BUTTON  (the "+" card)
        // ────────────────────────────────────────────────────────
        private void CreateAddButton()
        {
            Button addnewitem = new Button()
            {
                Style = FindResource("AddButton") as Style,
                Margin = new Thickness(6)
            };

            TextBlock label = new TextBlock()
            {
                Text = "Adicionar",
                FontSize = 11,
                Foreground = HexBrush("#94A3B8"),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 0)
            };

            StackPanel cardStack = new StackPanel()
            {
                Width = 102,
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(4, 6, 4, 6)
            };
            cardStack.Children.Add(addnewitem);
            cardStack.Children.Add(label);

            addnewitem.Click += (s, e) => CreateButtonFile();
            ungroupedPanel.Children.Add(cardStack);
        }

        private void Window1_initialized(object sender, EventArgs e)
        {
            MyCommand.InputGestures.Add(new KeyGesture(Key.Escape));
        }

        private void Window1_LocationChanged(object sender, EventArgs e)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
            System.Windows.Forms.Screen curr = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            Window1.UpdateLayout();
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(PresentationSource.FromVisual(this) is HwndSource source))
                throw new Exception("Could not create hWnd source from window.");
            source.AddHook(WndProc);

            CreateAddButton();
            ConfigManager.LoadOrCreate(this);
            RegisterHotKey(new WindowInteropHelper(this).Handle, 2, (int)ConfigManager.GetMODKey(0) | (int)ConfigManager.GetMODKey(1), (int)ConfigManager.GetKey());
            TaskbarInitialize();
            Window1.WindowState = WindowState.Maximized;
            Hide();
            Window1.WindowState = WindowState.Normal;
        }

        // ────────────────────────────────────────────────────────
        // GROUPS — Expanders in wrappanel1 (StackPanel)
        // ────────────────────────────────────────────────────────
        Expander GetOrCreateExpander(string group)
        {
            Expander customGroup = (Expander)this.FindName(group);
            if (customGroup == null)
            {
                customGroup = new Expander
                {
                    Name = group,
                    IsExpanded = true,
                    Header = group,
                    Style = FindResource("GlassExpander") as Style,
                    Margin = new Thickness(0, 4, 0, 0)
                };
                this.RegisterName(group, customGroup);
                wrappanel1.Children.Add(customGroup);
            }
            return customGroup;
        }

        [Obsolete]
        public async Task VerifyPaths(Action action)
        {
            while (true)
            {
                action();
                Task task = Task.Delay(TimeSpan.FromMinutes(30));
                try { await task; }
                catch { return; }
            }
        }

        private void TaskbarInitialize()
        {
            close.Click            += (s, e) => Process.GetCurrentProcess().Kill();
            show.Click             += (s, e) => Show();
            StartWithWindows.Checked = ConfigManager.GetBool();
            StartWithWindows.Click += (s, e) =>
            {
                StartWithWindows.Checked = !StartWithWindows.Checked;
                RegKeyRegister.SetStartup(StartWithWindows.Checked);
                ConfigManager.ChangeStartWithWindows();
            };
            changehotkey.Click += (s, e) =>
            {
                Helpandshortcut h = new Helpandshortcut();
                h.Show();
            };
            groupsTray.Click += (s, e) =>
            {
                groupsWindow gw = new groupsWindow();
                gw.mainWindow = this;
                gw.Show();
            };

            context.Items.Add(show);
            context.Items.Add(StartWithWindows);
            context.Items.Add(changehotkey);
            context.Items.Add(groupsTray);
            context.Items.Add(close);

            IntPtr icon = Properties.Resources.icon.GetHicon();
            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Text = "Oveger",
                ContextMenuStrip = context,
                Icon = System.Drawing.Icon.FromHandle(icon),
                Visible = true
            };
            notifyIcon.MouseClick += (s, e) =>
            {
                if (((System.Windows.Forms.MouseEventArgs)e).Button == System.Windows.Forms.MouseButtons.Left)
                    Show();
            };
        }

        private void CreateButtonFile()
        {
            OpenFileDialog AddItem = new OpenFileDialog
            {
                Multiselect = true,
                DereferenceLinks = false,
                Filter = "All files|*.*|images|*.png;*.jpeg;*.jpg|exe|*.exe|gif|*.gif"
            };
            if (AddItem.ShowDialog() == true)
            {
                foreach (string path in AddItem.FileNames)
                {
                    SetConfig(path);
                    ConfigManager.SavePath(path);
                }
            }
        }

        // ────────────────────────────────────────────────────────
        // ITEM CARD  — the heart of the new UI
        // ────────────────────────────────────────────────────────
        public void SetConfig(string path)
        {
            string fileName = Path.GetFileName(path);
            string labelText = ConfigManager.GetLabelName(path, fileName);

            // Outer card container
            StackPanel cardStack = new StackPanel()
            {
                Width = 102,
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(4, 6, 4, 6),
                Cursor = Cursors.Hand
            };

            // Glass card border
            Border cardBorder = new Border()
            {
                Width = 90,
                Height = 90,
                CornerRadius = new CornerRadius(16),
                BorderThickness = new Thickness(1),
                ClipToBounds = true,
                SnapsToDevicePixels = true
            };
            cardBorder.Background    = new SolidColorBrush(Color.FromArgb(160, 26, 23, 48));
            cardBorder.BorderBrush   = new SolidColorBrush(Color.FromArgb(26, 255, 255, 255));
            cardBorder.Effect        = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color       = Color.FromRgb(0, 0, 0),
                BlurRadius  = 16,
                ShadowDepth = 3,
                Opacity     = 0.45
            };

            // Image inside the card
            ImageBrush iconBrush = new ImageBrush() { Stretch = Stretch.UniformToFill };
            cardBorder.Background = iconBrush;

            // Try to load icon
            string EXT = Path.GetExtension(path).ToUpper();
            bool hasImage = false;
            try
            {
                if (ImageExtensions.Contains(EXT) || EXT == ".GIF")
                {
                    iconBrush.ImageSource = SetImage(path);
                    hasImage = true;
                }
                else if (EXT == ".EXE" || EXT == ".URL")
                {
                    System.Drawing.Bitmap bmp = ExtractIconfromFile(path).ToBitmap();
                    iconBrush = ConvertBitmapToImageBrush(bmp);
                    bmp.Dispose();
                    hasImage = true;
                }
                else if (EXT == ".MP4" || EXT == ".MKV" || EXT == ".WEBP")
                {
                    FFMpegConverter ff = new FFMpegConverter();
                    string thumbDir = "thumbnails";
                    string thumb    = fileName + ".jpg";
                    string thumbPath = Path.Combine(thumbDir, thumb);
                    if (!Directory.Exists(thumbDir)) Directory.CreateDirectory(thumbDir);
                    if (!File.Exists(thumbPath))
                        ff.GetVideoThumbnail(path, @"thumbnails\" + thumb);
                    iconBrush.ImageSource = SetImage(thumbPath);
                    hasImage = true;
                }
                else
                {
                    iconBrush.ImageSource = IconManager.FindIconForFilename(path, true);
                    hasImage = true;
                }
            }
            catch { /* silently fail – show empty card */ }

            // Apply the icon brush as background
            if (hasImage)
            {
                cardBorder.Background    = iconBrush;
                cardBorder.BorderBrush   = new SolidColorBrush(Color.FromArgb(26, 255, 255, 255));
                cardBorder.Effect        = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color       = Color.FromRgb(0, 0, 0),
                    BlurRadius  = 16,
                    ShadowDepth = 3,
                    Opacity     = 0.45
                };
            }
            else
            {
                // Fallback: gradient with first letter
                cardBorder.Background = new LinearGradientBrush(
                    Color.FromRgb(124, 58, 237),
                    Color.FromRgb(79, 70, 229),
                    new Point(0, 0), new Point(1, 1));
            }

            // Hover border glow via mouse events
            cardBorder.MouseEnter += (s, e) =>
            {
                cardBorder.BorderBrush  = HexBrush("#7C3AED");
                cardBorder.BorderThickness = new Thickness(1.5);
                cardBorder.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Color.FromRgb(124, 58, 237),
                    BlurRadius = 22,
                    ShadowDepth = 0,
                    Opacity = 0.70
                };
                cardStack.RenderTransformOrigin = new Point(0.5, 0.5);
                ScaleTransform scale = new ScaleTransform(1.06, 1.06);
                cardStack.RenderTransform = scale;
            };
            cardBorder.MouseLeave += (s, e) =>
            {
                cardBorder.BorderBrush     = new SolidColorBrush(Color.FromArgb(26, 255, 255, 255));
                cardBorder.BorderThickness = new Thickness(1);
                cardBorder.Effect          = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Color.FromRgb(0, 0, 0),
                    BlurRadius = 16,
                    ShadowDepth = 3,
                    Opacity = 0.45
                };
                cardStack.RenderTransform = new ScaleTransform(1.0, 1.0);
            };

            // Click & right-click
            cardBorder.MouseLeftButtonDown  += (s, e) => { if (e.ClickCount == 1) OpenFileProcess(path); };
            cardBorder.MouseRightButtonDown += (s, e) => PropertyRighClick(path, cardStack, labelBlock(cardStack));

            // Label
            TextBlock lbl = new TextBlock()
            {
                Text            = labelText,
                FontSize        = 11,
                Foreground      = HexBrush("#CBD5E1"),
                TextAlignment   = TextAlignment.Center,
                TextWrapping    = TextWrapping.Wrap,
                MaxWidth        = 98,
                Margin          = new Thickness(0, 5, 0, 0),
                TextTrimming    = TextTrimming.CharacterEllipsis
            };
            lbl.Tag = "label"; // used to retrieve it by right-click handler

            cardStack.Children.Add(cardBorder);
            cardStack.Children.Add(lbl);

            ConfigGroups(path, cardStack);
        }

        // Helper: retrieves the TextBlock label from a cardStack
        private TextBlock labelBlock(StackPanel stack)
        {
            foreach (UIElement el in stack.Children)
                if (el is TextBlock tb && (string)tb.Tag == "label")
                    return tb;
            return null;
        }

        void ConfigGroups(string path, StackPanel card)
        {
            if (ConfigManager.GetGroupByPath(path) != string.Empty)
            {
                StackPanel customStack = (StackPanel)this.FindName(ConfigManager.GetGroupByPath(path) + "stack");
                if (customStack == null)
                {
                    customStack = new StackPanel { Orientation = Orientation.Vertical };
                    WrapPanel groupItems = new WrapPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(8, 4, 8, 8) };
                    customStack.Children.Add(groupItems);
                    this.RegisterName(ConfigManager.GetGroupByPath(path) + "stack", customStack);
                }

                // Put the card in the first WrapPanel child of the stack
                WrapPanel wp = customStack.Children[0] as WrapPanel;
                wp?.Children.Add(card);

                var ex = GetOrCreateExpander(ConfigManager.GetGroupByPath(path));
                ex.Content = customStack;
            }
            else
                ungroupedPanel.Children.Add(card);
        }

        public void Reload()
        {
            ungroupedPanel.Children.Clear();
            CreateAddButton();
            foreach (var c in ConfigManager.GetGroups())
            {
                var sp = (StackPanel)this.FindName(c + "stack");
                if (sp != null)
                {
                    WrapPanel wp = sp.Children[0] as WrapPanel;
                    wp?.Children.Clear();
                }
            }
            ConfigManager.LoadOrCreate(this);
        }

        // ────────────────────────────────────────────────────────
        // CONTEXT MENU (right-click card)
        // ────────────────────────────────────────────────────────
        private void PropertyRighClick(string path, StackPanel cardToDelete, TextBlock textblock, string oldPath = null)
        {
            RightButtonClick right = new RightButtonClick();
            oldPath = path;

            Closing += (s, e) => right.Close();
            right.open.Click            += (s, e) => { OpenFileProcess(path); right.Close(); };
            right.rename.Click          += (s, e) => { RenameLabel(path, textblock); right.Close(); };
            right.changepath.Click      += (s, e) => { ChangePath(oldPath); right.Close(); };
            right.DeleteButton.Click    += (s, e) => { DeleteFile(path, cardToDelete); right.Close(); };
            right.DeleteinProgram.Click += (s, e) => { DeleteInProgram(path, cardToDelete); right.Close(); };
            right.openfolderpath.Click  += (s, e) => { OpenFilePath(path); right.Close(); };
            right.property.Click        += (s, e) => { OpenProperty(path); right.Close(); };

            if (!ConfigManager.GetGroupByPath(path).Equals(string.Empty))
                right.addgroup.Content = "Remover do Grupo";
            else
                right.addgroup.Content = "Adicionar a Grupo";

            right.addgroup.Click += (s, e) =>
            {
                if (!ConfigManager.GetGroupByPath(path).Equals(string.Empty))
                {
                    ConfigManager.RemovePathOnGroup(path, ConfigManager.GetGroupByPath(path));
                    Reload();
                    right.Close();
                }
                else
                {
                    groupsWindow gw = new groupsWindow();
                    gw.mainWindow = this;
                    gw.pathToAdd  = path;
                    gw.labelmain.FontSize = 12;
                    gw.labelmain.Content  = "Clique em um grupo para Adicionar";
                    Hide();
                    gw.Show();
                }
            };

            Point mp = VariablesClasses.Mouse.GetMousePosition();
            right.Left = mp.X - 50;
            right.Top  = mp.Y + 30;

            int entry = 0;
            right.MouseEnter += (s, e) => entry++;
            right.MouseLeave += (s, e) => { entry++; if (entry > 1) right.Hide(); entry = 0; };

            right.Show();
        }

        private void RenameLabel(string path, TextBlock textblock)
        {
            Hide();
            LocalRenameWindow lrw = new LocalRenameWindow();
            lrw.Show();
            lrw.ConfirmButton.Click += (s, e) =>
            {
                string name = lrw.renamedbox.Text;
                if (textblock != null) textblock.Text = name;
                lrw.Close();
                if (ConfigManager.RenameLabel(path, name) > 0)
                    Reload();
                Show();
            };
        }

        private void ChangePath(string oldPath)
        {
            string newPath = "";
            ChangePathForm cpf = new ChangePathForm();
            cpf.oldPathBox.Text = oldPath;
            cpf.confirmbutton.Click += (s, e) =>
            {
                newPath = cpf.newPathBox.Text;
                if (!Directory.Exists(newPath + @"\.."))
                    Directory.CreateDirectory(newPath + @"\..");
                File.Move(oldPath, newPath);
                ConfigManager.ChangePath(oldPath, newPath);
                cpf.Close();
                ungroupedPanel.Children.Clear();
                CreateAddButton();
                ConfigManager.LoadOrCreate(this);
                Show();
            };
            Hide();
            cpf.Show();
        }

        private void DeleteFile(string path, StackPanel card)
        {
            MessageBoxResult dialog = MessageBox.Show(this,
                $"Deseja deletar {Path.GetFileName(path)} permanentemente?",
                "Confirmação", MessageBoxButton.YesNo);
            if (dialog == MessageBoxResult.Yes)
            {
                try
                {
                    // Remove from its parent panel
                    Panel parent = VisualTreeHelper.GetParent(card) as Panel;
                    parent?.Children.Remove(card);
                    File.Delete(path);
                    if (path.Contains(".mp4"))
                        File.Delete(Path.Combine("thumbnails", Path.GetFileName(path) + ".jpg"));
                    ConfigManager.VerifyPaths(false);
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            }
        }

        private void DeleteInProgram(string path, StackPanel card)
        {
            Panel parent = VisualTreeHelper.GetParent(card) as Panel;
            parent?.Children.Remove(card);
            if (path.Contains(".mp4"))
                File.Delete(Path.Combine("thumbnails", Path.GetFileName(path) + ".jpg"));
            ConfigManager.Remove(path);
        }

        private Process OpenFilePath(string path)
        {
            Process p = new Process();
            p.StartInfo.FileName  = "explorer.exe";
            p.StartInfo.Arguments = $"/select, \"{path}\"";
            p.Start();
            Hide();
            return p;
        }

        private void OpenProperty(string path)
        {
            VariablesClasses.OpenPropertyDialog(path);
            Hide();
        }

        private Process OpenFileProcess(string path)
        {
            Hide();
            inprogram = false;
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = path;
                p.Start();
                return p;
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
                return null;
            }
        }

        // ────────────────────────────────────────────────────────
        // HELPERS
        // ────────────────────────────────────────────────────────
        private ImageBrush ConvertBitmapToImageBrush(System.Drawing.Bitmap img)
        {
            BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(
                img.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return new ImageBrush { ImageSource = bs };
        }

        private ImageSource SetImage(string filePath)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource   = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            return bmp;
        }

        public System.Drawing.Icon ExtractIconfromFile(string path)
            => System.Drawing.Icon.ExtractAssociatedIcon(path);

        private SolidColorBrush HexBrush(string hex)
            => (SolidColorBrush)new BrushConverter().ConvertFrom(hex);

        bool inprogram = false;
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0312)
            {
                if (inprogram) Hide();
                else           Show();
                inprogram = !inprogram;
            }
            return IntPtr.Zero;
        }

        [Obsolete]
        private void ReloadButtons()
        {
            ungroupedPanel.Children.Clear();
            CreateAddButton();
            ConfigManager.LoadOrCreate(this);
        }
    }
}