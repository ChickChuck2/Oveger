﻿using Microsoft.Win32;
using NReco.VideoConverter;
using Oveger.XAMLS;
using Shell32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Oveger
{
    /// <summary>
    /// Interação lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VariablesClasses variables = new VariablesClasses();

        public MainWindow()
        {
            InitializeComponent();
        }

        public static RoutedCommand MyCommand = new RoutedCommand();
        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Hide();
        }

        [Flags]
        public enum Modifiers
        {
            NoMod = 0x0000,
            Alt = 0x0001,
            Ctrl = 0x0002,
            Shift = 0x0004,
            Win = 0x0008
        }

        public readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPE", ".BMP", ".GIF", ".PNG", "JPGE" };

        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        private async Task DownloadFile(string URL, string fileName)
        {
            DownloadProgress progress = new DownloadProgress();
            progress.Show();
            progress.SetText(fileName);

            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
            myWebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler((object sender, DownloadProgressChangedEventArgs e )=>
            {
                Application.Current.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new Action(() =>  {try {progress.progressDownload.Value = e.ProgressPercentage;} catch { progress.Show(); }; }));
            });
            myWebClient.DownloadFileCompleted += new AsyncCompletedEventHandler((object sender, AsyncCompletedEventArgs e) =>
            {
                Application.Current.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new Action(() =>
                    {
                        progress.Background = new SolidColorBrush(Colors.Green);
                        progress.Opacity = 0.75f;
                        progress.setSeconds(5);
                    }));
            });

            Uri uriS = new Uri(URL);
            await Task.Run(() => myWebClient.DownloadFileAsync(uriS, fileName));
        }

        private void CreateAddButton()
        {
            Grid addItemGrid = new Grid();
            TextBlock ItemName = new TextBlock();
            Button addnewitem = new Button();

            wrappanel1.Width = wrappanel1.Width - 50;
            wrappanel1.Height = wrappanel1.Height - 50;

            // BUTTON START SETTINGS
            addItemGrid.Width = 70;
            addItemGrid.Height = 90;

            addItemGrid.Children.Add(addnewitem);

            ItemName.Text = "Adicionar Arquivo";
            ItemName.Width = 70;
            ItemName.Height = double.NaN;
            ItemName.Margin = new Thickness { Top = 77 };
            ItemName.TextAlignment = TextAlignment.Center;

            addItemGrid.Children.Add(ItemName);

            addnewitem.Style = FindResource("AddButton") as Style;
            addnewitem.Click += new RoutedEventHandler((object sender1, RoutedEventArgs e1) => CreateButtonFile());
            //BUTTON END SETTINGS

            wrappanel1.Children.Add(addItemGrid);
        }
        private void window1_initialized(object sender, EventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!isAdmin)
            {
                //Public domain; no attribution required.
                ProcessStartInfo info = new ProcessStartInfo(VariablesClasses.AppPath);
                info.UseShellExecute = true;
                info.Verb = "runas";
                Process.Start(info);
                Process.GetCurrentProcess().Kill();

            }

            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            MyCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            CreateAddButton();
        }
        public string[] Hotkeys { get; set; }
        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            var source = PresentationSource.FromVisual(this) as HwndSource;
            if (source == null)
                throw new Exception("Could not create hWnd source from window.");
            source.AddHook(WndProc);
            RegisterHotKey(new WindowInteropHelper(this).Handle, 2, (int)ConfigManager.GetMODKey(0) | (int)ConfigManager.GetMODKey(1), (int)ConfigManager.GetKey());
            TaskbarInitialize();
            Hide();

            ConfigManager.LoadOrCreate(this);
            ConfigManager.verifyPaths(true);
        }

        System.Windows.Forms.ContextMenuStrip context = new System.Windows.Forms.ContextMenuStrip();
        System.Windows.Forms.ToolStripMenuItem close = new System.Windows.Forms.ToolStripMenuItem { Text = "Fechar" };
        System.Windows.Forms.ToolStripMenuItem show = new System.Windows.Forms.ToolStripMenuItem { Text = "Mostrar" };
        System.Windows.Forms.ToolStripMenuItem StartWithWindows = new System.Windows.Forms.ToolStripMenuItem { Text = "Iniciar com Windows" };
        System.Windows.Forms.ToolStripMenuItem changehotkey = new System.Windows.Forms.ToolStripMenuItem { Text = "Atalhos e ajuda" };

        private void TaskbarInitialize()
        {
            close.Click += new EventHandler((object sender, EventArgs e) => Process.GetCurrentProcess().Kill());
            show.Click += new EventHandler((object sender, EventArgs e) => Show());
            StartWithWindows.Click += new EventHandler((object sender, EventArgs e) =>
            {
                StartWithWindows.Checked = !StartWithWindows.Checked;
                RegKeyRegister.SetStartup(StartWithWindows.Checked);
                ConfigManager.Save(StartWithWindows.Checked, null);
            });
            changehotkey.Click += new EventHandler((object sender, EventArgs e) =>
            {
                helpandshortcut helpandshortcut = new helpandshortcut();
                helpandshortcut.Show();
            });

            context.Items.Add(show);
            context.Items.Add(StartWithWindows);
            context.Items.Add(changehotkey);
            context.Items.Add(close);
            //context.Container.Add(changehotkey);
            //context.Container.Add(close);

            IntPtr icon = Properties.Resources.icon.GetHicon();
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = "Oveger";
            notifyIcon.ContextMenuStrip = context;
            notifyIcon.Icon = System.Drawing.Icon.FromHandle(icon);
            notifyIcon.Visible = true;
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler((object sender, System.Windows.Forms.MouseEventArgs e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    Show();
            });
        }
        private void CreateButtonFile()
        {
            OpenFileDialog AddItem = new OpenFileDialog();
            AddItem.Multiselect = true;
            AddItem.DereferenceLinks = false;
            AddItem.Filter = "All files|*.*" +
                "|images|*.png;*.jpeg;*.jpg" +
                "|exe|*.exe" +
                "|gif|*.gif";
            if (AddItem.ShowDialog() == true)
            {
                string[] paths = AddItem.FileNames;

                foreach(string path in paths)
                {
                    string VPath = path;
                    if(Path.GetExtension(path).ToUpper() == ".lnk".ToUpper())
                        VPath = GetLnkPath(path);
                    SetConfig(VPath);
                    ConfigManager.Save(StartWithWindows.Checked, VPath);
                }
            }
        }
        
        public void SetConfig(string path)
        {
            string EXT = Path.GetExtension(path).ToUpper();
            string FileName = Path.GetFileName(path);
            Grid addItemGrid = new Grid() { Width = 70, Height = 90 };
            Button addnewitem = new Button() { Style = FindResource("AddButton") as Style };
            TextBlock ItemName = new TextBlock() { Text = ConfigManager.GetLabelName(path, FileName), Width = 70, Height = double.NaN, Margin = new Thickness { Top = 77 }, TextAlignment = TextAlignment.Center };
            addItemGrid.Children.Add(addnewitem);
            addItemGrid.Children.Add(ItemName);
            wrappanel1.Children.Add(addItemGrid);

            ImageBrush img = new ImageBrush() { Stretch = Stretch.UniformToFill };
            if (ImageExtensions.Contains(EXT) || EXT.Contains(".gif".ToUpper()))
                setButtonConfig(TypeofGet.Icon, img, addnewitem, path);
            else if (EXT.Contains(".exe".ToUpper()) || EXT.Contains(".url".ToUpper()))
            {
                img = ConvertBitmapToImageBrush(ExtractIconfromFile(path).ToBitmap());
                addnewitem.Style = setStyle(addnewitem, img);
            }
            else if (EXT.Contains(".lnk".ToUpper()))
            {
                img = ConvertBitmapToImageBrush(ExtractIconfromFile(path).ToBitmap());
                setButtonConfig(TypeofGet.IconEXT, img, addnewitem, path);
            }
            else if (EXT.Contains(".mp4".ToUpper()) || EXT.Contains(".mkv".ToUpper()) || EXT.Contains(".webp".ToUpper()))
            {
                FFMpegConverter ffMpeg = new FFMpegConverter();
                string thumbDirectory = @"thumbnails";
                string thumbnail = FileName + ".jpg";
                string thumbFileDir = Path.Combine(thumbDirectory, thumbnail);
                if (!Directory.Exists("thumbnails"))
                    Directory.CreateDirectory("thumbnails");
                if (!File.Exists(Path.Combine(thumbDirectory, thumbnail)))
                    ffMpeg.GetVideoThumbnail(path, @"thumbnails\" + thumbnail);
                img.ImageSource = SetImage(thumbFileDir);
                addnewitem.Style = setStyle(addnewitem, img);
            }
            else
                setButtonConfig(TypeofGet.IconEXT, img, addnewitem, path);

            addnewitem.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => OpenFileProcess(path));
            addnewitem.MouseRightButtonDown += new MouseButtonEventHandler((object sender, MouseButtonEventArgs e) => PropertyRighClick(path, addItemGrid, ItemName));
        }
        private enum TypeofGet { IconEXT, Icon }
        private void setButtonConfig(TypeofGet type,ImageBrush img, Button addnewitem, string path)
        {
            Console.WriteLine($"EXTRACTING ICON FROM {path}");
            if(type == TypeofGet.IconEXT)
                img.ImageSource = IconManager.FindIconForFilename(path, true);
            else
                img.ImageSource = SetImage(path);
            addnewitem.Style = setStyle(addnewitem, img);
        }

        private ImageSource SetImage(string FilePath)
        {
            Uri iri = new Uri(FilePath, UriKind.Relative);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = iri;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        private void PropertyRighClick(string path, Grid gridToDelete, TextBlock textblock, string oldPath = null)
        {
            RightButtonClick right = new RightButtonClick();
            oldPath = path;

            Closing += new CancelEventHandler((object sender, CancelEventArgs e) => right.Close());
            right.open.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { OpenFileProcess(path);right.Close(); });
            right.rename.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { RenameLabel(path, textblock);right.Close(); });
            right.changepath.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { ChangePath(oldPath); right.Close(); });
            right.DeleteButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { DeleteFile(path, gridToDelete); right.Close(); });
            right.DeleteinProgram.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { DeleteInProgram(path, gridToDelete); right.Close(); });
            right.openfolderpath.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { OpenFilePath(path); right.Close(); });
            right.property.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => { OpenProperty(path); right.Close(); });

            int entry = 0;
            Point MouseP = new Point { X = GetMousePosition().X, Y = GetMousePosition().Y };

            right.Left = MouseP.X - 50;
            right.Top = MouseP.Y + 30;
            right.MouseEnter += new MouseEventHandler((object sender1, MouseEventArgs e1) => entry = entry + 1);
            right.MouseLeave += new MouseEventHandler((object sender1, MouseEventArgs e1) => { entry = entry + 1; if (entry > 1) right.Hide(); entry = 0; });

            right.Show();
        }

        private void RenameLabel(string path,TextBlock textblock)
        {
            Hide();
            LocalRenameWindow localRename = new LocalRenameWindow();
            localRename.Show();
            localRename.ConfirmButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                string Filename = localRename.renamedbox.Text;
                textblock.Text = Filename;
                ConfigManager.RenameLabel(path,Filename);
                localRename.Close();
                Show();
            });
        }
        private void ChangePath(string oldPath)
        {
            string newPath = "";
            ChangePathForm changePath = new ChangePathForm();
            changePath.oldPathBox.Text = oldPath;
            changePath.confirmbutton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                newPath = changePath.newPathBox.Text;
                if(!Directory.Exists(newPath+@"\.."))
                    Directory.CreateDirectory(newPath+@"\..");
                File.Move(oldPath, newPath);
                ConfigManager.ChangePath(oldPath, newPath);
                changePath.Close();
                wrappanel1.Children.Clear();
                CreateAddButton();
                ConfigManager.LoadOrCreate(this);
                Show();
            });
            Hide();
            changePath.Show();
        }
        private void DeleteFile(string path, Grid grid)
        {
            MessageBoxResult dialog =  MessageBox.Show(this, $"Deseja deletar {Path.GetFileName(path)} permanentemente?", "Confirmação", MessageBoxButton.YesNo);
            if (dialog == MessageBoxResult.Yes)
            {
                try
                {
                    wrappanel1.Children.Remove(grid);
                    File.Delete(path);
                    if (path.Contains(".mp4"))
                        File.Delete(Path.Combine("thumbnails", Path.GetFileName(path)+".jpg"));
                    ConfigManager.verifyPaths(false);
                }catch(Exception ex) {Console.WriteLine(ex);}
            }
        }
        private void DeleteInProgram(string path, Grid grid)
        {
            wrappanel1.Children.Remove(grid);
            if (path.Contains(".mp4"))
                File.Delete(Path.Combine("thumbnails", Path.GetFileName(path) + ".jpg"));
            ConfigManager.Remove(path);
        }
        private Process OpenFilePath(string path)
        {
            Process process = new Process();
            string argument = "/select, \"" + path + "\"";
            process.StartInfo.FileName = "explorer.exe";
            process.StartInfo.Arguments = argument;
            process.Start();
            Hide();
            return process;
        }
        private void OpenProperty(string path)
        {
            variables.OpenPropertyDialog(path);
            Hide();
        }
        private Process OpenFileProcess(string path)
        {
            Hide();
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = path;
                process.Start();
                return process;
            }
            catch (Win32Exception W32E)
            {
                MessageBox.Show($"{W32E.Message}");
                return null;
            }
        }

        private ImageBrush ConvertBitmapToImageBrush(System.Drawing.Bitmap img)
        {
            ImageBrush ib = new ImageBrush();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(img.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            ib.ImageSource = bitmapSource;

            return ib;
        }

        private Style setStyle(Button addnewitem, object img)
        {
            DataTrigger tg = new DataTrigger()
            {
                Binding = new Binding("mousehover"),
                Value = 1
            };
            tg.Setters.Add(new Setter()
            {
                Property = Button.BackgroundProperty,
                Value = Brushes.Green
            });

            addnewitem.Style = new Style(typeof(Button))
            {
                Setters =
                {
                    new Setter { Property = ContentProperty, Value = "-"},
                    new Setter { Property = HorizontalContentAlignmentProperty, Value = HorizontalAlignment.Center },
                    new Setter { Property = VerticalAlignmentProperty, Value = VerticalAlignment.Center },
                    new Setter { Property = WidthProperty, Value = 70d},
                    new Setter { Property = HeightProperty, Value = 70d},
                    new Setter { Property = ForegroundProperty,Value = Brushes.White},
                    new Setter { Property = FontSizeProperty, Value = 30d},
                    new Setter
                    {
                        Property = TemplateProperty,
                        Value = new ControlTemplate(typeof(Button))
                        {
                            VisualTree = CreateFactory(img)
                        }
                    }
                }
            };
            return addnewitem.Style;
        }

        private FrameworkElementFactory CreateFactory(object img)
        {
            // --START OF MAIN FACTORY-- //
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Border));
            factory.SetValue(Border.BackgroundProperty, img);
            factory.SetValue(Border.CornerRadiusProperty, new CornerRadius
            { TopLeft = 10, TopRight = 10, BottomLeft = 10, BottomRight = 10 });
            factory.SetValue(Border.BorderBrushProperty, BRUSHERHEX("#696969"));
            factory.SetValue(Border.BorderThicknessProperty, new Thickness
            { Top = 1, Left = 1, Right = 1, Bottom = 1, });
            // --END OF MAIN FACTORY-- //

            FrameworkElementFactory contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.MarginProperty, new Thickness
            { Top = -1, Left = -1, Right = -1, Bottom = -1 });
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            //contentFactory.SetValue(ContentPresenter.ContentProperty, "-");
            factory.AppendChild(contentFactory);
            return factory;

        }

        public System.Drawing.Icon ExtractIconfromFile(string pathEXE)
        {
            System.Drawing.Icon result = null;
            result = System.Drawing.Icon.ExtractAssociatedIcon(pathEXE);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <example>BRUSHERHEX("#ffaacc")</example>
        /// <param name="HEXFormater"></param>
        /// <returns></returns>
        private SolidColorBrush BRUSHERHEX(string HEXFormater)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFrom(HEXFormater);
        }

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0312)
            {
                Show();
            }
            return IntPtr.Zero;
        }


        public static string GetLnkPath(string path)
        {
            string pathOnly = Path.GetDirectoryName(path);
            string filenameOnly = Path.GetFileName(path);

            Shell shell = new Shell();
            Folder folder = shell.NameSpace(pathOnly);
            FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                ShellLinkObject link = (ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }
            return string.Empty;
        }
    }
}